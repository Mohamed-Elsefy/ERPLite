using ERPLite.Services.DTOs.HR;
using FluentValidation;

namespace ERPLite.Services.Validators.HR
{
    public class CheckOutValidator : AbstractValidator<CheckOutDto>
    {
        public CheckOutValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("Employee is required.");
        }
    }
}
