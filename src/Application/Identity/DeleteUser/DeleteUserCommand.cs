using MediatR;

namespace TimeKeeper.Application.Identity.DeleteUser
{
    public class DeleteUserCommand : IRequest
    {
        public string UserName { get; set; }

        public DeleteUserCommand(string userName)
        {
            this.UserName = userName;
        }
    }
}
