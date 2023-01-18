using System.Collections.Generic;
using Jwt.Identity.Data.Context;
using Jwt.Identity.Data.Repositories.UserRepositories;
using Jwt.Identity.Domain.User.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#pragma warning disable CS1591

namespace Jwt.Identity.Test.Helper
{
    /// <summary>
    ///     می باشد JWT.Identity های پروژه Test برای ساخت Helper این کلاس یک
    /// </summary>
    /// <remarks>
    ///     <para> استفاده می شود Helper یک API,Domain,data برای هر سه پروژه</para>
    ///     <para>
    ///         هر دو برای تست از این کلاس استفاده می کنند<see cref="RoleManagementService" /> و
    ///         <see cref="RoleManagementService" />
    ///     </para>
    /// </remarks>
    public static class DataServiceHelpers

    {
        public static IdentityContext CreateDbContext()

        {
            // Create a new service provider to create a new in-memory database.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance using an in-memory database and 
            // IServiceProvider that the context should resolve all of its 
            // services from.
            var builder = new DbContextOptionsBuilder<IdentityContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .UseInternalServiceProvider(serviceProvider);
            var dbContextOptions = builder.Options;

            return new IdentityContext(dbContextOptions);
        }

        public static UserManager<ApplicationUser> CreateUserManager(IdentityContext context)

        {
            var userStore = new UserStore<ApplicationUser>(context);
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var userValidators = new List<UserValidator<ApplicationUser>> { new() };
            var passwordValidators = new List<PasswordValidator<ApplicationUser>>
                { new() };
            var userLogger = new LoggerFactory().CreateLogger<UserManager<ApplicationUser>>();

            return new UserManager<ApplicationUser>(userStore, null, passwordHasher, userValidators, passwordValidators,
                null, null, null, userLogger);
        }

        public static RoleManager<IdentityRole> CreateRoleManager(IdentityContext context)
        {
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleValidators = new List<RoleValidator<IdentityRole>> { new() };
            var roleLogger = new LoggerFactory().CreateLogger<RoleManager<IdentityRole>>();

            return new RoleManager<IdentityRole>(roleStore, roleValidators, null, null, roleLogger);
        }

        public static List<ApplicationUser> GetTestUsers()

        {
            var returnList = new List<ApplicationUser>();

            returnList.Add(new ApplicationUser
            {
                FirstName = "aFirstName",
                LastName = "bLastName",
                Email = "c@b.cEmail",
                UserName = "d@b.cUserName",
                Approved = true
            });

            returnList.Add(new ApplicationUser
            {
                FirstName = "bFirstName",
                LastName = "cLastName",
                Email = "d@b.cEmail",
                UserName = "a@b.cUserName",
                Approved = false
            });

            returnList.Add(new ApplicationUser
            {
                FirstName = "cFirstName",
                LastName = "dLastName",
                Email = "a@b.cEmail",
                UserName = "b@b.cUserName",
                Approved = false
            });

            returnList.Add(new ApplicationUser
            {
                FirstName = "dFirstName",
                LastName = "aLastName",
                Email = "b@b.cEmail",
                UserName = "c@b.cUserName",
                Approved = false
            });

            return returnList;
        }

        public static RefreshTokenRepository CreatRefreshTokenRepository(IdentityContext context)
        {
            return new RefreshTokenRepository(context);
        }
    }
}