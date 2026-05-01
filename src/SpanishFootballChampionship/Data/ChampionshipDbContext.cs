using Microsoft.EntityFrameworkCore;
using SpanishFootballChampionship.Models;

namespace SpanishFootballChampionship.Data;

public class ChampionshipDbContext : DbContext
{
    public ChampionshipDbContext(DbContextOptions<ChampionshipDbContext> options)
        : base(options)
    {
    }

    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Goal> Goals => Set<Goal>();

    public DbSet<PlayerScoringStat> PlayerScoringStats => Set<PlayerScoringStat>();
    public DbSet<TeamGoalStat> TeamGoalStats => Set<TeamGoalStat>();
    public DbSet<TeamPointStat> TeamPointStats => Set<TeamPointStat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Team>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.City).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.Property(x => x.FullName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Position).HasMaxLength(40).IsRequired();
            entity.HasOne(x => x.Team)
                .WithMany(x => x.Players)
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasOne(x => x.HomeTeam)
                .WithMany(x => x.HomeMatches)
                .HasForeignKey(x => x.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.AwayTeam)
                .WithMany(x => x.AwayMatches)
                .HasForeignKey(x => x.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasOne(x => x.Match)
                .WithMany(x => x.Goals)
                .HasForeignKey(x => x.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Player)
                .WithMany(x => x.Goals)
                .HasForeignKey(x => x.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Team)
                .WithMany()
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PlayerScoringStat>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("View_PlayerScoringStats");
        });

        modelBuilder.Entity<TeamGoalStat>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("View_TeamGoalStats");
        });

        modelBuilder.Entity<TeamPointStat>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("View_TeamPointStats");
        });
    }
}
