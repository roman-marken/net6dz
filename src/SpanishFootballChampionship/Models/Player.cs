namespace SpanishFootballChampionship.Models;

public class Player
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Position { get; set; } = string.Empty;

    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public ICollection<Goal> Goals { get; set; } = new List<Goal>();
}
