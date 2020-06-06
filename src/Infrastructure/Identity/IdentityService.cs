using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using TimeKeeper.Application.Common.Interfaces;
using TimeKeeper.Application.Common.Models;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TimeKeeper.Application.Common.Exceptions;

namespace TimeKeeper.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IUserManagerAdapter<ApplicationUser> _userManagerAdapter;
        private readonly ICurrentUserService _currentUserService;
        private readonly IConfiguration _configuration;
        private const string NoUserFound = "User with such username doesn't exist";
        private const string NoRoleFound = "Desired role doesn't exist";
        private const string CantDeleteCurrentUser = "Couldn't delete current user";
        private const string SignInGeneralFailure = "Username or password is incorrect";
        private const string SignInUserHasNoRole = "Can't authenticate user without any role";
        private readonly string CantPromoteToAdminRole = $"Can't promote role to {Roles.Admin} user when active user has {Roles.UserManager} role";
        private readonly string CantChangeRoleForActiveUser = "Can't change role for active user";
        private readonly string CantChangeRoleOfAdmin = $"Can't degrade role of {Roles.Admin} user when active user has {Roles.UserManager} role";
        private readonly string CantDeleteAdmin = $"Can't delete user of {Roles.Admin} role when active user has {Roles.UserManager} role";
        private readonly string NonAdminCanManageOnlyItsOwnRecords = $"User without {Roles.Admin} role can manage only it's own records";
        private readonly string CantResetPasswordForAdmin = $"Can't reset the password for user with {Roles.Admin} role when active user has {Roles.UserManager} role";

        public IdentityService(
            IUserManagerAdapter<ApplicationUser> userManagerAdapter,
            ICurrentUserService currentUserService,
            IConfiguration configuration)
        {
            _userManagerAdapter = userManagerAdapter;
            _currentUserService = currentUserService;
            _configuration = configuration;
        }

        public void CheckUserIsManagingHisOwnRecordsOrIsAdmin(string userName)
        {
            var currentUser = _currentUserService.User;
            if (currentUser.UserName != userName && currentUser.Role != Roles.Admin) throw new ForbiddenException(NonAdminCanManageOnlyItsOwnRecords);
        }

        public void CheckUserIsNotNull(UserDto user)
        {
            if (user == null) throw new ValidationException(NoUserFound);
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManagerAdapter.FindByIdAsync(userId);
            if (user != null)
            {
                var role = await GetMainRoleAsync(user);
                return new UserDto(user.UserName, user.Id, role);
            }
            return null;
        }

        public async Task<UserDto> GetUserByNameAsync(string userName)
        {
            var user = await _userManagerAdapter.FindByNameAsync(userName);
            if (user != null)
            {
                var role = await GetMainRoleAsync(user);
                return new UserDto(user.UserName, user.Id, role);
            }
            return null;
        }

        public async Task CreateUserAsync(string userName, string password)
        {
            var user = new ApplicationUser
            {
                //Email = model.UserName, //TODO: do we need an email here?
                UserName = userName
            };
            var userCreateResult = await _userManagerAdapter.CreateAsync(user, password);
            if (!userCreateResult.Succeeded) throw userCreateResult.Errors.ToValidationException();

            var addToRoleResult = await _userManagerAdapter.AddToRoleAsync(user, Roles.User);
            if (!addToRoleResult.Succeeded) throw addToRoleResult.Errors.ToValidationException();
        }

        public async Task ChangeCurrentUserPasswordAsync(string oldPassword, string newPassword)
        {
            var currentUserName = _currentUserService.User.UserName;
            var user = await _userManagerAdapter.FindByNameAsync(currentUserName);
            var result = await _userManagerAdapter.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!result.Succeeded) throw result.Errors.ToValidationException();
        }

        public async Task ChangeUserPasswordAsync(string userName, string newPassword)
        {
            var roleOfCurrentUser = _currentUserService.User.Role;
            var user = await _userManagerAdapter.FindByNameAsync(userName);
            if (user == null) throw new ValidationException(NoUserFound);
            var mainUserRole = await GetMainRoleAsync(user);
            if (roleOfCurrentUser == Roles.UserManager && mainUserRole == Roles.Admin) throw new ForbiddenException(CantResetPasswordForAdmin);

            var result = await _userManagerAdapter.ChangePasswordAsync(user, newPassword);
            if (!result.Succeeded) throw result.Errors.ToValidationException();
        }

        public async Task<string> GetTokenAsync(string userName, string password)
        {
            var user = await _userManagerAdapter.FindByNameAsync(userName);
            if (user == null) throw new ValidationException(SignInGeneralFailure);

            var signInResult = await _userManagerAdapter.CheckPasswordSignInAsync(user, password, false);
            if (!signInResult.Succeeded) throw new ValidationException(SignInGeneralFailure);

            var mainUserRole = await GetMainRoleAsync(user);
            if (string.IsNullOrEmpty(mainUserRole)) throw new ValidationException(SignInUserHasNoRole);

            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); //TODO: make it constants!
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Role, mainUserRole),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = new List<UserDto>();
            foreach (var role in Roles.All)
            {
                var usersForRole = (await _userManagerAdapter.GetUsersInRoleAsync(role))
                    .Select(i => new UserDto(i.UserName, i.Id, role));
                users.AddRange(usersForRole);
            }
            return users.OrderBy(u => u.UserName);
        }

        public async Task UpdateRoleAsync(string userName, string desiredRole)
        {
            if (Roles.All.All(role => role != desiredRole)) Result.Failure(NoRoleFound);

            var roleOfCurrentUser = _currentUserService.User.Role;
            if (roleOfCurrentUser == Roles.UserManager && desiredRole == Roles.Admin) throw new ForbiddenException(CantPromoteToAdminRole);

            var currentUserName = _currentUserService.User.UserName;
            if (currentUserName == userName) throw new ForbiddenException(CantChangeRoleForActiveUser);

            var user = await _userManagerAdapter.FindByNameAsync(userName);
            if (user == null) throw new ValidationException(NoUserFound);

            var mainUserRole = await GetMainRoleAsync(user);
            if (roleOfCurrentUser == Roles.UserManager && mainUserRole == Roles.Admin) throw new ForbiddenException(CantChangeRoleOfAdmin);

            if (!string.IsNullOrEmpty(mainUserRole))
            {
                await _userManagerAdapter.RemoveFromRoleAsync(user, mainUserRole);
            }

            await _userManagerAdapter.AddToRoleAsync(user, desiredRole);
        }

        public async Task DeleteUserAsync(string userName)
        {
            var currentUserName = _currentUserService.User.UserName;
            var roleOfCurrentUser = _currentUserService.User.Role;

            if (currentUserName == userName) throw new ForbiddenException(CantDeleteCurrentUser);

            var user = await _userManagerAdapter.FindByNameAsync(userName);
            if (user == null) throw new ValidationException(NoUserFound);
            var mainUserRole = await GetMainRoleAsync(user);

            if (roleOfCurrentUser == Roles.UserManager && mainUserRole == Roles.Admin) throw new ForbiddenException(CantDeleteAdmin);

            var result = await _userManagerAdapter.DeleteAsync(user);
            if (!result.Succeeded) throw result.Errors.ToValidationException();
        }

        private async Task<string> GetMainRoleAsync(ApplicationUser user)
        {
            return (await _userManagerAdapter.GetRolesAsync(user))?[0];
        }
    }
}
