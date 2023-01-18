using System;

namespace Jwt.Identity.Domain.Token.Entitis
{
    public class TokenModel
    {
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}