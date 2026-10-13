using ERPLite.Services.DTOs.HR;
using FluentValidation;

namespace ERPLite.Services.Validators.HR
{
    public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDto>
    {
        public CreateDepartmentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department name is required.")
                .Length(2, 100).WithMessage("Department name must be between 2 and 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        }
    }
}
