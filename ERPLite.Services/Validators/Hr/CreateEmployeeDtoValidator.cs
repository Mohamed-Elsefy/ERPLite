using ERPLite.Services.DTOs.HR;
using FluentValidation;

namespace ERPLite.Services.Validators.Hr
{
    public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Salary)
                .GreaterThan(0);

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0);
        }
    }
}
