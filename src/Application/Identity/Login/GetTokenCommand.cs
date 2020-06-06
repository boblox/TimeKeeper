using MediatR;

namespace TimeKeeper.Application.Identity.Login
{
    public class GetTokenCommand : IRequest<string>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
