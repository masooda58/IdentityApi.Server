using System.Text.Json.Serialization;

namespace Jwt.Identity.Domain.IServices.Totp.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TotpTypeCode
    {
        TotpAccountConfirmationCode,
        TotpAccountPasswordResetCode
    }
}