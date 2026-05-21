using Futelo.Shared.Enums;

namespace Futelo.Server.Models;

public class Match
{
    public int Id { get; set; }
    public string? HomePlayerId { get; set; }
    public string? AwayPlayerId { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public string? WonOnPenaltiesId { get; set; }
    public int? HomePenaltyScore { get; set; }
    public int? AwayPenaltyScore { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Pending;
    public int Leg { get; set; } = 1;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? PlayedAt { get; set; }
    public int? VideoGameId { get; set; }
    public int? HomeTeamId { get; set; }
    public int? AwayTeamId { get; set; }
    public int? LeagueId { get; set; }
    public int? CupRoundId { get; set; }
    public int? SuperCupId { get; set; }

    public AppUser? HomePlayer { get; set; }
    public AppUser? AwayPlayer { get; set; }
    public AppUser? WonOnPenalties { get; set; }
    public VideoGame? VideoGame { get; set; }
    public Team? HomeTeam { get; set; }
    public Team? AwayTeam { get; set; }
    public League? League { get; set; }
    public CupRound? CupRound { get; set; }
    public SuperCup? SuperCup { get; set; }
}
