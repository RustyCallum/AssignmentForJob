using FluentValidation;
using ForJob.Controllers.Tasks.Post;
using ForJob.DbContext;

namespace ForJob.Controllers.Tasks.Put
{
    public class TaskPutRequestValidator : AbstractValidator<TaskPutRequest>
    {
        public TaskPutRequestValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .NotEmpty()
                .WithMessage("Request cannot be empty.");

            RuleFor(x => x.Title)
                .NotNull().WithMessage("Title is required.")
                .NotEmpty().WithMessage("Title cannot be empty.");

            RuleFor(x => x.Description)
                .NotNull().WithMessage("Description is required.")
                .NotEmpty().WithMessage("Description cannot be empty.");

            RuleFor(x => x.DueDate)
                .NotNull().WithMessage("DueDate is required.");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.Now).WithMessage("Due date must be in the future.");
        }
    }
}
