﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Domain.Clients.Entity;
using MediatR;

namespace Jwt.Identity.Domain.UseLoginPolicy.Query
{
    public class GetAllUserloginPolicyOption:IRequest<IEnumerable<GetAllUserloginPolicyOption>>
    {
    }
}
