using System.Threading.Tasks;
using Jwt.Identity.Domain.IServices;

namespace Jwt.Identity.BoursYarServer.Services.MessageServices
{
    public class SmsServices : ISmsSender
    {
        /// <inheritdoc />
        /// >
        public async Task<bool> SendSmsAsync(string mobileNo, string smsBody, string providerName)
        {
            return true;
        }
    }
}