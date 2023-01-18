using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Data.Context;
using Jwt.Identity.Domain.UseLoginPolicy.Data;
using Jwt.Identity.Domain.UseLoginPolicy.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jwt.Identity.Data.Repositories
{
    public class UserLoginPolicyOptionRepository:IUserLoginOptionRepository
    {
        private readonly IdentityContext _context;

        public UserLoginPolicyOptionRepository(IdentityContext context)
        {
            _context = context;
        }


        public IEnumerable<UserLoginPolicyOptions> Get(Expression<Func<UserLoginPolicyOptions, bool>> filter = null, Func<IQueryable<UserLoginPolicyOptions>, IOrderedQueryable<UserLoginPolicyOptions>> orderBy = null, string includeProperties = "")
        {
            throw new NotImplementedException();
        }

        public Task<UserLoginPolicyOptions> GetByAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<UserLoginPolicyOptions> GetById(object id)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(UserLoginPolicyOptions entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(UserLoginPolicyOptions entityToDelete)
        {
            throw new NotImplementedException();
        }

        public void Update(UserLoginPolicyOptions entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
