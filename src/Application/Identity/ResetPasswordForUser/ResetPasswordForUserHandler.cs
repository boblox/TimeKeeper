using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TimeKeeper.Application.Common.Interfaces;

namespace TimeKeeper.Application.Identity.ResetPasswordForUser
{
    public class ResetPasswordForUserHandler : IRequestHandler<ResetPasswordForUserCommand>
    {
        private readonly IIdentityService _service;

        public ResetPasswordForUserHandler(IIdentityService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ResetPasswordForUserCommand request, CancellationToken cancellationToken)
        {
            await _service.ChangeUserPasswordAsync(request.UserName, request.NewPassword);
            return Unit.Value;
        }
    }
}