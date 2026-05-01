CREATE OR ALTER VIEW dbo.View_PlayerScoringStats AS
SELECT
    p.Id AS PlayerId,
    p.FullName AS PlayerName,
    t.Id AS TeamId,
    t.Name AS TeamName,
    COUNT(g.Id) AS Goals
FROM dbo.Players AS p
INNER JOIN dbo.Teams AS t ON t.Id = p.TeamId
LEFT JOIN dbo.Goals AS g ON g.PlayerId = p.Id
GROUP BY p.Id, p.FullName, t.Id, t.Name;
GO

CREATE OR ALTER VIEW dbo.View_TeamGoalStats AS
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
FROM dbo.Teams AS t
LEFT JOIN dbo.Matches AS m ON m.HomeTeamId = t.Id OR m.AwayTeamId = t.Id
GROUP BY t.Id, t.Name;
GO

CREATE OR ALTER VIEW dbo.View_TeamPointStats AS
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
FROM dbo.Teams AS t
LEFT JOIN dbo.Matches AS m ON m.HomeTeamId = t.Id OR m.AwayTeamId = t.Id
GROUP BY t.Id, t.Name;
GO

CREATE OR ALTER PROCEDURE dbo.sp_FillMatchesRandom
    @MatchesPerPair int = 1,
    @ClearExisting bit = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @MatchesPerPair < 1
    BEGIN
        THROW 50001, 'MatchesPerPair must be greater than zero.', 1;
    END;

    IF @ClearExisting = 1
    BEGIN
        DELETE FROM dbo.Goals;
        DELETE FROM dbo.Matches;
    END;

    DECLARE @homeTeamId int;
    DECLARE @awayTeamId int;
    DECLARE @round int;
    DECLARE @homeScore int;
    DECLARE @awayScore int;
    DECLARE @matchId int;
    DECLARE @goalIndex int;
    DECLARE @playerId int;

    DECLARE team_pairs CURSOR LOCAL FAST_FORWARD FOR
        SELECT home.Id, away.Id
        FROM dbo.Teams AS home
        INNER JOIN dbo.Teams AS away ON home.Id < away.Id;

    OPEN team_pairs;
    FETCH NEXT FROM team_pairs INTO @homeTeamId, @awayTeamId;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @round = 1;

        WHILE @round <= @MatchesPerPair
        BEGIN
            SET @homeScore = ABS(CHECKSUM(NEWID())) % 6;
            SET @awayScore = ABS(CHECKSUM(NEWID())) % 6;

            INSERT INTO dbo.Matches (PlayedAt, HomeTeamId, AwayTeamId, HomeScore, AwayScore)
            VALUES (DATEADD(day, ABS(CHECKSUM(NEWID())) % 180, CAST(GETDATE() AS date)),
                    @homeTeamId, @awayTeamId, @homeScore, @awayScore);

            SET @matchId = SCOPE_IDENTITY();
            SET @goalIndex = 1;

            WHILE @goalIndex <= @homeScore
            BEGIN
                SELECT TOP 1 @playerId = Id
                FROM dbo.Players
                WHERE TeamId = @homeTeamId
                ORDER BY NEWID();

                INSERT INTO dbo.Goals (Minute, MatchId, PlayerId, TeamId)
                VALUES (1 + ABS(CHECKSUM(NEWID())) % 90, @matchId, @playerId, @homeTeamId);

                SET @goalIndex += 1;
            END;

            SET @goalIndex = 1;

            WHILE @goalIndex <= @awayScore
            BEGIN
                SELECT TOP 1 @playerId = Id
                FROM dbo.Players
                WHERE TeamId = @awayTeamId
                ORDER BY NEWID();

                INSERT INTO dbo.Goals (Minute, MatchId, PlayerId, TeamId)
                VALUES (1 + ABS(CHECKSUM(NEWID())) % 90, @matchId, @playerId, @awayTeamId);

                SET @goalIndex += 1;
            END;

            SET @round += 1;
        END;

        FETCH NEXT FROM team_pairs INTO @homeTeamId, @awayTeamId;
    END;

    CLOSE team_pairs;
    DEALLOCATE team_pairs;
END;
GO
