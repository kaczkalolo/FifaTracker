using MediatR;

namespace FifaTracker.Application.Users.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<List<UserDto>>;

public record UserDto(Guid Id, string Name, DateTime CreatedAt);
