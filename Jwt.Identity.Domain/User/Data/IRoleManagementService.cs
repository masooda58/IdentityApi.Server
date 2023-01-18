using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Jwt.Identity.Domain.User.Data
{
    public interface IRoleManagementService
    {
        Task<IdentityResult> CreateRoleAsync(string roleName);
        Task<IdentityResult> DeleteRoleAsync(IdentityRole role);
        Task<bool> DeleteRolesByNameAsync(List<string> rolesName);
        Task<List<IdentityRole>> GetAllRolesAsync(string searchRoleName);
        Task<IdentityRole> FindRoleByNameAsync(string name);
        Task<IdentityRole> FindRoleByIdAsync(string roleId);
        Task<List<Claim>> GetClaimsByRoleNameAsync(string roleName);
        Task<List<Claim>> GetClaimsByRoleAsync(IdentityRole role);
        Task<IdentityResult> AddClaimToRoleAsync(IdentityRole role, Claim claim);
        Task<List<IdentityResult>> AddClaimsToRoleAsync(IdentityRole role, List<Claim> claims);
        Task<List<IdentityResult>> RemoveClaimsToRoleAsync(IdentityRole role, List<Claim> claims);

        Task<IdentityResult> ChangRoleNameAsync(IdentityRole role, string newName);


        Task<IdentityResult> CreateRoleAsync(IdentityRole role);
    }
}