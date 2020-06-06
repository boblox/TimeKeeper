using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TimeKeeper.Application.Common.Exceptions;
using TimeKeeper.Application.Common.Interfaces;
using TimeKeeper.Application.Common.Models;

namespace TimeKeeper.Application.WorkingHours.UpdateWorkingHours
{
    public class UpdateWorkingHoursCommandHandler : IRequestHandler<UpdateWorkingHoursCommand>
    {
        private readonly IIdentityService _identityService;
        private readonly IApplicationDbContext _applicationDbContext;

        public UpdateWorkingHoursCommandHandler(IIdentityService identityService, IApplicationDbContext applicationDbContext)
        {
            _identityService = identityService;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Unit> Handle(UpdateWorkingHoursCommand request, CancellationToken cancellationToken)
        {
            var date = request.Date.Date;
            var existingWorkingHours = await _applicationDbContext.WorkingHoursSet.FirstOrDefaultAsync(i => i.Id == request.Id);
            if (existingWorkingHours == null)
            {
                throw new ValidationException("Working hours record doesn't exist");
            }

            var user = await _identityService.GetUserByIdAsync(existingWorkingHours.UserId);
            _identityService.CheckUserIsNotNull(user);
            _identityService.CheckUserIsManagingHisOwnRecordsOrIsAdmin(user.UserName);

            //TODO: make some helper method!
            var workingHoursOnThatDay = await _applicationDbContext.WorkingHoursSet
                .Where(i => i.Date == date && i.Id != request.Id && i.UserId == user.UserId).ToListAsync();
            var sumOfWorkingHoursOnThatDay = new TimeSpan(workingHoursOnThatDay.Sum(i => i.Duration.Ticks));
            if (sumOfWorkingHoursOnThatDay + request.Duration > DurationConstants.MaxDuration)
                throw new ValidationException($"Sum of working hours per days can't be bigger than {DurationConstants.MaxDuration.TotalHours} hours");

            existingWorkingHours.Date = date;
            existingWorkingHours.Duration = request.Duration;
            existingWorkingHours.Description = request.Description;
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}