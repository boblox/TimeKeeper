using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TimeKeeper.Infrastructure.Identity
{
    public interface IUserManagerAdapter<TUser>
    {
        Task<TUser> FindByIdAsync(string userId);

        Task<TUser> FindByNameAsync(string userName);

        Task<IdentityResult> CreateAsync(TUser user, string password);

        Task<IdentityResult> AddToRoleAsync(TUser user, string role);

        Task<IdentityResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword);

        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string newPassword);

        Task<SignInResult> CheckPasswordSignInAsync(TUser user, string password, bool lockoutOnFailure);

        Task<IList<TUser>> GetUsersInRoleAsync(string roleName);

        Task<IdentityResult> RemoveFromRoleAsync(TUser user, string role);

        Task<IdentityResult> DeleteAsync(TUser user);

        Task<IList<string>> GetRolesAsync(TUser user);
    }
}
