using System.Threading.Tasks;
using Jwt.Identity.Domain.IServices.Email.Enum;
using Jwt.Identity.Domain.User.Entities;
using Jwt.Identity.Framework.Response;

namespace Jwt.Identity.Domain.IServices.Email
{
    public interface IMailCode
    {
        public Task<ResultResponse> SendMailCodeAsync(ApplicationUser user, MailTypeCode type,
            string callbackUri = null);

        // public Task<ResultResponse> ConfirmMailCodeAsync(string email, string code, MailTypeCode type);
    }
}