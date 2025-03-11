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

        public TaskPost(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Post(TaskPostRequest req)
        {
            if (req == null)
            {
                return BadRequest("No body provided");
            }
            if (req.DueDate == null)
            {
                return BadRequest("No due date provided");
            }
            if (req.Description == null)
            {
                return BadRequest("Description not provided");
            }
            if (req.Title == null)
            {
                return BadRequest("Title not provided");
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

            return Ok(newTask);
        }
    }
}
