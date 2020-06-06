using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using TimeKeeper.Application.Common.Exceptions;
using TimeKeeper.Application.Common.Interfaces;

namespace TimeKeeper.Application.Common.Behaviours
{
    public class AuthorizeBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public AuthorizeBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService, IIdentityService identityService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _identityService = identityService;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var currentUser = _currentUserService.User;
            if (currentUser != null && !string.IsNullOrEmpty(currentUser.UserName))
            {
                var userFromDb = await _identityService.GetUserByNameAsync(currentUser.UserName);
                if (userFromDb == null) throw new ForbiddenException("Authenticated user doesn't exist anymore");
                if (userFromDb.Role != currentUser.Role) throw new ForbiddenException("User's role has changed. Please get a new token");
            }
        }
    }
}