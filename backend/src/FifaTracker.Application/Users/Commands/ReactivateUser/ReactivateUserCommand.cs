using MediatR;

namespace FifaTracker.Application.Users.Commands.ReactivateUser;

public record ReactivateUserCommand(Guid Id) : IRequest<Unit>;
