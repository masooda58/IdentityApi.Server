using System.Text.Json.Serialization;

namespace Jwt.Identity.Domain.User.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SignInLogerType
    {
        LogIn,
        LogOut
    }
}