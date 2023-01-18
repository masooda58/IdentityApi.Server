using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jwt.Identity.Api.Server.Resources;
using Jwt.Identity.Api.Server.Security;
using Jwt.Identity.Data.UnitOfWork;
using Jwt.Identity.Domain.IdentityPolicy.Command;
using Jwt.Identity.Domain.IdentityPolicy.Entity;
using Jwt.Identity.Framework.Response;
using Jwt.Identity.Framework.Tools;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Jwt.Identity.Api.Server.Services.ApplicationService.IdentitySettings.Command
{
    public class UpdateHandler:IRequestHandler<UpdatePolicy,ResultResponse>
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IdentityOptions options;
        private readonly DataProtectionTokenProviderOptions _tokenProviderOptions;
        private readonly EmailConfirmationTokenProviderOptions _emailTokenProrvider;

        public UpdateHandler(UnitOfWork unitOfWork, IOptions<IdentityOptions> options, 
            IOptions<DataProtectionTokenProviderOptions> tokenProviderOptions,
            IOptions<EmailConfirmationTokenProviderOptions> emailTokenProrvider)
        {
            _unitOfWork = unitOfWork;
            _emailTokenProrvider = emailTokenProrvider.Value;
            _tokenProviderOptions = tokenProviderOptions.Value;
            this.options = options.Value;
        }


        public async Task<ResultResponse> Handle(UpdatePolicy request, CancellationToken cancellationToken)
        {

            if (_unitOfWork.IdentitySettingPolicy.SettingExist(request.IdentitySetting.Id))
            {
                var setting = request.IdentitySetting;
                _unitOfWork.IdentitySettingPolicy.UpdateSetting(request.IdentitySetting);
                _unitOfWork.Save();
                options.Password.RequireDigit = setting.RequireDigit;
                options.Password.RequiredLength = setting.RequiredLength;
                options.Password.RequireNonAlphanumeric = setting.RequireNonAlphanumeric;
                options.Password.RequireUppercase = setting.RequireUppercase;
                options.Password.RequireLowercase = setting.RequireLowercase;
                options.Password.RequiredUniqueChars = setting.RequiredUniqueChars;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(setting.DefaultLockoutTimeSpanMinute);
                options.Lockout.MaxFailedAccessAttempts = setting.MaxFailedAccessAttempts;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = false;

                // SignIn settings
                //options.SignIn.RequireConfirmedEmail = false;
                // options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedAccount = setting.RequireConfirmedAccount;

                //Email token provider
                options.Tokens.EmailConfirmationTokenProvider = "EmailConFirmation";
                _tokenProviderOptions.TokenLifespan = TimeSpan.FromHours(setting.TokenLifespanHour);
                _emailTokenProrvider.TokenLifespan = TimeSpan.FromHours(setting.TokenLifespanHour);
                return new ResultResponse(true, "Policy Updated", request.IdentitySetting);

            }

            throw new Exception(ErrorRes.EntityNotExist);



        }
    }
}
