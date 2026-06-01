using AutoMapper;
using ERPLite.Data.Entities.Sales;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.Sales;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.Sales;
using ERPLite.Services.Interfaces.System;
using ERPLite.Shared.Constants;
using ERPLite.Shared.Enums;

namespace ERPLite.Services.Services.Sales
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IActivityLogService _activityLogService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IActivityLogService activityLogService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _activityLogService = activityLogService;
        }

        public async Task<ServiceResult<OrderDto>> GetOrderDetailsAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(id);
            if (order == null)
                return ServiceResult<OrderDto>.Failed("Order not found.");

            var dto = _mapper.Map<OrderDto>(order);
            return ServiceResult<OrderDto>.Successful(dto);
        }

        public async Task<ServiceResult<IEnumerable<OrderDto>>> GetRecentOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetRecentOrdersAsync(20);
            var dto = _mapper.Map<IEnumerable<OrderDto>>(orders);

            return ServiceResult<IEnumerable<OrderDto>>.Successful(dto);
        }

        public async Task<ServiceResult<IEnumerable<OrderDto>>> GetOrdersByCustomerAsync(int customerId)
        {
            var customerExists = await _unitOfWork.Customers.GetByIdAsync(customerId) != null;
            if (!customerExists)
                return ServiceResult<IEnumerable<OrderDto>>.Failed("Customer not found.");

            var orders = await _unitOfWork.Orders.GetOrdersByCustomerAsync(customerId);
            var dto = _mapper.Map<IEnumerable<OrderDto>>(orders);

            return ServiceResult<IEnumerable<OrderDto>>.Successful(dto);
        }

        public async Task<ServiceResult<IEnumerable<OrderDto>>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return ServiceResult<IEnumerable<OrderDto>>.Failed("Start date cannot be greater than end date.");

            var orders = await _unitOfWork.Orders.GetOrdersByDateRangeAsync(startDate, endDate);
            var dto = _mapper.Map<IEnumerable<OrderDto>>(orders);

            return ServiceResult<IEnumerable<OrderDto>>.Successful(dto);
        }

        public async Task<ServiceResult<decimal>> GetTotalRevenueAsync()
        {
            var totalRevenue = await _unitOfWork.Orders.GetTotalRevenueAsync();
            return ServiceResult<decimal>.Successful(totalRevenue);
        }

        public async Task<ServiceResult<int>> CreateOrderAsync(CreateOrderDto dto, string currentUserId)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId);
            if (customer == null)
                return ServiceResult<int>.Failed("Customer not found.");

            if (dto.Items == null || !dto.Items.Any())
                return ServiceResult<int>.Failed("Order must contain at least one item.");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    CustomerId = dto.CustomerId,
                    CreatedByUserId = dto.CreatedByUserId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Completed,
                    TotalPrice = 0
                };

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                decimal totalPrice = 0;

                foreach (var item in dto.Items)
                {
                    var product = await _unitOfWork.Products.GetForOrderAsync(item.ProductId);
                    if (product == null)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ServiceResult<int>.Failed($"Product with ID {item.ProductId} not found.");
                    }

                    if (product.QuantityInStock < item.Quantity)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ServiceResult<int>.Failed($"Insufficient stock for product: {product.Name}. Available: {product.QuantityInStock}");
                    }

                    var unitPrice = product.Price;
                    var subTotal = unitPrice * item.Quantity;

                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = unitPrice,
                        SubTotal = subTotal
                    };

                    await _unitOfWork.OrderItems.AddAsync(orderItem);

                    product.QuantityInStock -= item.Quantity;
                    _unitOfWork.Products.Update(product);

                    totalPrice += subTotal;
                }

                order.TotalPrice = totalPrice;
                _unitOfWork.Orders.Update(order);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _activityLogService.LogAsync(
                    userId: currentUserId,
                    action: "Create",
                    entityName: SystemModules.Orders,
                    entityId: order.Id,
                    description: $"Placed a new order for Customer: '{customer.FullName}' with a total price of {order.TotalPrice} across {dto.Items.Count} unique items."
                );

                return ServiceResult<int>.Successful(order.Id, "Order created successfully.");
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult<int>.Failed("An unexpected technical error occurred while processing the order.");
            }
        }
    }
}