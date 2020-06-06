using MediatR;

namespace TimeKeeper.Application.Identity.ChangePasswordForCurrentUser
{
    public class ChangePasswordForCurrentUserCommand : IRequest
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
