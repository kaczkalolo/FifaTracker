using FifaTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FifaTracker.Infrastructure.Persistence.Configurations;

public class MatchTeamConfiguration : IEntityTypeConfiguration<MatchTeam>
{
    public void Configure(EntityTypeBuilder<MatchTeam> builder)
    {
        builder.ToTable("MatchTeams");

        builder.HasKey(mt => mt.Id);

        builder.Property(mt => mt.MatchId)
            .IsRequired();

        builder.Property(mt => mt.UserId)
            .IsRequired();

        builder.Property(mt => mt.TeamNumber)
            .IsRequired();

        builder.HasOne(mt => mt.Match)
            .WithMany(m => m.MatchTeams)
            .HasForeignKey(mt => mt.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mt => mt.User)
            .WithMany(u => u.MatchTeams)
            .HasForeignKey(mt => mt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(mt => new { mt.MatchId, mt.UserId });
    }
}
