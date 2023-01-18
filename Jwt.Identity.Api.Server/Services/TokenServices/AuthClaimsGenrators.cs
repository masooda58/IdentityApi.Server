using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Jwt.Identity.Domain.Token.ITokenServices;
using Jwt.Identity.Domain.User.Entities;

namespace Jwt.Identity.Api.Server.Services.TokenServices
{
    // این کلاس یک پیاده سازی موقت است تا تعیین نحوه اصلی دسترسی در توکن
    // این کلاس تعیین می کنن چه مواردی در توکن اصلی قرار بگیرد
    public class AuthClaimsGenrators : IAuthClaimsGenrators
    {
        /// <inheritdoc />
        public List<Claim> CreatClaims(ApplicationUser user)
        {
         
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                //new Claim("BoursYarAccess","x"),
                //new Claim("BoursYarAccess","y"),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Role,"masood"),
                new(ClaimTypes.Role,"masood2"),
            };
          
            return authClaims;
        }
    }
}