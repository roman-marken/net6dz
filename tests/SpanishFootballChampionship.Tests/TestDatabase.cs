using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SpanishFootballChampionship.Data;
using SpanishFootballChampionship.Models;

namespace SpanishFootballChampionship.Tests;

public sealed class TestDatabase : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    public ChampionshipDbContext Db { get; }

    public TestDatabase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ChampionshipDbContext>()
            .UseSqlite(_connection)
            .Options;

        Db = new ChampionshipDbContext(options);
        Db.Database.EnsureCreated();
        CreateViews();
        Seed();
    }

    public ValueTask DisposeAsync()
    {
        Db.Dispose();
        return _connection.DisposeAsync();
    }

    private void Seed()
    {
        var lions = new Team { Name = "Lions", City = "Madrid" };
        var bears = new Team { Name = "Bears", City = "Barcelona" };
        var wolves = new Team { Name = "Wolves", City = "Sevilla" };

        lions.Players.Add(new Player { FullName = "Lion Forward", Number = 9, Position = "Forward" });
        lions.Players.Add(new Player { FullName = "Lion Midfielder", Number = 10, Position = "Midfielder" });

        bears.Players.Add(new Player { FullName = "Bear Forward", Number = 9, Position = "Forward" });
        bears.Players.Add(new Player { FullName = "Bear Midfielder", Number = 10, Position = "Midfielder" });

        wolves.Players.Add(new Player { FullName = "Wolf Forward", Number = 9, Position = "Forward" });
        wolves.Players.Add(new Player { FullName = "Wolf Midfielder", Number = 10, Position = "Midfielder" });

        Db.Teams.AddRange(lions, bears, wolves);
        Db.SaveChanges();

        AddMatch(lions, bears, 2, 1, [lions.Players.ElementAt(0), lions.Players.ElementAt(0), bears.Players.ElementAt(0)]);
        AddMatch(lions, wolves, 0, 3, [wolves.Players.ElementAt(0), wolves.Players.ElementAt(0), wolves.Players.ElementAt(1)]);
        AddMatch(bears, wolves, 1, 1, [bears.Players.ElementAt(1), wolves.Players.ElementAt(1)]);

        Db.SaveChanges();
    }

    private void AddMatch(Team home, Team away, int homeScore, int awayScore, Player[] scorers)
    {
        var match = new Match
        {
            HomeTeamId = home.Id,
            AwayTeamId = away.Id,
            PlayedAt = DateTime.UtcNow,
            HomeScore = homeScore,
            AwayScore = awayScore
        };

        Db.Matches.Add(match);
        Db.SaveChanges();

        for (var i = 0; i < scorers.Length; i++)
        {
            Db.Goals.Add(new Goal
            {
                MatchId = match.Id,
                PlayerId = scorers[i].Id,
                TeamId = scorers[i].TeamId,
                Minute = 10 + i
            });
        }
    }

    private void CreateViews()
    {
        Db.Database.ExecuteSqlRaw("""
            CREATE VIEW View_PlayerScoringStats AS
            SELECT
                p.Id AS PlayerId,
                p.FullName AS PlayerName,
                t.Id AS TeamId,
                t.Name AS TeamName,
                COUNT(g.Id) AS Goals
            FROM Players AS p
            INNER JOIN Teams AS t ON t.Id = p.TeamId
            LEFT JOIN Goals AS g ON g.PlayerId = p.Id
            GROUP BY p.Id, p.FullName, t.Id, t.Name;
            """);

        Db.Database.ExecuteSqlRaw("""
            CREATE VIEW View_TeamGoalStats AS
            SELECT
                t.Id AS TeamId,
                t.Name AS TeamName,
                COALESCE(SUM(CASE
                    WHEN m.HomeTeamId = t.Id THEN m.HomeScore
                    WHEN m.AwayTeamId = t.Id THEN m.AwayScore
                    ELSE 0
                END), 0) AS GoalsFor,
                COALESCE(SUM(CASE
                    WHEN m.HomeTeamId = t.Id THEN m.AwayScore
                    WHEN m.AwayTeamId = t.Id THEN m.HomeScore
                    ELSE 0
                END), 0) AS GoalsAgainst
            FROM Teams AS t
            LEFT JOIN Matches AS m ON m.HomeTeamId = t.Id OR m.AwayTeamId = t.Id
            GROUP BY t.Id, t.Name;
            """);

        Db.Database.ExecuteSqlRaw("""
            CREATE VIEW View_TeamPointStats AS
            SELECT
                t.Id AS TeamId,
                t.Name AS TeamName,
                COALESCE(SUM(CASE
                    WHEN m.HomeTeamId = t.Id AND m.HomeScore > m.AwayScore THEN 1
                    WHEN m.AwayTeamId = t.Id AND m.AwayScore > m.HomeScore THEN 1
                    ELSE 0
                END), 0) AS Wins,
                COALESCE(SUM(CASE
                    WHEN m.Id IS NOT NULL AND m.HomeScore = m.AwayScore THEN 1
                    ELSE 0
                END), 0) AS Draws,
                COALESCE(SUM(CASE
                    WHEN m.HomeTeamId = t.Id AND m.HomeScore < m.AwayScore THEN 1
                    WHEN m.AwayTeamId = t.Id AND m.AwayScore < m.HomeScore THEN 1
                    ELSE 0
                END), 0) AS Losses,
                COALESCE(SUM(CASE
                    WHEN m.HomeTeamId = t.Id AND m.HomeScore > m.AwayScore THEN 3
                    WHEN m.AwayTeamId = t.Id AND m.AwayScore > m.HomeScore THEN 3
                    WHEN m.Id IS NOT NULL AND m.HomeScore = m.AwayScore THEN 1
                    ELSE 0
                END), 0) AS Points
            FROM Teams AS t
            LEFT JOIN Matches AS m ON m.HomeTeamId = t.Id OR m.AwayTeamId = t.Id
            GROUP BY t.Id, t.Name;
            """);
    }
}
