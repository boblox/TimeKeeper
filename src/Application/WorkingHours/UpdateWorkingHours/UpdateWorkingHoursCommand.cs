using System;
using MediatR;

namespace TimeKeeper.Application.WorkingHours.UpdateWorkingHours
{
    public class UpdateWorkingHoursCommand : IRequest
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
