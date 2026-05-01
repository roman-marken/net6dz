using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SpanishFootballChampionship.Data;
using SpanishFootballChampionship.Services;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

var options = new DbContextOptionsBuilder<ChampionshipDbContext>()
    .UseSqlServer(connectionString)
    .Options;

await using var db = new ChampionshipDbContext(options);
await DbInitializer.InitializeAsync(db);

var service = new ChampionshipService(db);

Console.WriteLine("Spanish Football Championship");
Console.WriteLine("1. Generate random matches");
Console.WriteLine("2. Show reports");
Console.Write("Choose action: ");

var choice = Console.ReadLine();

if (choice == "1")
{
    await service.FillMatchesRandomAsync(matchesPerPair: 2);
    Console.WriteLine("Random matches were generated.");
    return;
}

var teams = await db.Teams.OrderBy(x => x.Name).ToListAsync();
var firstTeam = teams.FirstOrDefault();

if (firstTeam is null)
{
    Console.WriteLine("No teams found.");
    return;
}

Console.WriteLine();
Console.WriteLine($"Top-3 scorers of {firstTeam.Name}:");
foreach (var scorer in await service.GetTopThreeScorersByTeamAsync(firstTeam.Id))
{
    Console.WriteLine($"{scorer.PlayerName} - {scorer.Goals}");
}

Console.WriteLine();
Console.WriteLine("Best scorer of the championship:");
var bestScorer = await service.GetBestScorerAsync();
Console.WriteLine(bestScorer is null ? "No data" : $"{bestScorer.PlayerName} ({bestScorer.TeamName}) - {bestScorer.Goals}");

Console.WriteLine();
Console.WriteLine("Top-3 teams by scored goals:");
foreach (var team in await service.GetTopThreeTeamsByGoalsForAsync())
{
    Console.WriteLine($"{team.TeamName} - {team.GoalsFor}");
}

Console.WriteLine();
Console.WriteLine("Top-3 teams by least conceded goals:");
foreach (var team in await service.GetTopThreeTeamsByGoalsAgainstAsync())
{
    Console.WriteLine($"{team.TeamName} - {team.GoalsAgainst}");
}

Console.WriteLine();
Console.WriteLine("Top-3 teams by points:");
foreach (var team in await service.GetTopThreeTeamsByPointsAsync())
{
    Console.WriteLine($"{team.TeamName} - {team.Points}");
}

Console.WriteLine();
Console.WriteLine("Bottom-3 teams by points:");
foreach (var team in await service.GetBottomThreeTeamsByPointsAsync())
{
    Console.WriteLine($"{team.TeamName} - {team.Points}");
}
