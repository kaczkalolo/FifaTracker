using MediatR;

namespace FifaTracker.Application.Users.Commands.DeactivateUser;

public record DeactivateUserCommand(Guid Id) : IRequest<Unit>;
