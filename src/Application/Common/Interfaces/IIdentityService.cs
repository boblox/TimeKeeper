using System.Collections.Generic;
using TimeKeeper.Application.Common.Models;
using System.Threading.Tasks;

namespace TimeKeeper.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        void CheckUserIsManagingHisOwnRecordsOrIsAdmin(string userName);

        void CheckUserIsNotNull(UserDto user);

        Task<UserDto> GetUserByIdAsync(string userId);

        Task<UserDto> GetUserByNameAsync(string userName);

        Task DeleteUserAsync(string userName);

        Task<string> GetTokenAsync(string userName, string password);

        Task CreateUserAsync(string userName, string password);

        Task<IEnumerable<UserDto>> GetUsersAsync();

        Task UpdateRoleAsync(string userName, string desiredRole);

        Task ChangeCurrentUserPasswordAsync(string oldPassword, string newPassword);

        Task ChangeUserPasswordAsync(string userName, string newPassword);
    }
}
