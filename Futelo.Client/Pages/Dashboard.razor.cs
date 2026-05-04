using Futelo.Client.Services.Vault;
using Futelo.Shared.DTOs.Vault;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class Dashboard
{
    [Inject] private IVaultService VaultService { get; set; } = null!;

    private List<VaultResponse> vaults = [];
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
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
}
