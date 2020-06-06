using System.Collections.Generic;
using MediatR;
using TimeKeeper.Application.Common.Models;

namespace TimeKeeper.Application.Identity.GetUsers
{
    public class GetUsersQuery : IRequest<IEnumerable<UserDto>>
    {
    }
}
