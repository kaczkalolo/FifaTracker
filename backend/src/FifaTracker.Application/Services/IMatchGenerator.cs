using FifaTracker.Domain.Entities;

namespace FifaTracker.Application.Services;

public interface IMatchGenerator
{
    List<Match> GenerateMatches(Guid sessionId, List<Guid> userIds, FifaTracker.Domain.Entities.MatchType matchType);
}
