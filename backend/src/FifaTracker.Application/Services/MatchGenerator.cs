using FifaTracker.Domain.Entities;

namespace FifaTracker.Application.Services;

public class MatchGenerator : IMatchGenerator
{
    public List<Match> GenerateMatches(Guid sessionId, List<Guid> userIds, FifaTracker.Domain.Entities.MatchType matchType)
    {
        var matches = new List<Match>();

        switch (matchType)
        {
            case FifaTracker.Domain.Entities.MatchType.OneVsOne:
                matches = GenerateOneVsOneMatches(sessionId, userIds);
                break;
            case FifaTracker.Domain.Entities.MatchType.TwoVsTwo:
                matches = GenerateTwoVsTwoMatches(sessionId, userIds);
                break;
            case FifaTracker.Domain.Entities.MatchType.TwoVsOne:
                matches = GenerateTwoVsOneMatches(sessionId, userIds);
                break;
        }

        return matches;
    }

    private List<Match> GenerateOneVsOneMatches(Guid sessionId, List<Guid> userIds)
    {
        var matches = new List<Match>();

        // Generate all possible 1v1 combinations
        for (int i = 0; i < userIds.Count; i++)
        {
            for (int j = i + 1; j < userIds.Count; j++)
            {
                var match = new Match
                {
                    Id = Guid.NewGuid(),
                    SessionId = sessionId,
                    IsGenerated = true,
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    MatchTeams = new List<MatchTeam>
                    {
                        new MatchTeam
                        {
                            Id = Guid.NewGuid(),
                            UserId = userIds[i],
                            TeamNumber = 1
                        },
                        new MatchTeam
                        {
                            Id = Guid.NewGuid(),
                            UserId = userIds[j],
                            TeamNumber = 2
                        }
                    }
                };
                matches.Add(match);
            }
        }

        return matches;
    }

    private List<Match> GenerateTwoVsTwoMatches(Guid sessionId, List<Guid> userIds)
    {
        var matches = new List<Match>();

        if (userIds.Count < 4)
            return matches; // Need at least 4 players for 2v2

        // Generate all possible 2v2 combinations
        var combinations = GetCombinations(userIds, 2);
        var teamCombinations = combinations.ToList();

        for (int i = 0; i < teamCombinations.Count; i++)
        {
            for (int j = i + 1; j < teamCombinations.Count; j++)
            {
                var team1 = teamCombinations[i];
                var team2 = teamCombinations[j];

                // Check if teams don't share players
                if (!team1.Intersect(team2).Any())
                {
                    var match = new Match
                    {
                        Id = Guid.NewGuid(),
                        SessionId = sessionId,
                        IsGenerated = true,
                        IsCompleted = false,
                        CreatedAt = DateTime.UtcNow,
                        MatchTeams = new List<MatchTeam>()
                    };

                    foreach (var userId in team1)
                    {
                        match.MatchTeams.Add(new MatchTeam
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            TeamNumber = 1
                        });
                    }

                    foreach (var userId in team2)
                    {
                        match.MatchTeams.Add(new MatchTeam
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            TeamNumber = 2
                        });
                    }

                    matches.Add(match);
                }
            }
        }

        return matches;
    }

    private List<Match> GenerateTwoVsOneMatches(Guid sessionId, List<Guid> userIds)
    {
        var matches = new List<Match>();

        if (userIds.Count < 3)
            return matches; // Need at least 3 players for 2v1

        // Generate all possible 2v1 combinations
        // Each user can be the solo player
        foreach (var soloPlayer in userIds)
        {
            var teamPlayers = userIds.Where(id => id != soloPlayer).ToList();
            var teamCombinations = GetCombinations(teamPlayers, 2);

            foreach (var team in teamCombinations)
            {
                var match = new Match
                {
                    Id = Guid.NewGuid(),
                    SessionId = sessionId,
                    IsGenerated = true,
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    MatchTeams = new List<MatchTeam>()
                };

                // Team of 3
                foreach (var userId in team)
                {
                    match.MatchTeams.Add(new MatchTeam
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        TeamNumber = 1
                    });
                }

                // Solo player
                match.MatchTeams.Add(new MatchTeam
                {
                    Id = Guid.NewGuid(),
                    UserId = soloPlayer,
                    TeamNumber = 2
                });

                matches.Add(match);
            }
        }

        return matches;
    }

    private IEnumerable<List<T>> GetCombinations<T>(List<T> list, int length)
    {
        if (length == 1) return list.Select(t => new List<T> { t });

        return GetCombinations(list, length - 1)
            .SelectMany((t, i) => list.Skip(list.IndexOf(t.Last()) + 1),
                (t1, t2) => t1.Concat(new List<T> { t2 }).ToList());
    }
}
