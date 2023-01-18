using System.Threading.Tasks;

namespace Jwt.Identity.Domain.IServices
{
    public interface ISmsSender
    {
        /// <summary>
        ///     ارسال پیامک
        /// </summary>
        /// <param name="mobileNo">شماره موبایل</param>
        /// <param name="smsBody">متن پیامک</param>
        /// <param name="providerName">نام سرویس دهنده پیامک</param>
        /// <returns></returns>
        public Task<bool> SendSmsAsync(string mobileNo, string smsBody, string providerName);
    }
}