using System.Net.Mail;
using System.Threading.Tasks;

namespace YadiYad.Pro.Services.Services.Payout
{
    public class EmailNotificationService : INotificationService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;

        public EmailNotificationService(string smtpServer, int smtpPort, string smtpUser, string smtpPassword)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUser = smtpUser;
            _smtpPassword = smtpPassword;
        }

        public async Task SendNotificationAsync(string recipient, string subject, string message)
        {
            using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
            {
                smtpClient.Credentials = new System.Net.NetworkCredential(_smtpUser, _smtpPassword);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("noreply@yourdomain.com"),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = false
                };

                mailMessage.To.Add(recipient);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}
