using FifaTracker.Application.Sessions.Queries.GetActiveSessions;
using FifaTracker.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FifaTracker.Application.Sessions.Queries.GetAllSessions;

public class GetAllSessionsQueryHandler : IRequestHandler<GetAllSessionsQuery, List<SessionSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllSessionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SessionSummaryDto>> Handle(GetAllSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _context.Sessions
            .Include(s => s.SessionUsers)
            .Include(s => s.Matches)
            .OrderByDescending(s => s.StartDate)
            .ToListAsync(cancellationToken);

        return sessions.Select(s => new SessionSummaryDto(
            s.Id,
            s.Name,
            s.StartDate,
            s.Status.ToString(),
            s.MatchType.ToString(),
            s.Matches.Count,
            s.Matches.Count(m => m.IsCompleted),
            s.SessionUsers.Count
        )).ToList();
    }
}
