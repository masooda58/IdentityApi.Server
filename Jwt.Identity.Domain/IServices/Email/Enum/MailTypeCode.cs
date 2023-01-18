using System.Text.Json.Serialization;

namespace Jwt.Identity.Domain.IServices.Email.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MailTypeCode
    {
        MailAccountConfirmationCode,
        MailAccountPasswordResetCode
    }
}