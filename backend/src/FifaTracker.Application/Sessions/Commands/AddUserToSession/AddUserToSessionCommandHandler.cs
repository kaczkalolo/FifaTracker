using FifaTracker.Application.Services;
using FifaTracker.Domain.Entities;
using FifaTracker.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FifaTracker.Application.Sessions.Commands.AddUserToSession;

public class AddUserToSessionCommandHandler : IRequestHandler<AddUserToSessionCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IMatchGenerator _matchGenerator;

    public AddUserToSessionCommandHandler(IApplicationDbContext context, IMatchGenerator matchGenerator)
    {
        _context = context;
        _matchGenerator = matchGenerator;
    }

    public async Task<Unit> Handle(AddUserToSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions
            .Include(s => s.SessionUsers)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

        if (session == null)
            throw new KeyNotFoundException($"Session with ID {request.SessionId} not found");

        // Check if user exists and is active
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {request.UserId} not found");
        if (!user.IsActive)
            throw new InvalidOperationException("Cannot add inactive user to session");

        // Check if user is already in session
        if (session.SessionUsers.Any(su => su.UserId == request.UserId))
            throw new InvalidOperationException("User is already in this session");

        // Add user to session
        var sessionUser = new SessionUser
        {
            SessionId = request.SessionId,
            UserId = request.UserId,
            JoinedAt = DateTime.UtcNow
        };
        _context.SessionUsers.Add(sessionUser);

        // Generate missing matches if requested
        if (request.GenerateMissingMatches)
        {
            var allUserIds = session.SessionUsers.Select(su => su.UserId).ToList();

            var allMatches = _matchGenerator.GenerateMatches(session.Id, allUserIds, session.MatchType);
            var existingMatches = await _context.Matches
                .Where(m => m.SessionId == request.SessionId)
                .Include(m => m.MatchTeams)
                .ToListAsync(cancellationToken);

            // Filter out matches that already exist
            var newMatches = allMatches.Where(newMatch =>
                !existingMatches.Any(existingMatch =>
                    MatchesAreEqual(newMatch, existingMatch)
                )).ToList();

            foreach (var match in newMatches)
            {
                _context.Matches.Add(match);
            }
        }

        session.LastModifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private bool MatchesAreEqual(Match match1, Match match2)
    {
        var team1Match1 = match1.MatchTeams.Where(mt => mt.TeamNumber == 1).Select(mt => mt.UserId).OrderBy(id => id).ToList();
        var team2Match1 = match1.MatchTeams.Where(mt => mt.TeamNumber == 2).Select(mt => mt.UserId).OrderBy(id => id).ToList();

        var team1Match2 = match2.MatchTeams.Where(mt => mt.TeamNumber == 1).Select(mt => mt.UserId).OrderBy(id => id).ToList();
        var team2Match2 = match2.MatchTeams.Where(mt => mt.TeamNumber == 2).Select(mt => mt.UserId).OrderBy(id => id).ToList();

        return team1Match1.SequenceEqual(team1Match2) && team2Match1.SequenceEqual(team2Match2);
    }
}
