using System.Threading.Tasks;
using Jwt.Identity.Test.Helper;
using Xunit;

namespace Jwt.Identity.Test
{
    public class RefreshTokenREpositoryTest
    {
        [Theory]
        [InlineData("userId1", "refreshtoke1", true)]
        [InlineData("", "refreshtoke1", false)]
        [InlineData("userId1", "", false)]
        public async Task RefreshTokenRepository_CheckAllFunction(string userId, string refreshToken, bool expected)
        {
            using var context = DataServiceHelpers.CreateDbContext();
            var refreshTokenRepository = DataServiceHelpers.CreatRefreshTokenRepository(context);


            //Act
            var resulWrite = await refreshTokenRepository.WritRefreshTokenAsync(userId, refreshToken);
            var resultuserId = await refreshTokenRepository.GetUserIdByRefreshTokenAsync(refreshToken);
            var resultDelete = await refreshTokenRepository.DeleteRefreshTokenByuserIdAsync(userId);
            //Assert`
            var result = resulWrite && resultuserId == userId && resultDelete;
            Assert.Equal(expected, result);
        }
    }
}