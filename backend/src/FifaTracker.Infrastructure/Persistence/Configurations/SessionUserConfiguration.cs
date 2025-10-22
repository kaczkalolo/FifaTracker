using FifaTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FifaTracker.Infrastructure.Persistence.Configurations;

public class SessionUserConfiguration : IEntityTypeConfiguration<SessionUser>
{
    public void Configure(EntityTypeBuilder<SessionUser> builder)
    {
        builder.ToTable("SessionUsers");

        builder.HasKey(su => new { su.SessionId, su.UserId });

        builder.HasOne(su => su.Session)
            .WithMany(s => s.SessionUsers)
            .HasForeignKey(su => su.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(su => su.User)
            .WithMany(u => u.SessionUsers)
            .HasForeignKey(su => su.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(su => su.JoinedAt)
            .IsRequired();
    }
}
