using Futelo.Client.Services.Language;
using Futelo.Shared.DTOs.League;
using Futelo.Shared.DTOs.Team;
using Futelo.Shared.DTOs.VideoGame;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Shared;

public partial class MatchEditPanel
{
    [Inject] private ILanguageService Lang { get; set; } = null!;

    [Parameter] public bool IsEditing { get; set; }
    [Parameter] public string HomePlayerName { get; set; } = string.Empty;
    [Parameter] public string AwayPlayerName { get; set; } = string.Empty;
    [Parameter] public int? HomeTeamId { get; set; }
    [Parameter] public int? AwayTeamId { get; set; }
    [Parameter] public int? VideoGameId { get; set; }
    [Parameter] public List<TeamResponse> Teams { get; set; } = [];
    [Parameter] public List<VideoGameResponse> VideoGames { get; set; } = [];
    [Parameter] public EventCallback<PatchMatchRequest> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private string editHomeTeamIdStr = "";
    private string editAwayTeamIdStr = "";
    private string editVideoGameIdStr = "";
    private bool isSaving;
    private bool wasEditing;

    protected override void OnParametersSet()
    {
        if (IsEditing && !wasEditing)
        {
            editHomeTeamIdStr = HomeTeamId?.ToString() ?? "";
            editAwayTeamIdStr = AwayTeamId?.ToString() ?? "";
            editVideoGameIdStr = VideoGameId?.ToString() ?? "";
        }
        wasEditing = IsEditing;
    }

    private async Task HandleSave()
    {
        isSaving = true;
        try
        {
            var request = new PatchMatchRequest
            {
                HomeTeamId = string.IsNullOrEmpty(editHomeTeamIdStr) ? null : int.Parse(editHomeTeamIdStr),
                AwayTeamId = string.IsNullOrEmpty(editAwayTeamIdStr) ? null : int.Parse(editAwayTeamIdStr),
                VideoGameId = string.IsNullOrEmpty(editVideoGameIdStr) ? null : int.Parse(editVideoGameIdStr),
            };
            await OnSave.InvokeAsync(request);
        }
        finally
        {
            isSaving = false;
        }
    }

    private void HandleCancel() => OnCancel.InvokeAsync();
}
