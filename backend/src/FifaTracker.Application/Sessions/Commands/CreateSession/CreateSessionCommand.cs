using MediatR;

namespace FifaTracker.Application.Sessions.Commands.CreateSession;

public record CreateSessionCommand(
    string Name,
    FifaTracker.Domain.Entities.MatchType MatchType,
    List<Guid> UserIds
) : IRequest<Guid>;
