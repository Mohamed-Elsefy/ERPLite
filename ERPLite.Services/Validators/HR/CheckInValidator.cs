using ERPLite.Services.DTOs.HR;
using FluentValidation;

namespace ERPLite.Services.Validators.HR
{
    public class CheckInValidator : AbstractValidator<CheckInDto>
    {
        public CheckInValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("Employee is required.");
        }
    }
}
