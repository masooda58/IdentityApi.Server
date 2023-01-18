using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Jwt.Identity.Domain.User.Entities;
using Microsoft.AspNetCore.Identity;

namespace Jwt.Identity.Domain.User.Data
{
    // فقط در پیاده ساز این ماژول استفاده می شود IdentityCore موجود در UserManager از
    public interface IUserManagementService
    {
        Task<List<string>> GetUserRoleAsync(ApplicationUser user);
        Task<int> GetAllUsersCountAsync(string searchString);

        Task<List<ApplicationUser>> GetAllUsersAsync(string searchString);

        // Task<PaginatedList<ApplicationUser>> GetAllUsersPaginatedAsync(int pageIndex, int pageSize, string searchString, string sortOrder);
        Task<List<ApplicationUser>> GetUsersAsync(int offset, int limit, string sortOrder, string searchString);
        Task<List<string>> GetUserRoleAsync(string userIdOrEmail);

        Task<ApplicationUser> FindUserAsync(string userId);
        Task<IdentityResult> AddUserAsync(ApplicationUser user, string password, string role);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user, string newUserRole);
        Task<IdentityResult> DeleteUserAsync(string userId);
        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string newPassword);
        Task<bool> IsEmailInUseAsync(string email, string excludeUserId);
        Task<bool> IsEmailInUseAsync(string email);
        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal claimsPrincipal);


        Task<IdentityResult> DeleteUserAsync(ApplicationUser user);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);


    }
}