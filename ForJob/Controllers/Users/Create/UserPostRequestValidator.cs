using FluentValidation;
using ForJob.DbContext;

namespace ForJob.Controllers.Users.Create
{
    public class UserPostRequestValidator : AbstractValidator<UserCreateRequest>
    {
        public UserPostRequestValidator(DatabaseContext context)
        {
            RuleFor(x => x)
                .NotNull()
                .NotEmpty()
                .WithMessage("Request cannot be empty.");

            RuleFor(x => x.Name)
                .NotNull().WithMessage("Name is required.")
                .NotEmpty().WithMessage("Name cannot be empty.")
                .Must(name => !context.Users.Any(u => u.Name == name)).WithMessage("User with this name already exists.");

            RuleFor(x => x.Password)
                .NotNull().WithMessage("Password is required.")
                .NotEmpty().WithMessage("Password cannot be empty.");
        }
    }
}
