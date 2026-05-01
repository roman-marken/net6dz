namespace SpanishFootballChampionship.Models;

public class PlayerScoringStat
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int Goals { get; set; }
}
