using System;
using MediatR;

namespace TimeKeeper.Application.WorkingHours.CreateWorkingHours
{
    public class CreateWorkingHoursCommand : IRequest<int>
    {
        public string UserName { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
