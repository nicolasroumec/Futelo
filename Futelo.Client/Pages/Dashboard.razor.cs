using Futelo.Client.Services.Language;
using Futelo.Client.Services.Vault;
using Futelo.Shared.DTOs.Vault;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class Dashboard : IDisposable
{
    [Inject] private IVaultService VaultService { get; set; } = null!;
    [Inject] private ILanguageService Lang { get; set; } = null!;

    private List<VaultResponse> vaults = [];
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        Lang.OnChange += HandleLanguageChange;

        try
        {
            vaults = await VaultService.GetAllAsync();
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

    public void Dispose()
    {
        Lang.OnChange -= HandleLanguageChange;
    }
}
