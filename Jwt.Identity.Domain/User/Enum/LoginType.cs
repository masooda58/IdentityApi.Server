using System.Text.Json.Serialization;

namespace Jwt.Identity.Domain.User.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LoginType
    {
        Token,
        Cookie,
        TokenAndCookie
    }
}