using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Domain.UseLoginPolicy.Entities;
using Jwt.Identity.Domain.User.Entities;

namespace Jwt.Identity.Domain.User.Data
{
    public interface IUserPolicyManagementService
    {
        public Task<bool> AddPolicyToUserAsync(ApplicationUserPolicy userPolicy);
        public bool ChangeUserPolicy(ApplicationUserPolicy userPolicy);
        public Task<string> GetPolicyIdByUserIdAsync(string userId);
        public Task<UserLoginPolicyOptions> GetPolicyByUserIdAsync(string userId);
        public Task<List<ApplicationUser>> GetAllUserWithPolicyIdAsync(string policyId);
    }
}
