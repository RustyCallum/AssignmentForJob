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

        public TaskReminderService(IEmailService emailService, IHttpClientFactory httpClientFactory, DatabaseContext context)
        {
            _context = context;
            _emailService = emailService;
        }

        public async System.Threading.Tasks.Task CheckTasksAndSendReminders()
        {
            var now = DateTime.UtcNow;
            var tasks = await _context.Tasks
                .Where(t => t.DueDate <= now.AddHours(6) && t.DueDate >= now)
                .ToListAsync();

            foreach (var task in tasks)
            {
                string emailBody = $"Reminder: Task '{task.Title}' is due in {task.DueDate}!";
                Console.WriteLine($"Sending reminder for task: {task.Title}");

                // ENTER YOUR EMAIL HERE
                await _emailService.SendEmailAsync("YOUREMAILHERE", "Task Reminder", emailBody);
            }
        }
    }
}
