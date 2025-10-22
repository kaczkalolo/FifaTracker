using MediatR;

namespace FifaTracker.Application.Sessions.Queries.GetActiveSessions;

public record GetActiveSessionsQuery : IRequest<List<SessionSummaryDto>>;

public record SessionSummaryDto(
    Guid Id,
    string Name,
    DateTime StartDate,
    string Status,
    string MatchType,
    int TotalMatches,
    int CompletedMatches,
    int ParticipantCount
);
