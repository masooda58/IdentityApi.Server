using System.Collections.Generic;
using System.Security.Claims;
using Jwt.Identity.Domain.User.Entities;

namespace Jwt.Identity.Domain.Token.ITokenServices
{
    public interface IAuthClaimsGenrators
    {
        // های که در یوزر هست رابصورت یک لیست تهیه می کند Claim
        List<Claim> CreatClaims(ApplicationUser user);
    }
}