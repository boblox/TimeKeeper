using MediatR;

namespace TimeKeeper.Application.Identity.ResetPasswordForUser
{
    public class ResetPasswordForUserCommand : IRequest
    {
        public string UserName { get; set; }

        public string NewPassword { get; set; }
    }
}
