using System;
using System.Collections.Generic;

namespace TimeKeeper.Application.WorkingHours.ExportWorkingHours
{
    public class ExportWorkingHoursDto
    {
        public IEnumerable<string> Descriptions { get; }
        public DateTime Date { get; }
        public TimeSpan TotalDuration { get; }

        public ExportWorkingHoursDto(IEnumerable<string> descriptions, DateTime date, TimeSpan totalDuration)
        {
            Descriptions = descriptions;
            Date = date;
            TotalDuration = totalDuration;
        }
    }
}