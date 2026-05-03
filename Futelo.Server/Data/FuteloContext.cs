using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Futelo.Server.Models;

namespace Futelo.Server.Data;

public class FuteloContext : IdentityDbContext<AppUser>
{
    public FuteloContext(DbContextOptions<FuteloContext> options) : base(options) { }

    public DbSet<Vault> Vaults { get; set; } = null!;
    public DbSet<VaultPlayer> VaultPlayers { get; set; } = null!;
    public DbSet<VaultInvitation> VaultInvitations { get; set; } = null!;
    public DbSet<Season> Seasons { get; set; } = null!;
    public DbSet<SeasonPlayer> SeasonPlayers { get; set; } = null!;
    public DbSet<League> Leagues { get; set; } = null!;
    public DbSet<LeaguePlayer> LeaguePlayers { get; set; } = null!;
    public DbSet<Cup> Cups { get; set; } = null!;
    public DbSet<CupPlayer> CupPlayers { get; set; } = null!;
    public DbSet<CupRound> CupRounds { get; set; } = null!;
    public DbSet<SuperCup> SuperCups { get; set; } = null!;
    public DbSet<Match> Matches { get; set; } = null!;
    public DbSet<EloHistory> EloHistories { get; set; } = null!;
    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<VideoGame> VideoGames { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<VaultPlayer>()
            .HasKey(vp => new { vp.VaultId, vp.PlayerId });

        builder.Entity<SeasonPlayer>()
            .HasKey(sp => new { sp.SeasonId, sp.PlayerId });

        builder.Entity<LeaguePlayer>()
            .HasKey(lp => new { lp.LeagueId, lp.PlayerId });

        builder.Entity<CupPlayer>()
            .HasKey(cp => new { cp.CupId, cp.PlayerId });

        builder.Entity<Season>()
            .HasOne(s => s.League)
            .WithOne(l => l.Season)
            .HasForeignKey<League>(l => l.SeasonId);

        builder.Entity<Season>()
            .HasOne(s => s.Cup)
            .WithOne(c => c.Season)
            .HasForeignKey<Cup>(c => c.SeasonId);

        builder.Entity<Season>()
            .HasOne(s => s.SuperCup)
            .WithOne(sc => sc.Season)
            .HasForeignKey<SuperCup>(sc => sc.SeasonId);

        builder.Entity<Match>()
            .HasOne(m => m.HomePlayer)
            .WithMany()
            .HasForeignKey(m => m.HomePlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Match>()
            .HasOne(m => m.AwayPlayer)
            .WithMany()
            .HasForeignKey(m => m.AwayPlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Match>()
            .HasOne(m => m.WonOnPenalties)
            .WithMany()
            .HasForeignKey(m => m.WonOnPenaltiesId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Match>()
            .HasOne(m => m.HomeTeam)
            .WithMany()
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Match>()
            .HasOne(m => m.AwayTeam)
            .WithMany()
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SuperCup>()
            .HasOne(sc => sc.Player1)
            .WithMany()
            .HasForeignKey(sc => sc.Player1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SuperCup>()
            .HasOne(sc => sc.Player2)
            .WithMany()
            .HasForeignKey(sc => sc.Player2Id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
