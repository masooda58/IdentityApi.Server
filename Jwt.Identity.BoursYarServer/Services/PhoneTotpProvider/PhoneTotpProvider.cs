using System.Security.Cryptography;
using Jwt.Identity.BoursYarServer.Models.SettingModels;
using Jwt.Identity.Domain.IServices.Totp;
using Jwt.Identity.Framework.Response;
using Microsoft.Extensions.Options;
using OtpNet;

namespace Jwt.Identity.BoursYarServer.Services.PhoneTotpProvider
{
    public class PhoneTotpProvider : IPhoneTotpProvider
    {
        private readonly TotpSettings _options;
        private Totp _totp;

        public PhoneTotpProvider(IOptions<TotpSettings> options)
        {
            _options = options?.Value ?? new TotpSettings();
        }

        /// <inheritdoc />
        public string GenerateTotp(byte[] secretKey)
        {
            CreateTotp(secretKey);
            return _totp.ComputeTotp();
        }

        /// <inheritdoc />
        public ResultResponse VerifyTotp(byte[] secretKey, string code)
        {
            CreateTotp(secretKey);
            var isTotpValid = _totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
            return isTotpValid
                ? new ResultResponse(true, null)
                : new ResultResponse(false, "کد وراد شده معتبر نیست لطفا کد جدید دریافت نمایید");
        }

        /// <inheritdoc />
        public byte[] CreateSecretKey()
        {
            using var rng = new RNGCryptoServiceProvider();
            var secretKey = new byte[32];
            rng.GetBytes(secretKey);
            return secretKey;
        }

        private void CreateTotp(byte[] secretKey)
        {
            _totp = new Totp(secretKey, _options.Step);
        }
    }
}