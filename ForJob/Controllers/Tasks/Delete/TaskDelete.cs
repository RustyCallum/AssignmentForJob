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

        public TaskDelete(DatabaseContext context)
        {
            _context = context;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var task = await _context.Tasks.Where(x => x.Id == id).FirstOrDefaultAsync();

            if(task == null)
            {
                return NotFound();
            }

            task.IsDeleted = true;
            await _context.SaveChangesAsync();

            return Ok($"Task {id} marked as deleted sucessfully");
        }
    }
}
