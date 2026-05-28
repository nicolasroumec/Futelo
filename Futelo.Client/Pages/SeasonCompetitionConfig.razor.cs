using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Season;
using Futelo.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class SeasonCompetitionConfig : LocalizedComponentBase
{
    [Parameter] public ConfigureSeasonRequest Model { get; set; } = new();

    private void MoveCriterionUp(int index)
    {
        if (index <= 0) return;
        (Model.LeagueTiebreakerCriteria[index], Model.LeagueTiebreakerCriteria[index - 1]) =
            (Model.LeagueTiebreakerCriteria[index - 1], Model.LeagueTiebreakerCriteria[index]);
    }

    private void MoveCriterionDown(int index)
    {
        if (index >= Model.LeagueTiebreakerCriteria.Count - 1) return;
        (Model.LeagueTiebreakerCriteria[index], Model.LeagueTiebreakerCriteria[index + 1]) =
            (Model.LeagueTiebreakerCriteria[index + 1], Model.LeagueTiebreakerCriteria[index]);
    }

    private string CriterionNameKey(TiebreakerCriterion c) => c switch
    {
        TiebreakerCriterion.HeadToHeadPoints         => "season.tiebreaker.h2hPoints",
        TiebreakerCriterion.HeadToHeadWins           => "season.tiebreaker.h2hWins",
        TiebreakerCriterion.HeadToHeadGoalDifference => "season.tiebreaker.h2hGD",
        TiebreakerCriterion.HeadToHeadGoalsFor       => "season.tiebreaker.h2hGF",
        TiebreakerCriterion.GoalDifference           => "season.tiebreaker.goalDifference",
        TiebreakerCriterion.GoalsFor                 => "season.tiebreaker.goalsFor",
        TiebreakerCriterion.Wins                     => "season.tiebreaker.wins",
        _                                            => c.ToString()
    };

    private string CriterionDescKey(TiebreakerCriterion c) => CriterionNameKey(c) + ".desc";
}
