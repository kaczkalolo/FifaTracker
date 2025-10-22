namespace FifaTracker.Application.Users.Queries.GetInactiveUsers;

public record InactiveUserDto(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    DateTime? DeactivatedAt
);
