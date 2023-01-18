using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Framework.Response;
using MediatR;

namespace Jwt.Identity.Domain.Clients.Command
{
    public class DeletClient:IRequest<bool>
    {
        public string ClientName { get; set; }
    }
}
