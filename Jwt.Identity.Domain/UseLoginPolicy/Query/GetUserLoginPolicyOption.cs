using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Domain.UseLoginPolicy.Entities;
using MediatR;

namespace Jwt.Identity.Domain.UseLoginPolicy.Query
{
    public class GetUserLoginPolicyOption:IRequest<UserLoginPolicyOptions>
    {
        public string PolicyId { get; set; }
    }
}
