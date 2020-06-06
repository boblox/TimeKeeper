using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TimeKeeper.Application.Common.Exceptions;
using TimeKeeper.Application.Common.Interfaces;

namespace TimeKeeper.Application.Identity.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IIdentityService _service;

        public DeleteUserCommandHandler(IIdentityService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            await _service.DeleteUserAsync(request.UserName);
            return Unit.Value;
        }
    }
}