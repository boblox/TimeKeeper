using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TimeKeeper.Application.Common.Interfaces;
using TimeKeeper.Application.Common.Models;

namespace TimeKeeper.Application.Identity.GetUsers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IIdentityService _service;

        public GetUsersQueryHandler(IIdentityService service)
        {
            _service = service;
        }

        public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetUsersAsync();
        }
    }
}