using FifaTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FifaTracker.Domain.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Session> Sessions { get; }
    DbSet<SessionUser> SessionUsers { get; }
    DbSet<Match> Matches { get; }
    DbSet<MatchTeam> MatchTeams { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
