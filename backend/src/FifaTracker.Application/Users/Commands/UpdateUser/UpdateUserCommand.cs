using MediatR;

namespace FifaTracker.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, string Name) : IRequest<Unit>;
