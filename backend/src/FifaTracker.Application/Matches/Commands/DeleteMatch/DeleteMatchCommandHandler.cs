using FifaTracker.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FifaTracker.Application.Matches.Commands.DeleteMatch;

public class DeleteMatchCommandHandler : IRequestHandler<DeleteMatchCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteMatchCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteMatchCommand request, CancellationToken cancellationToken)
    {
        var match = await _context.Matches
            .Include(m => m.MatchTeams)
            .Include(m => m.Session)
            .FirstOrDefaultAsync(m => m.Id == request.MatchId, cancellationToken);

        if (match == null)
            throw new KeyNotFoundException($"Match with ID {request.MatchId} not found");

        // Only allow deleting matches from active sessions
        if (match.Session.Status != Domain.Entities.SessionStatus.Active)
            throw new InvalidOperationException("Cannot delete matches from completed sessions");

        _context.Matches.Remove(match);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
