using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TimeKeeper.Application.Common.Exceptions;
using TimeKeeper.Application.Common.Interfaces;

namespace TimeKeeper.Application.WorkingHours.GetWorkingHours
{
    public class GetWorkingHoursQueryHandler : IRequestHandler<GetWorkingHoursQuery, WorkingHoursSetDto>
    {
        private readonly IIdentityService _identityService;
        private readonly IApplicationDbContext _applicationDbContext;

        public GetWorkingHoursQueryHandler(IIdentityService identityService, IApplicationDbContext applicationDbContext)
        {
            _identityService = identityService;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<WorkingHoursSetDto> Handle(GetWorkingHoursQuery request, CancellationToken cancellationToken)
        {
            _identityService.CheckUserIsManagingHisOwnRecordsOrIsAdmin(request.UserName);
            var user = await _identityService.GetUserByNameAsync(request.UserName);
            _identityService.CheckUserIsNotNull(user);

            var startDate = request.Start.Date;
            var endDate = request.End.Date;

            if (startDate > endDate) throw new ValidationException("Start can't be bigger than end");

            var preferredWorkingHoursDuration = _applicationDbContext.UserPreferredWorkingHoursSet
                .FirstOrDefault(i => i.UserId == user.UserId)?.Duration ?? TimeSpan.Zero;
            var workingHoursList = await _applicationDbContext.WorkingHoursSet
                .Where(i => i.Date >= startDate && i.Date <= endDate && i.UserId == user.UserId)
                .ToListAsync();

            var totalDurationsByDate = workingHoursList
                    .GroupBy(i => i.Date)
                    .ToDictionary(i => i.Key,
                        i => new TimeSpan(i.Sum(i => i.Duration.Ticks)));
            var workingHoursListDto = workingHoursList
                .OrderByDescending(i => i.Date)
                .Select(i =>
                    new WorkingHoursDto(i.Id, i.Description, i.Date, i.Duration, totalDurationsByDate[i.Date] < preferredWorkingHoursDuration))
                .ToList();

            return new WorkingHoursSetDto(user.UserName, preferredWorkingHoursDuration, workingHoursListDto);
        }
    }
}