using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using WebCoffee.ViewModel;

namespace WebCoffee.Repository
{
    public interface IEmailRepository
    {
        bool SendEmail(EmailModel email);
        bool SendPasswordToEmail(string email);
    }
    public class EmailRepository : IEmailRepository
    {
        private readonly IConfiguration _configuration;

        public EmailRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SendEmail(EmailModel email)
        {
            try
            {
                var smtpClient = new SmtpClient
                {
                    Host = _configuration["Gmail:Host"],
                    Port = int.Parse(_configuration["Gmail:Port"]),
                    EnableSsl = bool.Parse(_configuration["Gmail:SMTP:starttls:enable"]),
                    Credentials = new NetworkCredential(_configuration["Gmail:Username"], _configuration["Gmail:Password"])
                };

                var mailMessage = new MailMessage(_configuration["Gmail:Username"], email.ToEmail)
                {
                    Subject = email.Subject,
                    Body = email.Body,
                    IsBodyHtml = true
                };

                smtpClient.Send(mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SendPasswordToEmail(string email)
        {
            string newPassword = GenerateRandomPassword(); // Hàm tự tạo mật khẩu ngẫu nhiên
            var emailModel = new EmailModel
            {
                ToEmail = email,
                Subject = "Mật khẩu mới của bạn",
                Body = $"Mật khẩu mới của bạn là: {newPassword}"
            };

            return SendEmail(emailModel); // Gửi email với mật khẩu ngẫu nhiên
        }

        private string GenerateRandomPassword()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 8) // Độ dài mật khẩu
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

}
