using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Domain.Clients.Entity;
using Jwt.Identity.Framework.Response;
using MediatR;

namespace Jwt.Identity.Domain.Clients.Command
{
    // creat and update
    public class UpSert:IRequest<ResultResponse>
    {
        public Client Client { get; set; }
    }
}
