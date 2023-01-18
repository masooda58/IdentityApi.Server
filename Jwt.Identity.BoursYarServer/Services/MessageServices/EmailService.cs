using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Jwt.Identity.Domain.IServices;

namespace Jwt.Identity.BoursYarServer.Services.MessageServices
{
    public class EmailService : IEmailSender
    {
        public Task SendEmailAsync(string toEmail, string subject, string message, bool isMessageHtml = false)
        {
            using (var client = new SmtpClient())
            {
                // قرار بدهم DataBase یا در Appsetting را بعد تصمیم می گیرم در user,Password اطلاعات
                var credentials = new NetworkCredential
                {
                    UserName = "masoud.emulator.test", // without @gmail.com
                    Password = "felaaktgsrrfjnfp"
                };

                client.Credentials = credentials;
                client.Host = "127.0.0.1";
                client.Port = 25;
                client.EnableSsl = false;
                client.UseDefaultCredentials = false;

                using var emailMessage = new MailMessage
                {
                    To = { new MailAddress(toEmail) },
                    From = new MailAddress("masoud.emulator.test @gmail.com"), // with @gmail.com
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = isMessageHtml
                };


                client.Send(emailMessage);
            }

            return Task.CompletedTask;
        }
    }
}