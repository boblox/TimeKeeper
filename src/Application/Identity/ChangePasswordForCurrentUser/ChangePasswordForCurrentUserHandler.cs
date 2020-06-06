using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TimeKeeper.Application.Common.Interfaces;

namespace TimeKeeper.Application.Identity.ChangePasswordForCurrentUser
{
    public class ChangePasswordForCurrentUserHandler : IRequestHandler<ChangePasswordForCurrentUserCommand>
    {
        private readonly IIdentityService _service;

        public ChangePasswordForCurrentUserHandler(IIdentityService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ChangePasswordForCurrentUserCommand request, CancellationToken cancellationToken)
        {
            await _service.ChangeCurrentUserPasswordAsync(request.OldPassword, request.NewPassword);
            return Unit.Value;
        }
    }
}