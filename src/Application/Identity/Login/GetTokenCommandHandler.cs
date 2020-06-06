using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TimeKeeper.Application.Common.Exceptions;
using TimeKeeper.Application.Common.Interfaces;

namespace TimeKeeper.Application.Identity.Login
{
    public class GetTokenCommandHandler : IRequestHandler<GetTokenCommand, string>
    {
        private readonly IIdentityService _service;

        public GetTokenCommandHandler(IIdentityService service)
        {
            _service = service;
        }

        public async Task<string> Handle(GetTokenCommand request, CancellationToken cancellationToken)
        {
            return await _service.GetTokenAsync(request.UserName, request.Password);
        }
    }
}