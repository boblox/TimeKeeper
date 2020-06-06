using System;
using TimeKeeper.Domain.Common;

namespace TimeKeeper.Domain.Entities
{
    public class UserPreferredWorkingHours : AuditableEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public TimeSpan Duration { get; set; }

        public UserPreferredWorkingHours(string userId, TimeSpan duration)
        {
            UserId = userId;
            Duration = duration;
        }
    }
}