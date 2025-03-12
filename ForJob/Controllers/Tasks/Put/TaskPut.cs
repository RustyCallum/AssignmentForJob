using FluentValidation;
using ForJob.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForJob.Controllers.Tasks.Put
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class TaskPut : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IValidator<TaskPutRequest> _taskValidator;


        public TaskPut(DatabaseContext context, IValidator<TaskPutRequest> taskValidator)
        {
            _context = context;
            _taskValidator = taskValidator;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, TaskPutRequest req)
        {
            var validationResult = await _taskValidator.ValidateAsync(req);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var task = _context.Tasks.Where(x => x.Id == id).FirstOrDefault();

            if(task == null)
            {
                return NotFound();
            }

            task.Title = req.Title;
            task.Description = req.Description;
            task.DueDate = req.DueDate.Value;
            task.IsCompleted = req.IsCompleted;

            await _context.SaveChangesAsync();

            return Ok($"Task {id} updated sucessfully");
        }
    }
}
