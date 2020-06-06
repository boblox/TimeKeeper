using System;
using TimeKeeper.Domain.Common;

namespace TimeKeeper.Domain.Entities
{
    public class UserWorkingHours : AuditableEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }

        public UserWorkingHours(string userId, string description, DateTime date, TimeSpan duration)
        {
            UserId = userId;
            Description = description;
            Date = date;
            Duration = duration;
        }
    }
}