using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeKeeper.Domain.Entities;
using TimeKeeper.Infrastructure.Identity;

namespace TimeKeeper.Infrastructure.Persistence.Configurations
{
    public class UserPreferredWorkingHoursConfiguration : IEntityTypeConfiguration<UserPreferredWorkingHours>
    {
        public const string TableName = "UserPreferredWorkingHoursList";

        public void Configure(EntityTypeBuilder<UserPreferredWorkingHours> builder)
        {
            builder.ToTable(TableName);
            builder.Property(i => i.Duration).IsRequired();
            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<UserPreferredWorkingHours>(i=>i.UserId)
                .IsRequired();
        }
    }
}