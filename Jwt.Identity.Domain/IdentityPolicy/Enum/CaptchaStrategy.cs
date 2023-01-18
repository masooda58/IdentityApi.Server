using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Jwt.Identity.Domain.IdentityPolicy.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CaptchStrategy
    {
        [Description("بعد از قفل اکانت")]
        AfterLock,
        [Description("همیشه")]
        Always,
        [Description("هرگز")]
        Never
    }
}
