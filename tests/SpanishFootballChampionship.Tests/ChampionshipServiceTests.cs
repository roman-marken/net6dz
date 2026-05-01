using SpanishFootballChampionship.Services;

namespace SpanishFootballChampionship.Tests;

public class ChampionshipServiceTests
{
    [Fact]
    public async Task GetTopThreeScorersAsync_ReturnsScorersOrderedByGoals()
    {
        await using var database = new TestDatabase();
        var service = new ChampionshipService(database.Db);

        var result = await service.GetTopThreeScorersAsync();

        Assert.Equal(3, result.Count);
        Assert.Equal("Lion Forward", result[0].PlayerName);
        Assert.Equal(2, result[0].Goals);
        Assert.Equal("Wolf Forward", result[1].PlayerName);
        Assert.Equal(2, result[1].Goals);
        Assert.Equal("Bear Forward", result[2].PlayerName);
        Assert.Equal(1, result[2].Goals);
    }

    [Fact]
    public async Task GetBestScorerByTeamAsync_ReturnsBestPlayerOfSelectedTeam()
    {
        await using var database = new TestDatabase();
        var service = new ChampionshipService(database.Db);
        var lions = database.Db.Teams.Single(x => x.Name == "Lions");

        var result = await service.GetBestScorerByTeamAsync(lions.Id);

        Assert.NotNull(result);
        Assert.Equal("Lion Forward", result.PlayerName);
        Assert.Equal(2, result.Goals);
    }

    [Fact]
    public async Task GetBestTeamByGoalsForAsync_ReturnsTeamWithMostGoals()
    {
        await using var database = new TestDatabase();
        var service = new ChampionshipService(database.Db);

        var result = await service.GetBestTeamByGoalsForAsync();

        Assert.NotNull(result);
        Assert.Equal("Wolves", result.TeamName);
        Assert.Equal(4, result.GoalsFor);
    }

    [Fact]
    public async Task GetBestTeamByGoalsAgainstAsync_ReturnsTeamWithLeastConcededGoals()
    {
        await using var database = new TestDatabase();
        var service = new ChampionshipService(database.Db);

        var result = await service.GetBestTeamByGoalsAgainstAsync();

        Assert.NotNull(result);
        Assert.Equal("Wolves", result.TeamName);
        Assert.Equal(1, result.GoalsAgainst);
    }

    [Fact]
    public async Task GetTopThreeTeamsByPointsAsync_ReturnsTeamsOrderedByPointsDescending()
    {
        await using var database = new TestDatabase();
        var service = new ChampionshipService(database.Db);

        var result = await service.GetTopThreeTeamsByPointsAsync();

        Assert.Equal(["Wolves", "Lions", "Bears"], result.Select(x => x.TeamName));
        Assert.Equal([4, 3, 1], result.Select(x => x.Points));
    }

    [Fact]
    public async Task GetWorstTeamByPointsAsync_ReturnsTeamWithLeastPoints()
    {
        await using var database = new TestDatabase();
        var service = new ChampionshipService(database.Db);

        var result = await service.GetWorstTeamByPointsAsync();

        Assert.NotNull(result);
        Assert.Equal("Bears", result.TeamName);
        Assert.Equal(1, result.Points);
    }
}
