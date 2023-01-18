using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Jwt.Identity.Data.Repositories.UserRepositories;
using Jwt.Identity.Test.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;

#pragma warning disable CS1591

namespace Jwt.Identity.Test
{
    /// <summary>
    ///     RoleManagerService in Jwt.identity.Data
    /// </summary>
    public class RoleManagerServiceTest
    {
        /// <summary>
        ///     GetRoleByNameAsync_OneRole_ReturnsMatchingRole
        /// </summary>
        /// <param name="rolename"></param>
        /// <param name="expected"></param>
        [Theory]
        [InlineData("testRole", "testRole")]
        [InlineData("NotExist", null)]
        public async Task GetRoleByNameAsync_TwoRole_ReturnsMatchingRole(string rolename, string expected)
        {
            await using var dbContext = DataServiceHelpers.CreateDbContext();
            using var userManager = DataServiceHelpers.CreateUserManager(dbContext);
            using var roleManager = DataServiceHelpers.CreateRoleManager(dbContext);
            // Arrange
           // var userManagementService = new UserManagementService(dbContext, userManager);
            var rolemanagerService = new RoleManagementService(dbContext, roleManager);
            const string ROLE_NAME = "testRole";

            var testRole = new IdentityRole(ROLE_NAME);
            await roleManager.CreateAsync(testRole);

            // Act
            var actualRole = await rolemanagerService.FindRoleByNameAsync(rolename);

            // Assert
            Assert.Equal(expected, actualRole?.Name);
        }

        /// <summary>
        ///     دو تست یکی نام نقش وجود دارد دیگری ندارد و ساخته می شود
        /// </summary>
        /// <param name="rolename"></param>
        /// <param name="expected"></param>
        [Theory]
        [InlineData("testRole", false)]
        [InlineData("NotExist", true)]
        public async Task CreateRoleAsync_OneRole_ReturnsMatchingRole(string rolename, bool expected)
        {
            await using var dbContext = DataServiceHelpers.CreateDbContext();
            //using var userManager = Helpers.CreateUserManager(dbContext);
            using var roleManager = DataServiceHelpers.CreateRoleManager(dbContext);

            //var userManagementService = new UserManagementService(dbContext, userManager);
            var rolemanagerService = new RoleManagementService(dbContext, roleManager);
            const string ROLE_NAME = "testRole";
            var testRole = new IdentityRole(ROLE_NAME);
            await roleManager.CreateAsync(testRole);
            // Act
            var identityResult = await rolemanagerService.CreateRoleAsync(rolename);
            // Assert
            Assert.Equal(expected, identityResult.Succeeded);
        }

        /// <summary>
        ///     دو تست حذف نقش یکی وجود دارد دیگری ندارد
        /// </summary>
        /// <param name="rolename"></param>
        /// <param name="expected"></param>
        [Theory]
        [InlineData("testRole", true)]
        [InlineData("NotExist", true)]
        public async Task DeletRoleAsync_TwoRole_ReturnsMatchingRole(string rolename, bool expected)
        {
            await using var dbContext = DataServiceHelpers.CreateDbContext();
            //using var userManager = Helpers.CreateUserManager(dbContext);
            using var roleManager = DataServiceHelpers.CreateRoleManager(dbContext);

            //var userManagementService = new UserManagementService(dbContext, userManager);
            var rolemanagerService = new RoleManagementService(dbContext, roleManager);
            //const string ROLE_NAME = "testRole";
            var testRole = new IdentityRole(rolename);

            var g = await roleManager.CreateAsync(testRole); //test role expected true delet
            // Act
            var identityResult = await rolemanagerService.DeleteRoleAsync(testRole);
            // Assert
            Assert.Equal(expected, identityResult.Succeeded);
        }

        [Theory]
        [InlineData("testRole", 1)]
        public async Task GetAllRolesAsync__ReturnsMatchingRole(string rolename, int expected)
        {
            using (var dbContext = DataServiceHelpers.CreateDbContext())
            using (var userManager = DataServiceHelpers.CreateUserManager(dbContext))
            using (var roleManager = DataServiceHelpers.CreateRoleManager(dbContext))
            {
                //var userManagementService = new UserManagementService(dbContext, userManager);
                var rolemanagerService = new RoleManagementService(dbContext, roleManager);
                //const string ROLE_NAME = "testRole";
                //var testRole = new IdentityRole(ROLE_NAME);
                var role = new IdentityRole(rolename);
                var cc = dbContext.Roles.AsNoTracking().ToList();

                var d = await roleManager.CreateAsync(role); //test role expected true delet
                // Act
                var identityResult = await rolemanagerService.GetAllRolesAsync();
                // Assert
                Assert.Equal(expected, identityResult.Count);
            }
        }

        [Theory]
        [ClassData(typeof(RoleDataForXuint))]
        public async Task DeleteRolesByNameAsync_ReturnOk(List<string> rolesName)
        {
            using (var dbContext = DataServiceHelpers.CreateDbContext())
                //using (var userManager = Helpers.CreateUserManager(dbContext))
            using (var roleManager = DataServiceHelpers.CreateRoleManager(dbContext))
            {
                //var userManagementService = new UserManagementService(dbContext, userManager);
                var rolemanagerService = new RoleManagementService(dbContext, roleManager);
                var roles = new List<IdentityRole>();
                foreach (var name in rolesName)
                {
                    var role = new IdentityRole(name);
                    await roleManager.CreateAsync(role);
                    roles.Add(new IdentityRole(name));
                }

                //Act
                //await dbContext.Roles.AddRangeAsync(roles);
                //dbContext.SaveChanges();
                var s = dbContext.Roles.ToList();

                var result = await rolemanagerService.DeleteRolesByNameAsync(rolesName);


                //Assert
                var sLists = dbContext.Roles.ToList();
                Assert.True(sLists.Count == 0);
            }
        }

        [Theory]
        [InlineData("Admin", true)]
        public async Task CreateRoleAsync_ByRole_Returnok(string roleName, bool expected)
        {
            using (var dbContext = DataServiceHelpers.CreateDbContext())
                //using (var userManager = Helpers.CreateUserManager(dbContext))
            using (var roleManager = DataServiceHelpers.CreateRoleManager(dbContext))
            {
                //var userManagementService = new UserManagementService(dbContext, userManager);
                var rolemanagerService = new RoleManagementService(dbContext, roleManager);
                var roles = new IdentityRole(roleName);
                //Act
                var result = await rolemanagerService.CreateRoleAsync(roles);
                //Assert
                Assert.Equal(expected, result.Succeeded);
            }
        }

        [Theory]
        [InlineData("Admin", "Admin")]
        public async Task FindRoleByIdAsync_Returnok(string roleName, string expected)
        {
            using (var dbContext = DataServiceHelpers.CreateDbContext())
                //using (var userManager = Helpers.CreateUserManager(dbContext))
            using (var roleManager = DataServiceHelpers.CreateRoleManager(dbContext))
            {
                //var userManagementService = new UserManagementService(dbContext, userManager);
                var rolemanagerService = new RoleManagementService(dbContext, roleManager);
                var role = new IdentityRole(roleName);
                //Act
                await rolemanagerService.CreateRoleAsync(role);
                var result = await rolemanagerService.FindRoleByIdAsync(role.Id);
                //Assert
                Assert.Equal(expected, result.Name);
            }
        }

        [Fact]
        public async Task AddClaimToRoleAsync_ReturnOk()
        {
            using (var dbContext = DataServiceHelpers.CreateDbContext())
                //using (var userManager = Helpers.CreateUserManager(dbContext))
            using (var roleManager = DataServiceHelpers.CreateRoleManager(dbContext))
            {
                var rolemanagerService = new RoleManagementService(dbContext, roleManager);
                var role = new IdentityRole("Admin");
                var creatRole = await rolemanagerService.CreateRoleAsync(role);
                var newClaim = new Claim(ClaimTypes.Name, "claim1");
                //Act
                var result = await rolemanagerService.AddClaimToRoleAsync(role, newClaim);

                Assert.True(result.Succeeded);
            }
        }

        [Fact]
        public async Task AddClaimsToRoleAsync_ReturnOk()
        {
            using (var dbContext = DataServiceHelpers.CreateDbContext())
                //using (var userManager = Helpers.CreateUserManager(dbContext))
            using (var roleManager = DataServiceHelpers.CreateRoleManager(dbContext))
            {
                var rolemanagerService = new RoleManagementService(dbContext, roleManager);
                var role = new IdentityRole("Admin");
                var creatRole = await rolemanagerService.CreateRoleAsync(role);
                var authClaims = new List<Claim>
                {
                    new(ClaimTypes.Name, "user.UserName"),
                    new("BoursYarAccess", "x"),
                    new("BoursYarAccess", "y"),
                    new("id", "user.Id")
                };
                //Act
                var results = await rolemanagerService.AddClaimsToRoleAsync(role, authClaims);
                foreach (var result in results) Assert.True(result.Succeeded);
            }
        }

        [Fact]
        public async Task RemoveClaimsToRoleAsync_ReturnOk()
        {
            using (var dbContext = DataServiceHelpers.CreateDbContext())
                //using (var userManager = Helpers.CreateUserManager(dbContext))
            using (var roleManager = DataServiceHelpers.CreateRoleManager(dbContext))
            {
                var rolemanagerService = new RoleManagementService(dbContext, roleManager);
                var removClaims = new List<Claim>
                {
                    new(ClaimTypes.Name, "user.UserName"),
                    // new Claim("BoursYarAccess","x"),
                    new("BoursYarAccess", "y"),
                    new("id", "user.Id"),
                    new("notAdd", "notAdd")
                };
                var role = new IdentityRole("Admin");
                var creatRole = await rolemanagerService.CreateRoleAsync(role);
                var authClaims = new List<Claim>
                {
                    new(ClaimTypes.Name, "user.UserName"),
                    new("BoursYarAccess", "x"),
                    new("BoursYarAccess", "y"),
                    new("id", "user.Id")
                };
                //Act
                var resultsAdd = await rolemanagerService.AddClaimsToRoleAsync(role, authClaims);


                var resultsRemove = await rolemanagerService.RemoveClaimsToRoleAsync(role, removClaims);
                var remainClaims = await rolemanagerService.GetClaimsByRoleNameAsync("Admin");
                Assert.True(resultsRemove.Count(rr => rr.Succeeded) == 4
                            && remainClaims.Count == 1 &&
                            remainClaims.FirstOrDefault(c => c.Type == "BoursYarAccess")?.Value == "x");
            }
        }

        [Fact]
        public async Task GetClaimsByRoleNameAsync_ReturnnNull()
        {
            using (var dbContext = DataServiceHelpers.CreateDbContext())
                //using (var userManager = Helpers.CreateUserManager(dbContext))
            using (var roleManager = DataServiceHelpers.CreateRoleManager(dbContext))
            {
                var rolemanagerService = new RoleManagementService(dbContext, roleManager);
                var role = new IdentityRole("admin");

                //Act
                var resultsByName = await rolemanagerService.GetClaimsByRoleNameAsync("admin");
                var resultsByRole = await rolemanagerService.GetClaimsByRoleAsync(role);
                //Assert
                Assert.True(resultsByName == resultsByRole);
            }
        }

        [Fact]
        public async Task ChangRoleNameAsync_Returnok()
        {
            using (var dbContext = DataServiceHelpers.CreateDbContext())
                //using (var userManager = Helpers.CreateUserManager(dbContext))
            using (var roleManager = DataServiceHelpers.CreateRoleManager(dbContext))
            {
                var rolemanagerService = new RoleManagementService(dbContext, roleManager);
                var role = new IdentityRole("admin");
                var authClaims = new List<Claim>
                {
                    new(ClaimTypes.Name, "user.UserName"),
                    new("BoursYarAccess", "x"),
                    new("BoursYarAccess", "y"),
                    new("id", "user.Id")
                };

                //Act
                await rolemanagerService.CreateRoleAsync(role);
                var resultAddClaim = await rolemanagerService.AddClaimsToRoleAsync(role, authClaims);
                var result = await rolemanagerService.ChangRoleNameAsync(role, "NewName");

                //Assert
                //var roleBack = await rolemanagerService.GetAllRolesAsync();
                //var testRole = roleBack.FirstOrDefault(x => x.Name == "NewName");
                //var resultclaim = await rolemanagerService.GetClaimsByRoleAsync(testRole);
                Assert.True(result.Succeeded);
            }
        }
    }
}