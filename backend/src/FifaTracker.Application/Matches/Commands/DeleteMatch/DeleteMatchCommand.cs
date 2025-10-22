using MediatR;

namespace FifaTracker.Application.Matches.Commands.DeleteMatch;

public record DeleteMatchCommand(Guid MatchId) : IRequest<Unit>;
