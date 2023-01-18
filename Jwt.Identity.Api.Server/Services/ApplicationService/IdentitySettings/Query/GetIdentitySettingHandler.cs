using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jwt.Identity.Data.UnitOfWork;
using Jwt.Identity.Domain.IdentityPolicy.Entity;
using Jwt.Identity.Domain.IdentityPolicy.Enum;
using Jwt.Identity.Domain.IdentityPolicy.Query;
using Jwt.Identity.Framework.Response;
using MediatR;

namespace Jwt.Identity.Api.Server.Services.ApplicationService.IdentitySettings.Query
{
    
    public class GetIdentitySettingHandler : IRequestHandler<GetIdentityPolicy, IdentitySettingPolicy>
    {
        private readonly UnitOfWork _unitOfWork;

        public GetIdentitySettingHandler(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IdentitySettingPolicy> Handle(GetIdentityPolicy request, CancellationToken cancellationToken)
        {
            return _unitOfWork.IdentitySettingPolicy.GetSetting();
        }
    }
}
