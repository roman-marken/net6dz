namespace SpanishFootballChampionship.Models;

public class TeamGoalStat
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
}
