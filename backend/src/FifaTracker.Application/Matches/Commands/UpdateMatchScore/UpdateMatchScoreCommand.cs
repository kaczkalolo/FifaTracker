using MediatR;

namespace FifaTracker.Application.Matches.Commands.UpdateMatchScore;

public record UpdateMatchScoreCommand(
    Guid MatchId,
    int Team1Score,
    int Team2Score
) : IRequest<Unit>;
