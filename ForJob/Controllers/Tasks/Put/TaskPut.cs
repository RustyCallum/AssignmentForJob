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
        public DatabaseContext _context;

        public TaskPut(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, TaskPutRequest req)
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
