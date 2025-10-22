namespace FifaTracker.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? DeactivatedAt { get; set; }

    // Navigation properties
    public ICollection<SessionUser> SessionUsers { get; set; } = new List<SessionUser>();
    public ICollection<MatchTeam> MatchTeams { get; set; } = new List<MatchTeam>();
}
