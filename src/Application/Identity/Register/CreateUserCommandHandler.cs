using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TimeKeeper.Application.Common.Exceptions;
using TimeKeeper.Application.Common.Interfaces;

namespace TimeKeeper.Application.Identity.Register
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        private readonly IIdentityService _service;

        public CreateUserCommandHandler(IIdentityService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            await _service.CreateUserAsync(request.UserName, request.Password);
            return Unit.Value;
        }
    }
}