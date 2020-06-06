using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TimeKeeper.Application.Common.Exceptions;
using TimeKeeper.Application.Common.Interfaces;
using TimeKeeper.Application.Common.Models;
using TimeKeeper.Domain.Entities;

namespace TimeKeeper.Application.WorkingHours.CreateWorkingHours
{
    public class CreateWorkingHoursCommandHandler : IRequestHandler<CreateWorkingHoursCommand, int>
    {
        private readonly IIdentityService _identityService;
        private readonly IApplicationDbContext _applicationDbContext;

        public CreateWorkingHoursCommandHandler(IIdentityService identityService, IApplicationDbContext applicationDbContext)
        {
            _identityService = identityService;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<int> Handle(CreateWorkingHoursCommand request, CancellationToken cancellationToken)
        {
            _identityService.CheckUserIsManagingHisOwnRecordsOrIsAdmin(request.UserName);
            var user = await _identityService.GetUserByNameAsync(request.UserName);
            _identityService.CheckUserIsNotNull(user);

            var date = request.Date.Date;
            var workingHoursOnThatDay = await _applicationDbContext.WorkingHoursSet
                .Where(i => i.Date == date && i.UserId == user.UserId).ToListAsync();
            var sumOfWorkingHoursOnThatDay = new TimeSpan(workingHoursOnThatDay.Sum(i => i.Duration.Ticks));
            if (sumOfWorkingHoursOnThatDay + request.Duration > DurationConstants.MaxDuration)
                throw new ValidationException($"Sum of working hours per day can't be bigger than {DurationConstants.MaxDuration.TotalHours} hours");

            var workingHours = new UserWorkingHours(user.UserId, request.Description, date, request.Duration);
            _applicationDbContext.WorkingHoursSet.Add(workingHours);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return workingHours.Id;
        }
    }
}