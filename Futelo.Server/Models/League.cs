using Futelo.Shared.Enums;

namespace Futelo.Server.Models;

public class League
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public string Name { get; set; } = "League";
    public bool IsHomeAndAway { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<TiebreakerCriterion> TiebreakerCriteria { get; set; } = DefaultCriteria();
    public FinalTiebreaker FinalTiebreaker { get; set; } = FinalTiebreaker.DrawingOfLots;
    public TournamentStatus Status { get; set; } = TournamentStatus.NotStarted;

    public string? ChampionId { get; set; }

    public Season Season { get; set; } = null!;
    public AppUser? Champion { get; set; }
    public ICollection<LeaguePlayer> Players { get; set; } = [];
    public ICollection<Match> Matches { get; set; } = [];

    public static List<TiebreakerCriterion> DefaultCriteria() =>
    [
        TiebreakerCriterion.HeadToHeadPoints,
        TiebreakerCriterion.HeadToHeadWins,
        TiebreakerCriterion.HeadToHeadGoalDifference,
        TiebreakerCriterion.HeadToHeadGoalsFor,
        TiebreakerCriterion.GoalDifference,
        TiebreakerCriterion.GoalsFor,
        TiebreakerCriterion.Wins
    ];
}
