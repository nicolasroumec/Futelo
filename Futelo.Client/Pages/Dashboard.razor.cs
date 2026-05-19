using Futelo.Client.Services.Vault;
using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Vault;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class Dashboard : LocalizedComponentBase
{
    [Inject] private IVaultService VaultService { get; set; } = null!;

    private List<VaultResponse> vaults = [];
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            vaults = await VaultService.GetAllAsync(ComponentToken);
        }
        catch (OperationCanceledException) { }
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
