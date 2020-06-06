using System;
using System.Collections.Generic;
using MediatR;

namespace TimeKeeper.Application.WorkingHours.GetWorkingHours
{
    public class GetWorkingHoursQuery : IRequest<WorkingHoursSetDto>
    {
        public DateTime Start { get; }
        public DateTime End { get; }
        public string UserName { get; }

        public GetWorkingHoursQuery(string userName, DateTime? start, DateTime? end)
        {
            Start = start ?? DateTime.MinValue;
            End = end ?? DateTime.MaxValue;
            UserName = userName;
        }
    }
}
