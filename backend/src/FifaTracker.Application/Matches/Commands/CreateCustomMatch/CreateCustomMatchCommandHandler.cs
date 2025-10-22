using FifaTracker.Domain.Entities;
using FifaTracker.Domain.Interfaces;
using MediatR;

namespace FifaTracker.Application.Matches.Commands.CreateCustomMatch;

public class CreateCustomMatchCommandHandler : IRequestHandler<CreateCustomMatchCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateCustomMatchCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateCustomMatchCommand request, CancellationToken cancellationToken)
    {
        var match = new Match
        {
            Id = Guid.NewGuid(),
            SessionId = request.SessionId,
            IsGenerated = false,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            MatchTeams = new List<MatchTeam>()
        };

        foreach (var userId in request.Team1UserIds)
        {
            match.MatchTeams.Add(new MatchTeam
            {
                Id = Guid.NewGuid(),
                MatchId = match.Id,
                UserId = userId,
                TeamNumber = 1
            });
        }

        foreach (var userId in request.Team2UserIds)
        {
            match.MatchTeams.Add(new MatchTeam
            {
                Id = Guid.NewGuid(),
                MatchId = match.Id,
                UserId = userId,
                TeamNumber = 2
            });
        }

        _context.Matches.Add(match);
        await _context.SaveChangesAsync(cancellationToken);

        return match.Id;
    }
}
