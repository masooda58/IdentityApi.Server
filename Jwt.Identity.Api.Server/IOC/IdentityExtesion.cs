using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jwt.Identity.Api.Server.Helpers.CustomSignIn;
using Jwt.Identity.Api.Server.Security;
using Jwt.Identity.Data.Context;
using Jwt.Identity.Data.Repositories.UserRepositories;
using Jwt.Identity.Data.UnitOfWork;
using Jwt.Identity.Domain.User.Entities;
using Jwt.Identity.Framework.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Jwt.Identity.Api.Server.IOC
{
  public static class IdentityExtesion
    {
        public static void IdentityConfigExtension(this IServiceCollection services)
        {
            var unitofwork = services.BuildServiceProvider().GetRequiredService<UnitOfWork>();
           
            var setting = unitofwork.IdentitySettingPolicy.GetSetting();
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders()
                .AddUserManager<UserManagementService>()
                //email Token Provider
                .AddTokenProvider<EmailConfirmationTokenProvide<ApplicationUser>>("EmailConFirmation")
                .AddSignInManager<CustomSignInManager>() //custom signin manage for mobile or email
                .AddErrorDescriber<PersianIdentityErrorDescriber>();
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
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
            });
            // تغییر زمان اعتبار همه توکن های ساخته شده
            services.Configure<DataProtectionTokenProviderOptions>(o => { o.TokenLifespan = TimeSpan.FromHours(setting.TokenLifespanHour); });
            //تغییر زمان توکن ایمیل
            services.Configure<EmailConfirmationTokenProviderOptions>(o => { o.TokenLifespan = TimeSpan.FromHours(setting.TokenLifespanHour); });
         
        }
    }
}
