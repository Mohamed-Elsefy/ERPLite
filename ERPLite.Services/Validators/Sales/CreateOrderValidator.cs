using ERPLite.Services.DTOs.Sales;
using FluentValidation;

namespace ERPLite.Services.Validators.Sales
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderValidator(IValidator<CreateOrderItemDto> itemValidator)
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Customer is required.");

            RuleFor(x => x.CreatedByUserId)
                .NotEmpty().WithMessage("Order creator is required.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must contain at least one item.");

            RuleForEach(x => x.Items).SetValidator(itemValidator);
        }
    }
}
