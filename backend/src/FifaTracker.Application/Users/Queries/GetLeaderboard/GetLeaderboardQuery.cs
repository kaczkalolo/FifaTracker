using MediatR;

namespace FifaTracker.Application.Users.Queries.GetLeaderboard;

public record GetLeaderboardQuery : IRequest<List<LeaderboardEntryDto>>;
