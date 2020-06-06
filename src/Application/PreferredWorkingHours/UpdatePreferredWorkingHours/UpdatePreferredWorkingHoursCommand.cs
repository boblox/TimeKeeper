using System;
using MediatR;

namespace TimeKeeper.Application.PreferredWorkingHours.UpdatePreferredWorkingHours
{
    public class UpdatePreferredWorkingHoursCommand : IRequest
    {
        public string UserName { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
