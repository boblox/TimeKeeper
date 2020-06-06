using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TimeKeeper.Application.Common.Exceptions;
using TimeKeeper.Application.Common.Interfaces;

namespace TimeKeeper.Application.WorkingHours.DeleteWorkingHours
{
    public class DeleteWorkingHoursCommandHandler : IRequestHandler<DeleteWorkingHoursCommand>
    {
        private readonly IIdentityService _identityService;
        private readonly IApplicationDbContext _applicationDbContext;

        public DeleteWorkingHoursCommandHandler(IIdentityService identityService, IApplicationDbContext applicationDbContext)
        {
            _identityService = identityService;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Unit> Handle(DeleteWorkingHoursCommand request, CancellationToken cancellationToken)
        {
            var existingWorkingHour = await _applicationDbContext.WorkingHoursSet.FirstOrDefaultAsync(i => i.Id == request.Id);
            if (existingWorkingHour == null)
            {
                throw new ValidationException("Working hours record doesn't exist");
            }

            var user = await _identityService.GetUserByIdAsync(existingWorkingHour.UserId);
            _identityService.CheckUserIsNotNull(user);
            _identityService.CheckUserIsManagingHisOwnRecordsOrIsAdmin(user.UserName);

            _applicationDbContext.WorkingHoursSet.Remove(existingWorkingHour);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}