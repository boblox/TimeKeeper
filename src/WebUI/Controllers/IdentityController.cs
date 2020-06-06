using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeKeeper.Application.Common.Models;
using TimeKeeper.Application.Identity.ChangePasswordForCurrentUser;
using TimeKeeper.Application.Identity.DeleteUser;
using TimeKeeper.Application.Identity.GetUsers;
using TimeKeeper.Application.Identity.Login;
using TimeKeeper.Application.Identity.Register;
using TimeKeeper.Application.Identity.ResetPasswordForUser;
using TimeKeeper.Application.Identity.UpdateRole;

namespace TimeKeeper.WebUI.Controllers
{
    [Route("api/users")]
    public class IdentityController : ApiController
    {
        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPost("current/changePassword")]
        [Authorize]
        public async Task<ActionResult> ChangePasswordForCurrentUser([FromBody] ChangePasswordForCurrentUserCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPost("{userName}/resetPassword")]
        [Authorize(Roles = Roles.CanManageUsers)]
        public async Task<ActionResult> ResetPasswordForUser(string userName, [FromBody] ResetPasswordForUserCommand command)
        {
            command.UserName = userName;
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPost("token")]
        public async Task<ActionResult<string>> GetUserToken([FromBody] GetTokenCommand command)
        {
            var token = await Mediator.Send(command);
            return Ok(token);
        }

        [HttpGet]
        [Authorize(Roles = Roles.CanManageUsers)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            return Ok(await Mediator.Send(new GetUsersQuery()));
        }

        [HttpPut("{userName}/roles")]
        [Authorize(Roles = Roles.CanManageUsers)]
        public async Task<ActionResult> UpdateUserRole(string userName, [FromBody] UpdateRoleCommand command)
        {
            command.UserName = userName;
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{userName}")]
        //TODO: I think permissions is the application level of concerns
        [Authorize(Roles = Roles.CanManageUsers)]
        public async Task<ActionResult> DeleteUser(string userName)
        {
            await Mediator.Send(new DeleteUserCommand(userName));
            return NoContent();
        }
    }
}
