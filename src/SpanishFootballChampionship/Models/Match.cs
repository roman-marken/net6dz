namespace SpanishFootballChampionship.Models;

public class Match
{
    public int Id { get; set; }
    public DateTime PlayedAt { get; set; }

    public int HomeTeamId { get; set; }
    public Team HomeTeam { get; set; } = null!;

    public int AwayTeamId { get; set; }
    public Team AwayTeam { get; set; } = null!;

    public int HomeScore { get; set; }
    public int AwayScore { get; set; }

    public ICollection<Goal> Goals { get; set; } = new List<Goal>();
}
