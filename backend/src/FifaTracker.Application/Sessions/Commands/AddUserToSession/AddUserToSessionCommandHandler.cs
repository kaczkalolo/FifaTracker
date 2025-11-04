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

        // Remove all pending generated matches (keep completed and custom matches)
        var pendingGeneratedMatches = await _context.Matches
            .Where(m => m.SessionId == request.SessionId && !m.IsCompleted && m.IsGenerated)
            .ToListAsync(cancellationToken);

        foreach (var match in pendingGeneratedMatches)
        {
            _context.Matches.Remove(match);
        }

        // Get all users including the new one
        var allUserIds = session.SessionUsers.Select(su => su.UserId).ToList();
        allUserIds.Add(request.UserId); // Include the new user
        
        // Get remaining matches (completed + custom pending)
        var existingMatches = await _context.Matches
            .Where(m => m.SessionId == request.SessionId && (m.IsCompleted || !m.IsGenerated))
            .Include(m => m.MatchTeams)
            .ToListAsync(cancellationToken);

        // Build user join times dictionary
        var userJoinTimes = session.SessionUsers.ToDictionary(su => su.UserId, su => su.JoinedAt);
        userJoinTimes[request.UserId] = DateTime.UtcNow;
        
        // Generate 5 new matches considering all users and their join times
        var newMatches = _matchGenerator.GenerateSmartMatches(
            session.Id,
            allUserIds,
            session.MatchType,
            5, // Always generate 5 pending matches
            existingMatches,
            userJoinTimes,
            session.StartDate);

        foreach (var match in newMatches)
        {
            _context.Matches.Add(match);
        }

        session.LastModifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
