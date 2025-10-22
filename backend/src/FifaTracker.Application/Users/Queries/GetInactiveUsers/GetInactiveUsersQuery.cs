using MediatR;

namespace FifaTracker.Application.Users.Queries.GetInactiveUsers;

public record GetInactiveUsersQuery : IRequest<List<InactiveUserDto>>;
