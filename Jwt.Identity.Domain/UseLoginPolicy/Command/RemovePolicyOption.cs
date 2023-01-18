using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Framework.Response;
using MediatR;

namespace Jwt.Identity.Domain.UseLoginPolicy.Command
{
    public class RemovePolicyOption:IRequest<ResultResponse>
    {
        public int PolicyId { get; set; }
    }
}
