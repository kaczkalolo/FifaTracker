using MediatR;

namespace FifaTracker.Application.Sessions.Commands.EndSession;

public record EndSessionCommand(Guid SessionId) : IRequest<Unit>;
