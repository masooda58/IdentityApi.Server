using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoursYar.Authorization.repositories;
using Jwt.Identity.Test.Helper;
using Xunit;

#pragma warning disable CS1591

namespace Jwt.Identity.Test
{
    public class TokenGenratorsAndValidationTest

    {
        [Fact]
        public async Task TokenGenrators_CreatToken_returnJwtToken()
        {
            var tokenGenrators = ApiHelper.CreaTokenGenrators();
            var tokentValidators = ApiHelper.CreaTokenValidators();
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, "user.UserName"),
                new(ClaimStore.BoursYarAccess, "x"),
                new(ClaimStore.BoursYarAccess, "y"),
                new("id", "user.Id")
            };
            //Act
            //var genrateResult = tokenGenrators.GetAccessToken(authClaims);
            //var validResult = tokentValidators.Validate(genrateResult.RefreshToken);
            //Assert
           // Assert.True(validResult);
        }
    }
}