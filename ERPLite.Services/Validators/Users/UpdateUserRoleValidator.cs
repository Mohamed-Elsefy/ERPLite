using ERPLite.Services.DTOs.Users;
using FluentValidation;

namespace ERPLite.Services.Validators.Users
{
    public class UpdateUserRoleValidator : AbstractValidator<UpdateUserRoleDto>
    {
        public UpdateUserRoleValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User id is required.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Must(ValidationRules.BeValidRole).WithMessage(ValidationRules.InvalidRoleMessage);
        }
    }
}
