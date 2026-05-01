using Microsoft.EntityFrameworkCore;
using SpanishFootballChampionship.Data;
using SpanishFootballChampionship.Models;

namespace SpanishFootballChampionship.Services;

public class ChampionshipService
{
    private readonly ChampionshipDbContext _db;

    public ChampionshipService(ChampionshipDbContext db)
    {
        _db = db;
    }

    public Task FillMatchesRandomAsync(int matchesPerPair = 1, bool clearExisting = true)
    {
        return _db.Database.ExecuteSqlInterpolatedAsync(
            $"EXEC dbo.sp_FillMatchesRandom @MatchesPerPair = {matchesPerPair}, @ClearExisting = {clearExisting}");
    }

    public Task<List<PlayerScoringStat>> GetTopThreeScorersByTeamAsync(int teamId)
    {
        return _db.PlayerScoringStats
            .Where(x => x.TeamId == teamId)
            .OrderByDescending(x => x.Goals)
            .ThenBy(x => x.PlayerName)
            .Take(3)
            .ToListAsync();
    }

    public Task<PlayerScoringStat?> GetBestScorerByTeamAsync(int teamId)
    {
        return _db.PlayerScoringStats
            .Where(x => x.TeamId == teamId)
            .OrderByDescending(x => x.Goals)
            .ThenBy(x => x.PlayerName)
            .FirstOrDefaultAsync();
    }

    public Task<List<PlayerScoringStat>> GetTopThreeScorersAsync()
    {
        return _db.PlayerScoringStats
            .OrderByDescending(x => x.Goals)
            .ThenBy(x => x.PlayerName)
            .Take(3)
            .ToListAsync();
    }

    public Task<PlayerScoringStat?> GetBestScorerAsync()
    {
        return _db.PlayerScoringStats
            .OrderByDescending(x => x.Goals)
            .ThenBy(x => x.PlayerName)
            .FirstOrDefaultAsync();
    }

    public Task<List<TeamGoalStat>> GetTopThreeTeamsByGoalsForAsync()
    {
        return _db.TeamGoalStats
            .OrderByDescending(x => x.GoalsFor)
            .ThenBy(x => x.TeamName)
            .Take(3)
            .ToListAsync();
    }

    public Task<TeamGoalStat?> GetBestTeamByGoalsForAsync()
    {
        return _db.TeamGoalStats
            .OrderByDescending(x => x.GoalsFor)
            .ThenBy(x => x.TeamName)
            .FirstOrDefaultAsync();
    }

    public Task<List<TeamGoalStat>> GetTopThreeTeamsByGoalsAgainstAsync()
    {
        return _db.TeamGoalStats
            .OrderBy(x => x.GoalsAgainst)
            .ThenBy(x => x.TeamName)
            .Take(3)
            .ToListAsync();
    }

    public Task<TeamGoalStat?> GetBestTeamByGoalsAgainstAsync()
    {
        return _db.TeamGoalStats
            .OrderBy(x => x.GoalsAgainst)
            .ThenBy(x => x.TeamName)
            .FirstOrDefaultAsync();
    }

    public Task<List<TeamPointStat>> GetTopThreeTeamsByPointsAsync()
    {
        return _db.TeamPointStats
            .OrderByDescending(x => x.Points)
            .ThenBy(x => x.TeamName)
            .Take(3)
            .ToListAsync();
    }

    public Task<TeamPointStat?> GetBestTeamByPointsAsync()
    {
        return _db.TeamPointStats
            .OrderByDescending(x => x.Points)
            .ThenBy(x => x.TeamName)
            .FirstOrDefaultAsync();
    }

    public Task<List<TeamPointStat>> GetBottomThreeTeamsByPointsAsync()
    {
        return _db.TeamPointStats
            .OrderBy(x => x.Points)
            .ThenBy(x => x.TeamName)
            .Take(3)
            .ToListAsync();
    }

    public Task<TeamPointStat?> GetWorstTeamByPointsAsync()
    {
        return _db.TeamPointStats
            .OrderBy(x => x.Points)
            .ThenBy(x => x.TeamName)
            .FirstOrDefaultAsync();
    }
}
