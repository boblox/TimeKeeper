using System;
using MediatR;

namespace TimeKeeper.Application.WorkingHours.DeleteWorkingHours
{
    public class DeleteWorkingHoursCommand : IRequest
    {
        public int Id { get; set; }

        public DeleteWorkingHoursCommand(int id)
        {
            Id = id;
        }
    }
}
