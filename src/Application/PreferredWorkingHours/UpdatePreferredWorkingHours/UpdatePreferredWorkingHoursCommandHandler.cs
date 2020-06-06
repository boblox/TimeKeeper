using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TimeKeeper.Application.Common.Exceptions;
using TimeKeeper.Application.Common.Interfaces;
using TimeKeeper.Application.Common.Models;
using TimeKeeper.Domain.Entities;

namespace TimeKeeper.Application.PreferredWorkingHours.UpdatePreferredWorkingHours
{
    public class UpdatePreferredWorkingHoursCommandHandler : IRequestHandler<UpdatePreferredWorkingHoursCommand>
    {
        private readonly IIdentityService _identityService;
        private readonly IApplicationDbContext _applicationDbContext;

        public UpdatePreferredWorkingHoursCommandHandler(IIdentityService identityService, IApplicationDbContext applicationDbContext)
        {
            _identityService = identityService;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Unit> Handle(UpdatePreferredWorkingHoursCommand request, CancellationToken cancellationToken)
        {
            _identityService.CheckUserIsManagingHisOwnRecordsOrIsAdmin(request.UserName);
            var user = await _identityService.GetUserByNameAsync(request.UserName);
             _identityService.CheckUserIsNotNull(user);

            var existingWH = await _applicationDbContext.UserPreferredWorkingHoursSet.FirstOrDefaultAsync(i => i.UserId == user.UserId);
            if (existingWH == null)
            {
                _applicationDbContext.UserPreferredWorkingHoursSet.Add(new UserPreferredWorkingHours(user.UserId, request.Duration));
            }
            else
            {
                existingWH.Duration = request.Duration;
            }
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}