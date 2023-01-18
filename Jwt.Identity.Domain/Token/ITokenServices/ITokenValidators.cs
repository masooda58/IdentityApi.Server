using System.Security.Claims;
using Jwt.Identity.Domain.Clients.Query;
using Jwt.Identity.Domain.Token.Enum;
using Microsoft.IdentityModel.Tokens;

namespace Jwt.Identity.Domain.Token.ITokenServices
{
    public interface ITokenValidators
    {
        public bool Validate(string token,TokenType type=TokenType.RefreshToken);
        public ClaimsPrincipal GetClaimPrincipalValidatedToken(string token,TokenType type=TokenType.AccessToken);

    }
}