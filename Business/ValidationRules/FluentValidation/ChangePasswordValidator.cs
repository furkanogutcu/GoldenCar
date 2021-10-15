using Entities.Models;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordModel>
    {
        public ChangePasswordValidator()
        {
            RuleFor(u => u.Email).NotEmpty();
            RuleFor(u => u.Email).NotNull();
            RuleFor(u => u.Email).EmailAddress();

            RuleFor(u => u.OldPassword).NotNull();
            RuleFor(u => u.OldPassword).NotEmpty();
            RuleFor(u => u.OldPassword).MinimumLength(6);
            RuleFor(u => u.OldPassword).MaximumLength(25);

            RuleFor(u => u.NewPassword).NotNull();
            RuleFor(u => u.NewPassword).NotEmpty();
            RuleFor(u => u.NewPassword).MinimumLength(6);
            RuleFor(u => u.NewPassword).MaximumLength(25);
        }
    }
}
