using System;
using Jwt.Identity.Domain.Token.Entitis;

namespace Jwt.Identity.Domain.Token.Dto
{
    public class UserTokenResponse : TokenModel
    {
        public string RefreshToken { get; set; }

        public DateTime RefreshExpiration { get; set; }
    }
}