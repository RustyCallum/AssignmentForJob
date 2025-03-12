using ForJob.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForJob.Controllers.Tasks.Delete
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class TaskDelete : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<TaskDelete> _logger;

        public TaskDelete(DatabaseContext context, ILogger<TaskDelete> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var task = await _context.Tasks.Where(x => x.Id == id).FirstOrDefaultAsync();

            if(task == null)
            {
                _logger.LogInformation($"Task with id {id} could not be found");
                return NotFound();
            }

            task.IsDeleted = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Task with id {id} marked as deleted");

            return NoContent();
        }
    }
}
