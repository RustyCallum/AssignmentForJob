using ForJob.DbContext;
using ForJob.Library;
using ForJob.Services.EmailService;
using Microsoft.EntityFrameworkCore;

namespace ForJob.Services.TaskReminderService
{
    public class TaskReminderService
    {
        private readonly IEmailService _emailService;
        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TaskReminderService> _logger;

        public TaskReminderService(IEmailService emailService, IHttpClientFactory httpClientFactory, DatabaseContext context, IConfiguration configuration, ILogger<TaskReminderService> logger)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        public async System.Threading.Tasks.Task CheckTasksAndSendReminders()
        {
            var now = DateTime.UtcNow;
            var tasks = await _context.Tasks
                .Where(t => t.DueDate <= now.AddHours(6) && t.DueDate >= now)
                .ToListAsync();

            string recipientEmail = _configuration["RecipentEmail"];

            if (recipientEmail == null)
            {
                throw new Exception("Recipent email not provided");
            }

            foreach (var task in tasks)
            {
                string emailBody = $"Reminder: Task '{task.Title}' is due to {task.DueDate}!";
                _logger.LogInformation($"Sending reminder for task: {task.Title}");

                // ENTER YOUR EMAIL HERE
                await _emailService.SendEmailAsync(recipientEmail, "Task Reminder", emailBody);
            }
        }
    }
}
