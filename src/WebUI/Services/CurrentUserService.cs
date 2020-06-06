using TimeKeeper.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TimeKeeper.Application.Common.Models;

namespace TimeKeeper.WebUI.Services
{
    //TODO: move it to Infrastructure!
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;
            User = user != null ?
                new UserDto(user.FindFirstValue(ClaimTypes.Name),
                    user.FindFirstValue(ClaimTypes.NameIdentifier),
                    user.FindFirstValue(ClaimTypes.Role)) :
                null;
        }

        public UserDto User { get; }
    }
}
