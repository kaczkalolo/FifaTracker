using MediatR;

namespace FifaTracker.Application.Matches.Commands.CreateCustomMatch;

public record CreateCustomMatchCommand(
    Guid SessionId,
    List<Guid> Team1UserIds,
    List<Guid> Team2UserIds
) : IRequest<Guid>;
