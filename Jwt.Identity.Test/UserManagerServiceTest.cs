//using System.Security.Claims;
//using System.Threading.Tasks;
//using Jwt.Identity.Data.Repositories.UserRepositories;
//using Jwt.Identity.Domain.User.Entities;
//using Jwt.Identity.Test.Helper;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using Xunit;
//using static Jwt.Identity.Test.Helper.DataServiceHelpers;

//// است Summery این خط برای نداشتن کامنت CS1591
//// در این فایل برای توابع راهنما نوشته نشده است
//#pragma warning disable CS1591

//namespace Jwt.Identity.Test
//{
//    /// <summary>
//    ///     Data در پروژه UserManagerService تست
//    /// </summary>
//    /// <remarks>
//    ///     در این فایل برای توابع راهنما نوشته نشده است اسامی گویاست
//    /// </remarks>
//    public class UserManagerServiceTest
//    {
//        private const string ACCEPTABLE_PASSWORD = "accEPTable123!@#Pass";

//        [Theory]
//        [InlineData("aFirstName", 1)]
//        [InlineData("aLastName", 1)]
//        [InlineData("a@b.cEmail", 1)]
//        [InlineData("FirstName", 4)]
//        [InlineData("LastName", 4)]
//        [InlineData("Email", 4)]
//        public async Task GetAllUsersCountAsync_ManyUsers_ReturnsMatchingUserCount(string searchString,
//            int expectedCount)
//        {
//            using (var dbContext = CreateDbContext())
//            using (var userManager = CreateUserManager(dbContext))
//            using (var roleManager = CreateRoleManager(dbContext))
//            {
//                // Arrange
//                var userManagementService = new UserManagementService(dbContext, userManager);

//                var testUsers = GetTestUsers();

//                foreach (var user in testUsers) await userManager.CreateAsync(user, ACCEPTABLE_PASSWORD);

//                // Act
//                var actualCount = await userManagementService.GetAllUsersCountAsync(searchString);

//                // Assert
//                Assert.Equal(expectedCount, actualCount);
//            }
//        }

//        // Tests all possible combinations for sorting for first and last user using offset
//        [Theory]
//        [InlineData("Fname", 0, "d@b.cUserName")]
//        [InlineData("Fname", 3, "c@b.cUserName")]
//        [InlineData("Fname_desc", 0, "c@b.cUserName")]
//        [InlineData("Fname_desc", 3, "d@b.cUserName")]
//        [InlineData("Lname", 0, "c@b.cUserName")]
//        [InlineData("Lname", 3, "b@b.cUserName")]
//        [InlineData("Lname_desc", 0, "b@b.cUserName")]
//        [InlineData("Lname_desc", 3, "c@b.cUserName")]
//        [InlineData("Email", 0, "b@b.cUserName")]
//        [InlineData("Email", 3, "a@b.cUserName")]
//        [InlineData("Email_desc", 0, "a@b.cUserName")]
//        [InlineData("Email_desc", 3, "b@b.cUserName")]
//        [InlineData("Approved", 0, "a@b.cUserName")]
//        [InlineData("Approved", 3, "d@b.cUserName")]
//        [InlineData("Approved_desc", 0, "d@b.cUserName")]
//        [InlineData("Approved_desc", 3, "c@b.cUserName")]
//        public async Task GetUsersAsync_ManyUsers_ReturnsCorrectFirstUser(string sortOrder, int offset,
//            string expectedFirstUserName)
//        {
//            using (var dbContext = CreateDbContext())
//            using (var userManager = CreateUserManager(dbContext))
//            using (var roleManager = CreateRoleManager(dbContext))
//            {
//                // Arrange
//                var userManagementService = new UserManagementService(dbContext, userManager);

//                var testUsers = GetTestUsers();

//                foreach (var user in testUsers)
//                {
//                    var result = await userManager.CreateAsync(user, ACCEPTABLE_PASSWORD);
//                }

//                // Act
//                var actualFirstUserName =
//                    (await userManagementService.GetUsersAsync(offset, 1, sortOrder, null))[0].UserName;

//                // Assert
//                Assert.Equal(expectedFirstUserName, actualFirstUserName);
//            }
//        }

//        [Fact]
//        public async Task GetUserRoleAsync_UserWithuserIdOrEmailAndRole_ReturnsRole()
//        {
//            using (var dbContext = CreateDbContext())
//            using (var userManager = CreateUserManager(dbContext))
//            using (var roleManager = CreateRoleManager(dbContext))
//            {
//                // Arrange
//                var userManagementService = new UserManagementService(dbContext, userManager);

//                const string ROLE_NAME = "administrator";
//                var testRole = new IdentityRole(ROLE_NAME);
//                await roleManager.CreateAsync(testRole);

//                var testUsers = GetTestUsers();

//                var user = testUsers[0];
//                await userManager.CreateAsync(user, ACCEPTABLE_PASSWORD);
//                await userManager.AddToRoleAsync(user, ROLE_NAME);

//                // Act
//                var actualRoleName = await userManagementService.GetUserRoleAsync(user.Email);

//                // Assert
//                Assert.Equal(ROLE_NAME, actualRoleName[0]);
//            }
//        }

//        [Fact]
//        public async Task AddUserAsync_EmailExists_ReturnsError()
//        {
//            using (var dbContext = CreateDbContext())
//            using (var userManager = CreateUserManager(dbContext))
//            using (var roleManager = CreateRoleManager(dbContext))
//            {
//                // Arrange
//                var userManagementService = new UserManagementService(dbContext, userManager);
//                const string ROLE_NAME = "administrator";
//                const string NEW_EMAIL = "c@b.cEmail";

//                var testRole = new IdentityRole(ROLE_NAME);
//                await roleManager.CreateAsync(testRole);

//                var testUsers = GetTestUsers();

//                var user1 = testUsers[0];
//                await userManager.CreateAsync(user1, ACCEPTABLE_PASSWORD);
//                await userManager.AddToRoleAsync(user1, ROLE_NAME);

//                var user2 = testUsers[1];
//                user2.Email = NEW_EMAIL;

//                // Act
//                var identityResult = await userManagementService.AddUserAsync(user2, ACCEPTABLE_PASSWORD, ROLE_NAME);

//                // Assert
//                Assert.False(identityResult.Succeeded);
//            }
//        }

//        [Fact]
//        public async Task AddUserAsync_EmailUnique_AddsUserWithPassword()
//        {
//            using (var dbContext = CreateDbContext())
//            using (var userManager = CreateUserManager(dbContext))
//            using (var roleManager = CreateRoleManager(dbContext))
//            {
//                // Arrange
//                var userManagementService = new UserManagementService(dbContext, userManager);

//                const string ROLE_NAME = "administrator";
//                var testRole = new IdentityRole(ROLE_NAME);
//                await roleManager.CreateAsync(testRole);

//                var testUsers = GetTestUsers();

//                var user = testUsers[0];

//                // Act
//                await userManagementService.AddUserAsync(user, ACCEPTABLE_PASSWORD, ROLE_NAME);

//                // Assert
//                Assert.True(await userManager.CheckPasswordAsync(user, ACCEPTABLE_PASSWORD));
//            }
//        }

//        [Fact]
//        public async Task AddUserAsync_EmailUnique_AddsUserToRole()
//        {
//            using (var dbContext = CreateDbContext())
//            using (var userManager = CreateUserManager(dbContext))
//            using (var roleManager = CreateRoleManager(dbContext))
//            {
//                // Arrange
//                var userManagementService = new UserManagementService(dbContext, userManager);

//                const string ROLE_NAME = "administrator";
//                var testRole = new IdentityRole(ROLE_NAME);
//                await roleManager.CreateAsync(testRole);

//                var testUsers = GetTestUsers();

//                var user = testUsers[0];

//                // Act
//                await userManagementService.AddUserAsync(user, ACCEPTABLE_PASSWORD, ROLE_NAME);

//                // Assert
//                Assert.True(await userManager.IsInRoleAsync(user, ROLE_NAME));
//            }
//        }

//        [Fact]
//        public async Task UpdateUserAsync_EmailExists_ReturnsError()
//        {
//            using (var dbContext = CreateDbContext())
//            using (var userManager = CreateUserManager(dbContext))
//            using (var roleManager = CreateRoleManager(dbContext))
//            {
//                // Arrange
//                var userManagementService = new UserManagementService(dbContext, userManager);

//                const string ROLE_NAME = "administrator";
//                const string NEW_EMAIL = "c@b.cEmail";
//                var testRole = new IdentityRole(ROLE_NAME);
//                await roleManager.CreateAsync(testRole);

//                var testUsers = GetTestUsers();

//                var user1 = testUsers[0];
//                await userManager.CreateAsync(user1, ACCEPTABLE_PASSWORD);
//                await userManager.AddToRoleAsync(user1, ROLE_NAME);

//                var user2 = testUsers[1];
//                await userManager.CreateAsync(user2, ACCEPTABLE_PASSWORD);
//                await userManager.AddToRoleAsync(user2, ROLE_NAME);
//                user2.Email = NEW_EMAIL;

//                // Act
//                var identityResult = await userManagementService.UpdateUserAsync(user2, ROLE_NAME);

//                // Assert
//                Assert.False(identityResult.Succeeded);
//            }
//        }

//        [Fact]
//        public async Task UpdateUserAsync_EmailUnique_UpdatesUserProperties()
//        {
//            using (var dbContext = CreateDbContext())
//            using (var userManager = CreateUserManager(dbContext))
//            using (var roleManager = CreateRoleManager(dbContext))
//            {
//                // Arrange
//                var userManagementService = new UserManagementService(dbContext, userManager);

//                const string ROLE_NAME = "administrator";
//                var testRole = new IdentityRole(ROLE_NAME);
//                await roleManager.CreateAsync(testRole);

//                var testUsers = GetTestUsers();

//                var user1 = testUsers[0];
//                await userManager.CreateAsync(user1, ACCEPTABLE_PASSWORD);
//                await userManager.AddToRoleAsync(user1, ROLE_NAME);

//                user1.FirstName = "ADifferentFirstName";
//                user1.LastName = "ADifferentFirstName";
//                user1.Email = "aDifferent@email.com";
//                user1.UserName = "aDifferent@username.com";

//                // Act
//                await userManagementService.UpdateUserAsync(user1, ROLE_NAME);
//                var actualUser = await dbContext.Users
//                    .AsNoTracking() // Retrieve fresh entity rather than through the Change Tracker
//                    .FirstOrDefaultAsync(user => user.Id == user1.Id);

//                // Assert
//                //JsonConvert.SerializeObject(obj1) == JsonConvert.SerializeObject(obj2)
//                Assert.True(JsonConvert.SerializeObject(actualUser) == JsonConvert.SerializeObject(user1));
//            }
//        }

//        [Fact]
//        public async Task UpdateUserAsync_EmailUnique_UpdatesUserRole()
//        {
//            using (var dbContext = CreateDbContext())
//            using (var userManager = CreateUserManager(dbContext))
//            using (var roleManager = CreateRoleManager(dbContext))
//            {
//                // Arrange
//                var userManagementService = new UserManagementService(dbContext, userManager);

//                const string ROLE_NAME1 = "administrator";
//                const string ROLE_NAME2 = "supervisor";
//                await roleManager.CreateAsync(new IdentityRole(ROLE_NAME1));
//                await roleManager.CreateAsync(new IdentityRole(ROLE_NAME2));

//                var testUsers = GetTestUsers();

//                var user1 = testUsers[0];
//                await userManager.CreateAsync(user1, ACCEPTABLE_PASSWORD);
//                await userManager.AddToRoleAsync(user1, ROLE_NAME1);

//                // Act
//                await userManagementService.UpdateUserAsync(user1, ROLE_NAME2);

//                // Assert
//                Assert.True(await userManager.IsInRoleAsync(user1, ROLE_NAME2) &&
//                            !await userManager.IsInRoleAsync(user1, ROLE_NAME1));
//            }
//        }

//        [Fact]
//        public async Task DeleteUserAsync_UserExists_DeletesUser()
//        {
//            using (var dbContext = CreateDbContext())
//            using (var userManager = CreateUserManager(dbContext))
//            using (var roleManager = CreateRoleManager(dbContext))
//            {
//                // Arrange
//                var userManagementService = new UserManagementService(dbContext, userManager);

//                var testUsers = GetTestUsers();

//                var user1 = testUsers[0];
//                await userManager.CreateAsync(user1, ACCEPTABLE_PASSWORD);

//                // Act
//                await userManagementService.DeleteUserAsync(user1.Id);

//                // Assert
//                var actualUser = await userManager.FindByIdAsync(user1.Id);
//                Assert.Null(actualUser);
//            }
//        }

//        [Fact]
//        public async Task ChangePasswordAsync_UserExists_ChangesPassword()
//        {
//            using (var dbContext = CreateDbContext())
//            using (var userManager = CreateUserManager(dbContext))
//            using (var roleManager = CreateRoleManager(dbContext))
//            {
//                // Arrange
//                var userManagementService = new UserManagementService(dbContext, userManager);

//                var testUsers = GetTestUsers();

//                var user1 = testUsers[0];
//                await userManager.CreateAsync(user1, ACCEPTABLE_PASSWORD);

//                const string ACCEPTABLE_PASSWORD2 = "accEPTable123!@#Pass2";
//                // Act
//                await userManagementService.ChangePasswordAsync(user1, ACCEPTABLE_PASSWORD2);

//                // Assert
//                Assert.True(await userManager.CheckPasswordAsync(user1, ACCEPTABLE_PASSWORD2));
//            }
//        }

//        [Fact]
//        public async Task IsEmailInUseAsync_EmailInUseUserNotExcluded_ReturnsTrue()
//        {
//            using (var dbContext = CreateDbContext())
//            using (var userManager = CreateUserManager(dbContext))
//            using (var roleManager = CreateRoleManager(dbContext))
//            {
//                // Arrange
//                var userManagementService = new UserManagementService(dbContext, userManager);

//                var testUsers = GetTestUsers();

//                var user1 = testUsers[0];
//                await userManager.CreateAsync(user1, ACCEPTABLE_PASSWORD);

//                // Act
//                var result = await userManagementService.IsEmailInUseAsync(user1.Email, null);

//                // Assert
//                Assert.True(result);
//            }
//        }

//        [Fact]
//        public async Task IsEmailInUseAsync_EmailInUseUserExcluded_ReturnsFalse()
//        {
//            using (var dbContext = CreateDbContext())
//            using (var userManager = CreateUserManager(dbContext))
//            using (var roleManager = CreateRoleManager(dbContext))
//            {
//                // Arrange
//                var userManagementService = new UserManagementService(dbContext, userManager);

//                var testUsers = GetTestUsers();

//                var user1 = testUsers[0];
//                await userManager.CreateAsync(user1, ACCEPTABLE_PASSWORD);

//                // Act
//                var result = await userManagementService.IsEmailInUseAsync(user1.Email, user1.Id);

//                // Assert
//                Assert.False(result);
//            }
//        }

//        [Fact]
//        public async Task GetUserAsync_UserExists_ReturnsUser()
//        {
//            using (var dbContext = CreateDbContext())
//            using (var userManager = CreateUserManager(dbContext))
//            using (var roleManager = CreateRoleManager(dbContext))
//            {
//                // Arrange
//                var userManagementService = new UserManagementService(dbContext, userManager);

//                var testUsers = GetTestUsers();

//                var user1 = testUsers[0];
//                await userManager.CreateAsync(user1, ACCEPTABLE_PASSWORD);

//                var claims = new[]
//                {
//                    new Claim(ClaimTypes.NameIdentifier, user1.Id)
//                };

//                var identity = new ClaimsIdentity(claims, "Test");
//                var principal = new ClaimsPrincipal(identity);

//                // Act
//                var actualUser = await userManagementService.GetUserAsync(principal);

//                // Assert
//                Assert.Equal(actualUser, user1);
//            }
//        }

//        [Theory]
//        [ClassData(typeof(UserDataForXuit))]
//        public async Task FindUserAsync_TwoCheckUserExistOrNot_returnUserorNull(ApplicationUser userdata)
//        {
//            using (var dbContext = CreateDbContext())
//            using (var userManager = CreateUserManager(dbContext))
//            using (var roleManager = CreateRoleManager(dbContext))
//            {
//                // Arrange
//            //    var userManagementService = new UserManagementService(dbContext, userManager);

//                var testUsers = GetTestUsers();

//                var user1 = testUsers[0];
//                await userManager.CreateAsync(user1, ACCEPTABLE_PASSWORD);
//                var user1Id = user1.Id;
//                var user2id = userdata;
//              //  var actualUser = await userManagementService.FindUserAsync(user1Id);
//              //  var nullUser = await userManagementService.FindUserAsync(user2id.Id);
//              //  Assert.True(actualUser != null && nullUser == null);
//            }
//        }
//    }
//}