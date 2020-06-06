using TimeKeeper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace TimeKeeper.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<UserWorkingHours> WorkingHoursSet { get; set; }

        DbSet<UserPreferredWorkingHours> UserPreferredWorkingHoursSet { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
