using System.Threading.Tasks;
using Jwt.Identity.Domain.IServices.Totp.Enum;
using Jwt.Identity.Framework.Response;

namespace Jwt.Identity.Domain.IServices.Totp
{
    public interface ITotpCode
    {
        public Task<ResultResponse> SendTotpCodeAsync(string phoneNo, TotpTypeCode type);

        public Task<ResultResponse> ConfirmTotpCodeAsync(string phoneNo, string code, TotpTypeCode type);
    }
}