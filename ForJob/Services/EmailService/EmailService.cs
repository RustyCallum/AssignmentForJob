using MailKit.Security;
using MimeKit;

namespace ForJob.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("KidPlayground", _configuration["Smtp:Username"]));
                email.To.Add(new MailboxAddress("", to));
                email.Subject = subject;
                email.Body = new TextPart("html") { Text = body };

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(
                    _configuration["Smtp:Host"],
                    int.Parse(_configuration["Smtp:Port"]),
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"E-mail sent to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while sending e-mail: {ex.Message}");
            }
        }
    }
}
