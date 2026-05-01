namespace SpanishFootballChampionship.Models;

public class TeamPointStat
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }
    public int Points { get; set; }
}
