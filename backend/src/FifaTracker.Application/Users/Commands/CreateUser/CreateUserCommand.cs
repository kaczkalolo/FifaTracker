using MediatR;

namespace FifaTracker.Application.Users.Commands.CreateUser;

public record CreateUserCommand(string Name) : IRequest<Guid>;
