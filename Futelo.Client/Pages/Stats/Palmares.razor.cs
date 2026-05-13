using Futelo.Client.Services.Language;
using Futelo.Client.Services.Stats;
using Futelo.Shared.DTOs.Stats;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.Stats;

public partial class Palmares : IDisposable
{
    [Parameter] public int VaultId { get; set; }
    [Inject] private IStatsService StatsService { get; set; } = null!;
    [Inject] private ILanguageService Lang { get; set; } = null!;

    private List<PalmaresSeasonRow> rows = [];
    private bool isLoading = true;
    private string? errorMessage;

    private List<(string Name, int Count)> TitlesByPlayer => rows
        .SelectMany(r => new[] { r.LeagueChampion, r.CupChampion, r.SuperCupChampion }
            .Where(c => c != null).Select(c => c!))
        .GroupBy(n => n)
        .Select(g => (g.Key, g.Count()))
        .OrderByDescending(x => x.Item2)
        .ToList();

    private int MaxTitles => TitlesByPlayer.FirstOrDefault().Count;

    protected override async Task OnInitializedAsync()
    {
        Lang.OnChange += HandleLanguageChange;
        try
        {
            rows = await StatsService.GetPalmaresAsync(VaultId);
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

    private void HandleLanguageChange() => InvokeAsync(StateHasChanged);

    private static string? GetTrebleWinner(PalmaresSeasonRow row)
    {
        if (row.LeagueChampion == null || row.CupChampion == null || row.SuperCupChampion == null)
            return null;
        return row.LeagueChampion == row.CupChampion && row.CupChampion == row.SuperCupChampion
            ? row.LeagueChampion
            : null;
    }

    private static string? GetDoubleWinner(PalmaresSeasonRow row)
    {
        if (GetTrebleWinner(row) != null) return null;
        var champions = new[] { row.LeagueChampion, row.CupChampion, row.SuperCupChampion }
            .Where(c => c != null).ToList();
        return champions
            .GroupBy(c => c)
            .Where(g => g.Count() >= 2)
            .Select(g => g.Key)
            .FirstOrDefault();
    }

    public void Dispose() => Lang.OnChange -= HandleLanguageChange;
}
