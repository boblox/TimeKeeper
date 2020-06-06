using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TimeKeeper.Application.Common.Exceptions;
using TimeKeeper.Application.Common.Interfaces;

namespace TimeKeeper.Application.Identity.UpdateRole
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand>
    {
        private readonly IIdentityService _service;

        public UpdateRoleCommandHandler(IIdentityService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            await _service.UpdateRoleAsync(request.UserName, request.DesiredRole);
            return Unit.Value;
        }
    }
}