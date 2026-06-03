using Futelo.Shared.Enums;

namespace Futelo.Shared.DTOs.Season;

public class ConfigureSeasonRequest
{
    public bool HasLeague { get; set; }
    public string LeagueName { get; set; } = "League";
    public bool LeagueIsHomeAndAway { get; set; }
    public List<TiebreakerCriterion> LeagueTiebreakerCriteria { get; set; } =
    [
        TiebreakerCriterion.HeadToHeadPoints,
        TiebreakerCriterion.HeadToHeadWins,
        TiebreakerCriterion.HeadToHeadGoalDifference,
        TiebreakerCriterion.HeadToHeadGoalsFor,
        TiebreakerCriterion.GoalDifference,
        TiebreakerCriterion.GoalsFor,
        TiebreakerCriterion.Wins
    ];
    public FinalTiebreaker LeagueFinalTiebreaker { get; set; } = FinalTiebreaker.DrawingOfLots;
    public DateTime? LeagueStartDate { get; set; }
    public DateTime? LeagueEndDate { get; set; }
    public bool HasCup { get; set; }
    public string CupName { get; set; } = "Cup";
    public bool CupIsHomeAndAway { get; set; }
    public CupSeedingMode CupSeedingMode { get; set; } = CupSeedingMode.SeasonElo;
    public bool CupAwayGoalRule { get; set; }
    public DateTime? CupStartDate { get; set; }
    public DateTime? CupEndDate { get; set; }
    public bool HasSuperCup { get; set; }
    public string SuperCupName { get; set; } = "SuperCup";
    public DateTime? SuperCupStartDate { get; set; }
    public DateTime? SuperCupEndDate { get; set; }
    public List<string> PlayerIds { get; set; } = [];
}
