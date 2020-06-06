using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeKeeper.Domain.Entities;
using TimeKeeper.Infrastructure.Identity;

namespace TimeKeeper.Infrastructure.Persistence.Configurations
{
    public class UserWorkingHoursConfiguration : IEntityTypeConfiguration<UserWorkingHours>
    {
        public const string TableName = "UserWorkingHoursList";

        public void Configure(EntityTypeBuilder<UserWorkingHours> builder)
        {
            builder.ToTable(TableName);
            builder.Property(i => i.Duration).IsRequired();
            builder.Property(i => i.Date).IsRequired();
            builder.Property(t => t.Description)
                .HasMaxLength(500)
                .IsRequired();
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .IsRequired();
        }
    }
}