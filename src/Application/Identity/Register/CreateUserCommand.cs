using MediatR;

namespace TimeKeeper.Application.Identity.Register
{
    public class CreateUserCommand : IRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        //TODO: will we use them?
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
    }
}