using System;
using System.Collections.Generic;

namespace TimeKeeper.Application.WorkingHours.GetWorkingHours
{
    public class WorkingHoursSetDto
    {
        public string UserName { get; }
        public TimeSpan PreferredWorkingHoursDuration { get; }
        public List<WorkingHoursDto> WorkingHoursList { get; }

        public WorkingHoursSetDto(string userName, TimeSpan preferredWorkingHoursDuration, List<WorkingHoursDto> workingHoursList)
        {
            UserName = userName;
            PreferredWorkingHoursDuration = preferredWorkingHoursDuration;
            WorkingHoursList = workingHoursList;
        }
    }
}