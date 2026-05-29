using Futelo.Client.Services.Language;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public record MatchResultInput(
    int HomeScore,
    int AwayScore,
    string? WonOnPenaltiesId,
    int? HomePenaltyScore,
    int? AwayPenaltyScore);

public partial class MatchResultForm
{
    [Inject] private ILanguageService Lang { get; set; } = null!;

    [Parameter] public bool HasPenalties { get; set; }
    [Parameter] public bool IsHomeAndAway { get; set; }
    [Parameter] public bool IsLeg2 { get; set; }
    [Parameter] public int? OtherLegHomeScore { get; set; }
    [Parameter] public int? OtherLegAwayScore { get; set; }
    [Parameter] public string HomePlayerId { get; set; } = string.Empty;
    [Parameter] public string HomePlayerName { get; set; } = string.Empty;
    [Parameter] public string AwayPlayerId { get; set; } = string.Empty;
    [Parameter] public string AwayPlayerName { get; set; } = string.Empty;
    [Parameter] public bool IsSaving { get; set; }
    [Parameter] public int? InitialHomeScore { get; set; }
    [Parameter] public int? InitialAwayScore { get; set; }
    [Parameter] public string? InitialWonOnPenaltiesId { get; set; }
    [Parameter] public int? InitialHomePenaltyScore { get; set; }
    [Parameter] public int? InitialAwayPenaltyScore { get; set; }
    [Parameter] public EventCallback<MatchResultInput> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private int homeScore;
    private int awayScore;
    private int? homePenaltyScore;
    private int? awayPenaltyScore;

    protected override void OnInitialized()
    {
        homeScore = InitialHomeScore ?? 0;
        awayScore = InitialAwayScore ?? 0;
        homePenaltyScore = InitialHomePenaltyScore;
        awayPenaltyScore = InitialAwayPenaltyScore;
    }

    private bool ShowPenaltyFields
    {
        get
        {
            if (!IsHomeAndAway)
                return homeScore == awayScore;

            if (!IsLeg2 || OtherLegHomeScore == null) return false;
            int aGoals = OtherLegHomeScore.Value + awayScore;
            int bGoals = (OtherLegAwayScore ?? 0) + homeScore;
            return aGoals == bGoals;
        }
    }

    private async Task HandleSave()
    {
        string? wonOnPenaltiesId = null;
        int? finalHome = null;
        int? finalAway = null;

        if (HasPenalties && ShowPenaltyFields
            && homePenaltyScore.HasValue && awayPenaltyScore.HasValue
            && homePenaltyScore != awayPenaltyScore)
        {
            wonOnPenaltiesId = homePenaltyScore > awayPenaltyScore ? HomePlayerId : AwayPlayerId;
            finalHome = homePenaltyScore;
            finalAway = awayPenaltyScore;
        }

        await OnSave.InvokeAsync(new MatchResultInput(
            homeScore,
            awayScore,
            wonOnPenaltiesId,
            finalHome,
            finalAway));
    }

    private void HandleCancel() => OnCancel.InvokeAsync();
}
