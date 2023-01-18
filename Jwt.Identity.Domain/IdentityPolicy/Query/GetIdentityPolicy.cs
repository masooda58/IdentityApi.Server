using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Domain.IdentityPolicy.Entity;
using MediatR;

namespace Jwt.Identity.Domain.IdentityPolicy.Query
{
    public class GetIdentityPolicy:IRequest<IdentitySettingPolicy>
    {

    }
}
