using ERPLite.Services.DTOs.Sales;
using FluentValidation;

namespace ERPLite.Services.Validators.Sales
{
    public class CreateCustomerValidator : AbstractValidator<CreateCustomerDto>
    {
        public CreateCustomerValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Customer name is required.")
                .Length(2, 150).WithMessage("Customer name must be between 2 and 150 characters.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.");
        }
    }
}
