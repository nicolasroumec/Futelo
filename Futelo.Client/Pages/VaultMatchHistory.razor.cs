using Futelo.Client.Services.Vault;
using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Vault;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class VaultMatchHistory : LocalizedComponentBase
{
    [Parameter] public int Id { get; set; }
    [Inject] private IVaultService VaultService { get; set; } = null!;

    private MatchHistoryPageResponse history = new() { Page = 1, PageSize = 10 };
    private int currentPage = 1;
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        await LoadPage(1);
    }

    private async Task LoadPage(int page)
    {
        isLoading = true;
        errorMessage = null;
        try
        {
            history = await VaultService.GetMatchHistoryAsync(Id, page);
            currentPage = page;
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

    private Task PrevPage() => LoadPage(currentPage - 1);
    private Task NextPage() => LoadPage(currentPage + 1);

    private static string MatchScore(RecentMatchResponse m)
    {
        var score = $"{m.HomeScore} - {m.AwayScore}";
        if (m.WonOnPenaltiesId is not null)
            score += $" ({m.HomePenaltyScore}-{m.AwayPenaltyScore} pen.)";
        return score;
    }
}
