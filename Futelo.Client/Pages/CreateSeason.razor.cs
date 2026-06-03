using Futelo.Client.Services.Season;
using Futelo.Client.Services.Vault;
using Futelo.Client.Services.VideoGames;
using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Season;
using Futelo.Shared.DTOs.VideoGame;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class CreateSeason : LocalizedComponentBase
{
    [Parameter] public int VaultId { get; set; }
    [Inject] private ISeasonService SeasonService { get; set; } = null!;
    [Inject] private IVaultService VaultService { get; set; } = null!;
    [Inject] private IVideoGameService VideoGameService { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private CreateSeasonRequest model = new();
    private List<VideoGameResponse> videoGames = [];
    private List<SeasonResponse> previousSeasons = [];
    private HashSet<string> vaultPlayerIds = [];
    private SeasonResponse? selectedSourceSeason;
    private string? errorMessage;
    private bool isLoading;

    protected override async Task OnParametersSetAsync()
    {
        model.VaultId = VaultId;
        model.Year = DateTime.UtcNow.Year;

        var videoGamesTask = VideoGameService.GetAllAsync(ComponentToken);
        var seasonsTask = SeasonService.GetByVaultAsync(VaultId, ComponentToken);
        var vaultTask = VaultService.GetByIdAsync(VaultId, ComponentToken);
        await Task.WhenAll(videoGamesTask, seasonsTask, vaultTask);

        videoGames = videoGamesTask.Result;
        previousSeasons = seasonsTask.Result
            .Where(s => s.Status is "Finished" or "Active")
            .OrderByDescending(s => s.Year)
            .ThenByDescending(s => s.Id)
            .ToList();
        vaultPlayerIds = vaultTask.Result.Players.Select(p => p.PlayerId).ToHashSet();
    }

    private void OnSourceSeasonChanged(ChangeEventArgs e)
    {
        if (!int.TryParse(e.Value?.ToString(), out var id) || id == 0)
        {
            selectedSourceSeason = null;
            model.VideoGameId = null;
            return;
        }
        selectedSourceSeason = previousSeasons.FirstOrDefault(s => s.Id == id);
        model.VideoGameId = selectedSourceSeason?.VideoGameId;
    }

    private async Task HandleCreate()
    {
        isLoading = true;
        errorMessage = null;
        try
        {
            var season = await SeasonService.CreateAsync(model);
            if (selectedSourceSeason is { } src)
            {
                var clonedPlayers = src.Players
                    .Select(p => p.PlayerId)
                    .Where(pid => vaultPlayerIds.Contains(pid))
                    .ToList();

                if (clonedPlayers.Count >= 2 && (src.HasLeague || src.HasCup))
                {
                    await SeasonService.ConfigureAsync(season.Id, new ConfigureSeasonRequest
                    {
                        PlayerIds = clonedPlayers,
                        HasLeague = src.HasLeague,
                        LeagueName = src.LeagueName,
                        LeagueIsHomeAndAway = src.LeagueIsHomeAndAway,
                        LeagueTiebreakerCriteria = [.. src.LeagueTiebreakerCriteria],
                        LeagueFinalTiebreaker = src.LeagueFinalTiebreaker,
                        HasCup = src.HasCup,
                        CupName = src.CupName,
                        CupIsHomeAndAway = src.CupIsHomeAndAway,
                        CupSeedingMode = src.CupSeedingMode,
                        CupAwayGoalRule = src.CupAwayGoalRule,
                        HasSuperCup = src.HasSuperCup,
                        SuperCupName = src.SuperCupName
                    });
                }
            }
            Nav.NavigateTo($"/seasons/{season.Id}");
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isLoading = false;
        }
    }
}
