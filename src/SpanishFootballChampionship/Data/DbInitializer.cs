using Microsoft.EntityFrameworkCore;
using SpanishFootballChampionship.Models;
using SpanishFootballChampionship.Sql;

namespace SpanishFootballChampionship.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ChampionshipDbContext db)
    {
        await db.Database.EnsureCreatedAsync();
        await CreateViewsAndProceduresAsync(db);

        if (await db.Teams.AnyAsync())
        {
            return;
        }

        var teams = CreateSeedTeams();
        db.Teams.AddRange(teams);
        await db.SaveChangesAsync();
    }

    public static async Task CreateViewsAndProceduresAsync(ChampionshipDbContext db)
    {
        foreach (var script in SqlServerScripts.All)
        {
            await db.Database.ExecuteSqlRawAsync(script);
        }
    }

    private static Team[] CreateSeedTeams()
    {
        return
        [
            CreateTeam("Real Madrid", "Madrid", ["Vinicius Junior", "Jude Bellingham", "Rodrygo", "Joselu"]),
            CreateTeam("Barcelona", "Barcelona", ["Robert Lewandowski", "Pedri", "Ferran Torres", "Raphinha"]),
            CreateTeam("Atletico Madrid", "Madrid", ["Antoine Griezmann", "Alvaro Morata", "Memphis Depay", "Angel Correa"]),
            CreateTeam("Sevilla", "Sevilla", ["Youssef En-Nesyri", "Lucas Ocampos", "Suso", "Rafa Mir"]),
            CreateTeam("Real Sociedad", "San Sebastian", ["Mikel Oyarzabal", "Takefusa Kubo", "Brais Mendez", "Umar Sadiq"])
        ];
    }

    private static Team CreateTeam(string name, string city, string[] players)
    {
        var team = new Team
        {
            Name = name,
            City = city
        };

        for (var i = 0; i < players.Length; i++)
        {
            team.Players.Add(new Player
            {
                FullName = players[i],
                Number = i + 7,
                Position = i == 0 ? "Forward" : "Midfielder"
            });
        }

        return team;
    }
}
