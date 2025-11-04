using MediatR;

namespace FifaTracker.Application.Sessions.Commands.AddUserToSession;

public record AddUserToSessionCommand(Guid SessionId, Guid UserId) : IRequest<Unit>;
