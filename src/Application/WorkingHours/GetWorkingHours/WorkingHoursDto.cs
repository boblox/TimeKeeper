using System;

namespace TimeKeeper.Application.WorkingHours.GetWorkingHours
{
    public class WorkingHoursDto
    {
        public int Id { get; }
        public string Description { get; }
        public DateTime Date { get; }
        public TimeSpan Duration { get; }
        public bool IsUnderPreferredWorkingHoursDuration { get; }

        public WorkingHoursDto(int id, string description, DateTime date, TimeSpan duration, bool isUnderPreferredWorkingHoursDuration)
        {
            Id = id;
            Description = description;
            Date = date;
            Duration = duration;
            IsUnderPreferredWorkingHoursDuration = isUnderPreferredWorkingHoursDuration;
        }
    }
}