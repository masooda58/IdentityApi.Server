using System.Threading.Tasks;

namespace Jwt.Identity.Domain.Token.Data
{
    public interface IRefreshTokenRepository
    {
        Task<bool> WritRefreshTokenAsync(string userId, string refreshToken);
        Task<string> GetUserIdByRefreshTokenAsync(string refreshToken);
        Task<bool> DeleteRefreshTokenByuserIdAsync(string userId);
    }
}