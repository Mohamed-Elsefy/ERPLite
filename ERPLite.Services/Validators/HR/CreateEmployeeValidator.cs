using ERPLite.Services.DTOs.HR;
using FluentValidation;

namespace ERPLite.Services.Validators.HR
{
    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Employee name is required.")
                .Length(2, 150).WithMessage("Employee name must be between 2 and 150 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is invalid.")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

            RuleFor(x => x.Salary)
                .GreaterThanOrEqualTo(0).WithMessage("Salary cannot be negative.");

            RuleFor(x => x.HireDate)
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Hire date cannot be in the future.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Department is required.");
        }
    }
}
