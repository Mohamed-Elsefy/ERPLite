using AutoMapper;
using ERPLite.Data.Entities.Sales;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.Common;
using ERPLite.Services.DTOs.Sales;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.Sales;
using ERPLite.Services.Interfaces.System;
using ERPLite.Services.Interfaces.Infrastructure;
using ERPLite.Shared.Constants;
using ERPLite.Shared.Enums;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ERPLite.Services.Services.Sales
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IActivityLogService _activityLogService;
        private readonly INotificationService _notificationService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IValidator<CreatePaymentDto> _createValidator;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IActivityLogService activityLogService,
            INotificationService notificationService,
            IDateTimeProvider dateTimeProvider,
            IValidator<CreatePaymentDto> createValidator,
            ILogger<PaymentService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _activityLogService = activityLogService;
            _notificationService = notificationService;
            _dateTimeProvider = dateTimeProvider;
            _createValidator = createValidator;
            _logger = logger;
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
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return ServiceResult<int>.Failed(validationResult.ToErrorString());

            var order = await _unitOfWork.Orders.GetByIdAsync(dto.OrderId);
            if (order == null)
                return ServiceResult<int>.Failed("Order not found.");

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
                    PaymentDate = _dateTimeProvider.UtcNow,
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

                await _unitOfWork.CommitTransactionAsync();

                await _notificationService.CreateSystemNotificationAsync(
                    userId: currentUserId,
                    title: "Financial Sinking Fund Credit",
                    message: $"Remittance allocation of {dto.Amount} committed for Order #{order.Id} via {dto.PaymentMethod}.",
                    type: NotificationTypes.Finance,
                    priority: NotificationPriorities.High
                );

                await _activityLogService.LogAsync(
                    userId: currentUserId,
                    action: AuditActions.Create,
                    entityName: SystemModules.Payments,
                    entityId: payment.Id,
                    description: $"Recorded payment of {payment.Amount} via {payment.PaymentMethod} for Order #{order.Id}. New order financial status: {order.PaymentStatus}."
                );

                _logger.LogInformation("Recorded payment {PaymentId} of {Amount} for order {OrderId} via {Method}",
                    payment.Id, dto.Amount, dto.OrderId, dto.PaymentMethod);
                return ServiceResult<int>.Successful(payment.Id, "Payment recorded and order financial status updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Failed to record payment of {Amount} for order {OrderId}", dto.Amount, dto.OrderId);
                return ServiceResult<int>.Failed("An unexpected technical error occurred while recording the payment.");
            }
        }

        public async Task<ServiceResult<PagedResponseDto<PaymentDto>>> GetPagedAsync(PagedRequestDto request)
        {
            var paged = await _unitOfWork.Payments.GetPagedRecentAsync(
                request.PageNumber,
                request.PageSize);

            return ServiceResult<PagedResponseDto<PaymentDto>>.Successful(
                paged.ToPagedResponse<Payment, PaymentDto>(_mapper));
        }

        public async Task<ServiceResult<IEnumerable<PaymentDto>>> GetRecentPaymentsAsync(int count)
        {
            try
            {
                var payments = await _unitOfWork.Payments.GetRecentPaymentsAsync(count);
                var dto = _mapper.Map<IEnumerable<PaymentDto>>(payments);

                return ServiceResult<IEnumerable<PaymentDto>>.Successful(dto, "Recent payments retrieved successfully.");
            }
            catch (Exception)
            {
                return ServiceResult<IEnumerable<PaymentDto>>.Failed("An unexpected error occurred while retrieving the overall financial collections record.");
            }
        }

        public async Task<ServiceResult<decimal>> GetRemainingBalanceAsync(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                    return ServiceResult<decimal>.Failed("Order context not found.");

                var totalPaid = await _unitOfWork.Payments.GetTotalPaidAmountAsync(orderId);
                var remaining = order.TotalPrice - totalPaid;

                return ServiceResult<decimal>.Successful(remaining < 0 ? 0 : remaining);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate remaining balance for order {OrderId}", orderId);
                return ServiceResult<decimal>.Failed("Failed to calculate balance.");
            }
        }
    }
}