using ForJob.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForJob.Controllers.Tasks.Get
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class TaskGet : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<TaskGet> _logger;

        public TaskGet(DatabaseContext context, ILogger<TaskGet> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Domain.Task>>> GetTasks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            if (page < 1 || pageSize < 1)
            {
                _logger.LogInformation($"Provided page or pagesize lower than 1");
                return BadRequest("page and pageSize must be greater than 0");
            }

            var query = _context.Tasks.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.Title.Contains(search));
            }

            if (from.HasValue)
            {
                query = query.Where(t => t.DueDate >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(t => t.DueDate <= to.Value);
            }

            var totalItems = await query.CountAsync();
            var tasks = await query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Data = tasks
            });
        }
    }
}
