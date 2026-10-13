using ERPLite.Services.DTOs.Sales;
using FluentValidation;

namespace ERPLite.Services.Validators.Sales
{
    public class CreatePaymentValidator : AbstractValidator<CreatePaymentDto>
    {
        public CreatePaymentValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Order is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("Invalid payment method.");
        }
    }
}
