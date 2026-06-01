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
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IActivityLogService _activityLogService;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IActivityLogService activityLogService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _activityLogService = activityLogService;
        }

        public async Task<ServiceResult<IEnumerable<PaymentDto>>> GetPaymentsByOrderAsync(int orderId)
        {
            var orderExists = await _unitOfWork.Orders.GetByIdAsync(orderId) != null;
            if (!orderExists)
                return ServiceResult<IEnumerable<PaymentDto>>.Failed("Order not found.");

            var payments = await _unitOfWork.Payments.GetPaymentsByOrderAsync(orderId);
            var dto = _mapper.Map<IEnumerable<PaymentDto>>(payments);

            return ServiceResult<IEnumerable<PaymentDto>>.Successful(dto);
        }

        public async Task<ServiceResult<OrderFinancialDto>> GetOrderFinancialSummaryAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                return ServiceResult<OrderFinancialDto>.Failed("Order not found.");

            var alreadyPaid = await _unitOfWork.Payments.GetTotalPaidAmountAsync(orderId);

            var summary = new OrderFinancialDto
            {
                OrderId = order.Id,
                TotalPrice = order.TotalPrice,
                PaidAmount = alreadyPaid,
                RemainingAmount = order.TotalPrice - alreadyPaid,
                PaymentStatus = order.PaymentStatus
            };

            return ServiceResult<OrderFinancialDto>.Successful(summary);
        }

        public async Task<ServiceResult<int>> CreatePaymentAsync(CreatePaymentDto dto, string currentUserId)
        {
            if (dto.Amount <= 0)
                return ServiceResult<int>.Failed("Payment amount must be greater than zero.");

            var order = await _unitOfWork.Orders.GetByIdAsync(dto.OrderId);
            if (order == null)
                return ServiceResult<int>.Failed("Order not found.");

            if (!Enum.IsDefined(typeof(PaymentMethod), dto.PaymentMethod))
                return ServiceResult<int>.Failed("Invalid payment method.");

            var alreadyPaid = await _unitOfWork.Payments.GetTotalPaidAmountAsync(dto.OrderId);
            var remainingBalance = order.TotalPrice - alreadyPaid;

            if (dto.Amount > remainingBalance)
                return ServiceResult<int>.Failed($"Payment exceeds remaining balance. Max allowed: {remainingBalance}");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var payment = new Payment
                {
                    OrderId = dto.OrderId,
                    Amount = dto.Amount,
                    PaymentMethod = dto.PaymentMethod,
                    PaymentDate = DateTime.UtcNow,
                    Status = PaymentStatus.Completed
                };

                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                var newTotalPaid = alreadyPaid + dto.Amount;
                var newRemainingBalance = order.TotalPrice - newTotalPaid;

                if (newRemainingBalance <= 0)
                {
                    order.PaymentStatus = OrderPaymentStatus.Paid;
                }
                else if (newTotalPaid > 0)
                {
                    order.PaymentStatus = OrderPaymentStatus.PartiallyPaid;
                }
                else
                {
                    order.PaymentStatus = OrderPaymentStatus.Unpaid;
                }

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                await _activityLogService.LogAsync(
                    userId: currentUserId,
                    action: "Create",
                    entityName: SystemModules.Payments,
                    entityId: payment.Id,
                    description: $"Recorded payment of {payment.Amount} via {payment.PaymentMethod} for Order #{order.Id}. New order financial status: {order.PaymentStatus}."
                );

                return ServiceResult<int>.Successful(payment.Id, "Payment recorded and order financial status updated successfully.");
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult<int>.Failed("An unexpected technical error occurred while recording the payment.");
            }
        }
    }
}