using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Data.Context;
using Jwt.Identity.Data.Repositories.BaseRepository;
using Jwt.Identity.Domain.UseLoginPolicy.Data;
using Jwt.Identity.Domain.UseLoginPolicy.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jwt.Identity.Data.Repositories.UserLoginOptionRepository
{
    public class UserLoginOptionRepositoryServices:BaseGenricRepository<UserLoginPolicyOptions>
    {
        private readonly IdentityContext _context;
        public UserLoginOptionRepositoryServices(IdentityContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<UserLoginPolicyOptions> GetByAsync(object policyName)
        {
            return await _context.UserLoginPolicyOptions.FirstOrDefaultAsync(c=>
                c.PolicyName==policyName.ToString());
        }

        public override async Task<UserLoginPolicyOptions> GetByIdNotraking(object id)
        {
            return await _context.UserLoginPolicyOptions.AsNoTracking().FirstOrDefaultAsync(c=>
               c.Id==id.ToString());
        }
    }
}
