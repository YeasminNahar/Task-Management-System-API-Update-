using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Task_Management_System_API.Models;
namespace Task_Management_System_API.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }
        public async System.Threading.Tasks.Task SendEmailAsync(string to, string subject, string body)
        {
            using (var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
            {

                smtpClient.Credentials = new NetworkCredential("taskmanagementproject61@gmail.com", _smtpSettings.Password);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("taskmanagementproject61@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}
