using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Domain.UseLoginPolicy.Entities;
using Jwt.Identity.Framework.Response;
using MediatR;

namespace Jwt.Identity.Domain.UseLoginPolicy.Command
{
    public class UpSertUserPolicy:IRequest<ResultResponse>
    {
        public UserLoginPolicyOptions UserLoginPolicyOption { get; set; } 
    }
}
