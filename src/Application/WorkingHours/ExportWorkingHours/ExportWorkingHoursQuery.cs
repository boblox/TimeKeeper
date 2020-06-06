using System;
using System.Collections.Generic;
using MediatR;

namespace TimeKeeper.Application.WorkingHours.ExportWorkingHours
{
    public class ExportWorkingHoursQuery : IRequest<IEnumerable<ExportWorkingHoursDto>>
    {
        public DateTime Start { get; }
        public DateTime End { get; }
        public string UserName { get; }

        public ExportWorkingHoursQuery(string userName, DateTime? start, DateTime? end)
        {
            Start = start ?? DateTime.MinValue;
            End = end ?? DateTime.MaxValue;
            UserName = userName;
        }
    }
}
