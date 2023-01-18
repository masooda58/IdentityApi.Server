using System.Collections.Generic;
using System.Security.Claims;
using Jwt.Identity.Domain.Token.Dto;
using Jwt.Identity.Domain.Token.Entitis;

namespace Jwt.Identity.Domain.Token.ITokenServices
{
    public interface ITokenGenrators
    {
        public UserTokenResponse GetAccessToken(List<Claim> authClaims);
        //public TokenModel GetRefreshToken();
    }
}