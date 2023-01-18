using System;
using System.Threading.Tasks;
using Jwt.Identity.Data.Context;
using Jwt.Identity.Domain.Token.Data;

namespace Jwt.Identity.Data.Repositories.UserRepositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IdentityContext _context;

        public RefreshTokenRepository(IdentityContext context)
        {
            _context = context;
        }

        public async Task<bool> WritRefreshTokenAsync(string userId, string refreshToken)
        {
            try
            {
                //if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(refreshToken))
                //{
                //    return false;
                //}
                //var result = await _context.AddAsync(new RefreshToken()

                //{
                //    UserId = userId,
                //    TempRefreshToken = refreshToken,
                //    CreatTime = DateTime.Now


                //});
                //await _context.SaveChangesAsync();
                // return result != null;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task<string> GetUserIdByRefreshTokenAsync(string refreshToken)
        {
            //var result = string.IsNullOrEmpty(refreshToken) ? null : await _context.RefreshTokens.FirstOrDefaultAsync
            //    (r => r.TempRefreshToken == refreshToken);
            //return result?.UserId;
            return "ok";
        }

        public async Task<bool> DeleteRefreshTokenByuserIdAsync(string userId)
        {
            //try
            //{
            //    if (string.IsNullOrEmpty(userId))
            //    {
            //        return false;
            //    }
            //    _context.RemoveRange(_context.RefreshTokens.Where(u => u.UserId == userId));
            //    var result = await _context.SaveChangesAsync();
            //    return true;
            //}
            //catch (Exception e)
            //{
            //    return false;
            //}
            return true;
        }
    }
}