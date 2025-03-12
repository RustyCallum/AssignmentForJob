using FluentValidation;
using ForJob.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForJob.Controllers.Tasks.Post
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class TaskPost : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IValidator<TaskPostRequest> _taskValidator;

        public TaskPost(DatabaseContext context, IValidator<TaskPostRequest> taskValidator)
        {
            _context = context;
            _taskValidator = taskValidator;
        }

        [HttpPost]
        public async Task<ActionResult> Post(TaskPostRequest req)
        {
            var validationResult = await _taskValidator.ValidateAsync(req);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var newTask = new Library.Task
            {
                DueDate = req.DueDate.Value,
                Description = req.Description,
                Title = req.Title,
                IsCompleted = false
            };

            _context.Tasks.Add(newTask);
            await _context.SaveChangesAsync();

            return Created();
        }
    }
}
