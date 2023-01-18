using System.Text.Json.Serialization;

namespace Jwt.Identity.Domain.UseLoginPolicy.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MultiUserNumberPolicy
    {
        KeepLastLogin,
        SignOutLastLogin
    }
}
