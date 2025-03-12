using FluentValidation;
using ForJob.Controllers.Users.Create;

namespace ForJob.Controllers.Users.Login
{
    public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .NotEmpty()
                .WithMessage("Request cannot be empty.");

            RuleFor(x => x.Name)
                .NotNull().WithMessage("Name is required.")
                .NotEmpty().WithMessage("Name cannot be empty.");

            RuleFor(x => x.Password)
                .NotNull().WithMessage("Password is required.")
                .NotEmpty().WithMessage("Password cannot be empty.");
        }
    }
}
