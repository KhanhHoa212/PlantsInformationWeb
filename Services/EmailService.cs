using System.Net;
using System.Net.Mail;

namespace PlantsInformationWeb.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("khanhhoakt2k4@gmail.com", "pzmmpojtkotakyze"), // App Password
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("khanhhoakt2k4@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };
            mailMessage.To.Add(toEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi gửi email: " + ex.Message);
                throw;
            }
        }
    }
}
