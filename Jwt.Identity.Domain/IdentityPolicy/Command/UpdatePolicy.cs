using System;
using System.ComponentModel.DataAnnotations;
using Jwt.Identity.Domain.IdentityPolicy.Entity;
using Jwt.Identity.Domain.IdentityPolicy.Enum;
using Jwt.Identity.Framework.Response;
using MediatR;

namespace Jwt.Identity.Domain.IdentityPolicy.Command
{
    public class UpdatePolicy : IRequest<ResultResponse>
    {
        public IdentitySettingPolicy IdentitySetting { get; set; }
    }
}
