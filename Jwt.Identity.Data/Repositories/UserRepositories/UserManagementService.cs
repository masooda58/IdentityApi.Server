using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Jwt.Identity.Data.Context;
using Jwt.Identity.Domain.User.Data;
using Jwt.Identity.Domain.User.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jwt.Identity.Data.Repositories.UserRepositories
{

    public class UserManagementService : UserManager<ApplicationUser>

    {
        public UserManagementService(IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public async Task<ApplicationUser> FindUserByEmailOrPhoneAsync(string emailOrPhone)
        {
            var user = await Users.FirstOrDefaultAsync(u =>  u.PhoneNumber == emailOrPhone || u.Email == emailOrPhone);
                      
            return user;
        }

        
        public async Task<bool> IsUserExistAsync( string emailOrPhone)
        {
            var userExist = await Users.AnyAsync(u => u.PhoneNumber == emailOrPhone || u.Email == emailOrPhone);
                       
            return userExist;
        }

     
      
    }
}

        //#region User-Role


        //public async Task<List<string>> GetUserRoleAsync(string userIdOrEmail)
        //{
        //    var user = await _context.Users.AsNoTracking().Where(u => u.Id == userIdOrEmail).FirstOrDefaultAsync() ??
        //               await _context.Users.AsNoTracking().Where(u => u.Email == userIdOrEmail).FirstOrDefaultAsync();
        //    if (user != null)
        //    {
        //        var roles = await _userManager.GetRolesAsync(user);
        //        return roles.ToList();
        //    }

        //    return null;
        //}

        //#endregion

        //#region Get-USERS

        //public async Task<int> GetAllUsersCountAsync(string searchString)
        //{
        //    var users = _userManager.Users.AsNoTracking();

        //    if (!string.IsNullOrEmpty(searchString))
        //        users = users.Where(user => user.LastName.Contains(searchString)
        //                                    || user.FirstName.Contains(searchString)
        //                                    || user.Email.Contains(searchString));

        //    return await users.CountAsync();
        //}

        //public async Task<List<ApplicationUser>> GetAllUsersAsync(string searchString = null)
        //{
        //    var users = _userManager.Users.AsNoTracking();

        //    if (!string.IsNullOrEmpty(searchString))
        //        users = users.Where(user => user.LastName.Contains(searchString)
        //                                    || user.FirstName.Contains(searchString)
        //                                    || user.Email.Contains(searchString));

        //    return await users.ToListAsync();
        //}

        //public async Task<List<ApplicationUser>> GetUsersAsync(int offset, int limit, string sortOrder,
        //    string searchString)
        //{
        //    offset = offset < 0 ? 0 : offset;
        //    limit = limit < 0 ? 0 : limit;


        //    var pageUsers = _userManager.Users.AsNoTracking();

        //    if (!string.IsNullOrEmpty(searchString))
        //        pageUsers = pageUsers.Where(user => user.LastName.Contains(searchString)
        //                                            || user.FirstName.Contains(searchString)
        //                                            || user.Email.Contains(searchString));

        //    switch (sortOrder)
        //    {
        //        case "Lname":
        //            pageUsers = pageUsers.OrderBy(u => u.LastName);
        //            break;
        //        case "Lname_desc":
        //            pageUsers = pageUsers.OrderByDescending(u => u.LastName);
        //            break;
        //        case "Fname":
        //            pageUsers = pageUsers.OrderBy(u => u.FirstName);
        //            break;
        //        case "Fname_desc":
        //            pageUsers = pageUsers.OrderByDescending(u => u.FirstName);
        //            break;
        //        case "Email":
        //            pageUsers = pageUsers.OrderBy(u => u.Email);
        //            break;
        //        case "Email_desc":
        //            pageUsers = pageUsers.OrderByDescending(u => u.Email);
        //            break;
        //        case "Approved":
        //            pageUsers = pageUsers.OrderBy(u => u.Approved);
        //            break;
        //        case "Approved_desc":
        //            pageUsers = pageUsers.OrderByDescending(u => u.Approved);
        //            break;
        //        default:
        //            pageUsers = pageUsers.OrderBy(u => u.LastName);
        //            break;
        //    }

        //    pageUsers = pageUsers.Skip(offset).Take(limit);

        //    return await pageUsers.ToListAsync();
        //}

  

        //#endregion

        //#region CRUD-USER

        //public async Task<ApplicationUser> FindUserAsync(string userIdOrName)
        //{
        //    var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userIdOrName)
        //               ?? await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userIdOrName);
        //    return user;
        //}

    







        //#endregion