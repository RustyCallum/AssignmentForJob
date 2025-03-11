using ForJob.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForJob.Controllers.Tasks.GetById
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class TaskGetById : ControllerBase
    {
        private readonly DatabaseContext _context;

        public TaskGetById(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            //I decided to make only GetById task request ignore the query filter for not taking soft deleted tasks
            var task = await _context.Tasks.IgnoreQueryFilters().Where(x => x.Id == id).FirstOrDefaultAsync();

            if (task == null)
            {
                return BadRequest($"There is no task matching id {id}");
            }

            return Ok(task);
        }
    }
}
