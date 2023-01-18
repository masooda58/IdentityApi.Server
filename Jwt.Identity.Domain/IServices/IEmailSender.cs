using System.Threading.Tasks;

namespace Jwt.Identity.Domain.IServices
{
    public interface IEmailSender
    {
        public Task SendEmailAsync(string toEmail, string subject, string message, bool isMessageHtml = false);
    }
}