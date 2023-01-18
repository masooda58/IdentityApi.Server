using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Data.Context;
using Jwt.Identity.Domain.UseLoginPolicy.Entities;
using Jwt.Identity.Domain.User.Data;
using Jwt.Identity.Domain.User.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jwt.Identity.Data.Repositories.UserRepositories
{
    public class UserPolicyManagementService:IUserPolicyManagementService
    {
        private readonly IdentityContext _context;

        public UserPolicyManagementService(IdentityContext context)
        {
            _context = context;
        }


        public async Task<bool> AddPolicyToUserAsync(ApplicationUserPolicy userPolicy)
        {
            await _context.ApplicationUserPolicies.AddAsync(userPolicy);
            return true;
        }

        public  bool ChangeUserPolicy(ApplicationUserPolicy userPolicy)
        {
            bool isDetached = _context.Entry(userPolicy).State == EntityState.Detached;
            if (isDetached)
                _context.Attach(userPolicy);
            _context.Entry(userPolicy).State = EntityState.Modified;
            return true;
        }

        public async Task<string> GetPolicyIdByUserIdAsync(string userId)
        {
            var policy = await _context.ApplicationUserPolicies.AsNoTracking().SingleOrDefaultAsync(c => c.UserId == userId);
            return policy?.PolicyId;
        }

        public async Task<UserLoginPolicyOptions> GetPolicyByUserIdAsync(string userId)
        {
            var policy = await _context.UserLoginPolicyOptions.AsNoTracking()
                .Include(
                    c => c.ApplicationUserPolicies
                        .Where(c => c.UserId == userId)).ToListAsync();
              
            if (policy != null) return policy[0];
            return null;
        }

        public async Task<List<ApplicationUser>> GetAllUserWithPolicyIdAsync(string policyId)
        {
            var user = await _context.Users.AsNoTracking().Include(o => o.ApplicationUserPolicy).Where(c=>c.ApplicationUserPolicy.PolicyId==policyId).ToListAsync();
            return user;
        }
    }
}
