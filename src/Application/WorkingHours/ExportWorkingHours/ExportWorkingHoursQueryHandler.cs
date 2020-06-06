using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TimeKeeper.Application.Common.Interfaces;

namespace TimeKeeper.Application.WorkingHours.ExportWorkingHours
{
    public class ExportWorkingHoursQueryHandler : IRequestHandler<ExportWorkingHoursQuery, IEnumerable<ExportWorkingHoursDto>>
    {
        private readonly IIdentityService _identityService;
        private readonly IApplicationDbContext _applicationDbContext;

        public ExportWorkingHoursQueryHandler(IIdentityService identityService, IApplicationDbContext applicationDbContext)
        {
            _identityService = identityService;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IEnumerable<ExportWorkingHoursDto>> Handle(ExportWorkingHoursQuery request, CancellationToken cancellationToken)
        {
            _identityService.CheckUserIsManagingHisOwnRecordsOrIsAdmin(request.UserName);
            var user = await _identityService.GetUserByNameAsync(request.UserName);
            _identityService.CheckUserIsNotNull(user);

            var startDate = request.Start.Date;
            var endDate = request.End.Date;
            //TODO: optimize query!
            var result = (await _applicationDbContext.WorkingHoursSet
                .Where(i => i.Date >= startDate && i.Date <= endDate && i.UserId == user.UserId)
                .ToListAsync())
                .GroupBy(i => i.Date)
                .Select(i => new ExportWorkingHoursDto(
                    i.Select(i => i.Description),
                    i.Key,
                    new TimeSpan(i.Sum(i => i.Duration.Ticks))))
                .ToList();

            return result;
        }
    }
}