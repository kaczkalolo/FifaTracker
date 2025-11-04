using FifaTracker.Application.Services;
using FifaTracker.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FifaTracker.Application.Matches.Commands.UpdateMatchScore;

public class UpdateMatchScoreCommandHandler : IRequestHandler<UpdateMatchScoreCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IMatchGenerator _matchGenerator;

    public UpdateMatchScoreCommandHandler(IApplicationDbContext context, IMatchGenerator matchGenerator)
    {
        _context = context;
        _matchGenerator = matchGenerator;
    }

    public async Task<Unit> Handle(UpdateMatchScoreCommand request, CancellationToken cancellationToken)
    {
        var match = await _context.Matches
            .Include(m => m.Session)
            .ThenInclude(s => s.SessionUsers)
            .FirstOrDefaultAsync(m => m.Id == request.MatchId, cancellationToken);

        if (match == null)
            throw new KeyNotFoundException($"Match with ID {request.MatchId} not found");

        match.Team1Score = request.Team1Score;
        match.Team2Score = request.Team2Score;
        match.IsCompleted = true;
        match.PlayedAt = DateTime.UtcNow;
        match.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Auto-generate new matches if pending count is below 5
        if (match.Session.Status == Domain.Entities.SessionStatus.Active)
        {
            var allMatches = await _context.Matches
                .Where(m => m.SessionId == match.SessionId)
                .Include(m => m.MatchTeams)
                .ToListAsync(cancellationToken);

            var pendingGeneratedMatches = allMatches.Where(m => !m.IsCompleted && m.IsGenerated).Count();

            if (pendingGeneratedMatches < 5)
            {
                var matchesToGenerate = 5 - pendingGeneratedMatches;
                var userIds = match.Session.SessionUsers.Select(su => su.UserId).ToList();
                var userJoinTimes = match.Session.SessionUsers.ToDictionary(su => su.UserId, su => su.JoinedAt);

                var newMatches = _matchGenerator.GenerateSmartMatches(
                    match.SessionId,
                    userIds,
                    match.Session.MatchType,
                    matchesToGenerate,
                    allMatches,
                    userJoinTimes,
                    match.Session.StartDate);

                foreach (var newMatch in newMatches)
                {
                    _context.Matches.Add(newMatch);
                }

                if (newMatches.Count > 0)
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }

        return Unit.Value;
    }
}
