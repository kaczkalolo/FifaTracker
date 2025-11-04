using FifaTracker.Domain.Entities;

namespace FifaTracker.Application.Services;

public interface IMatchGenerator
{
    List<Match> GenerateSmartMatches(
        Guid sessionId, 
        List<Guid> userIds, 
        FifaTracker.Domain.Entities.MatchType matchType,
        int targetCount,
        List<Match> existingMatches,
        Dictionary<Guid, DateTime> userJoinTimes,
        DateTime sessionStartTime);
}
