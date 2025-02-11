namespace Task_Management_System_API.Services
{
   
        public interface IEmailService
        {
            Task SendEmailAsync(string to, string subject, string body);
        }
    }

