using TimeKeeper.Application.Common.Interfaces;
using System;

namespace TimeKeeper.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
