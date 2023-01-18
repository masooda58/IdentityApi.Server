using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Domain.IServices;
using Jwt.Identity.Domain.UseLoginPolicy.Entities;

namespace Jwt.Identity.Domain.UseLoginPolicy.Data
{
    public interface IUserLoginOptionRepository:IBaseGenricRepository<UserLoginPolicyOptions>
    {


    }
}
