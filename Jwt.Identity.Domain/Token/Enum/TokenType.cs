using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Jwt.Identity.Domain.Token.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TokenType
    {
      AccessToken,
      RefreshToken
    }
}
