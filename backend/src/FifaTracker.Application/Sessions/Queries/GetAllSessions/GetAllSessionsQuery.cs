using FifaTracker.Application.Sessions.Queries.GetActiveSessions;
using MediatR;

namespace FifaTracker.Application.Sessions.Queries.GetAllSessions;

public record GetAllSessionsQuery : IRequest<List<SessionSummaryDto>>;
