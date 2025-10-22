namespace FifaTracker.Domain.Entities;

// Represents a user's participation in a specific team in a match
public class MatchTeam
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public Match Match { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int TeamNumber { get; set; } // 1 or 2
}
