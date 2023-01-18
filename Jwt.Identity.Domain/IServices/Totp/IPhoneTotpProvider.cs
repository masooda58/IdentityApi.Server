using Jwt.Identity.Framework.Response;

namespace Jwt.Identity.Domain.IServices.Totp
{
    public interface IPhoneTotpProvider
    {
        /// <summary>
        ///     // Totp ساخت
        /// </summary>
        /// <param name="secretKey"></param>
        /// <returns> code Totp</returns>
        public string GenerateTotp(byte[] secretKey);

        /// <summary>
        ///     تایید کد دریافتی
        /// </summary>
        /// <param name="secretKey"></param>
        /// <param name="code"></param>
        /// <returns>  نتیجه تایید کد</returns>
        public ResultResponse VerifyTotp(byte[] secretKey, string code);

        /// <summary>
        ///     ساخت کلید رندم
        /// </summary>
        /// <returns></returns>
        public byte[] CreateSecretKey();
    }
}