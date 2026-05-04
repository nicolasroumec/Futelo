using Futelo.Client.Services.Vault;
using Futelo.Shared.DTOs.Vault;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class CreateVault
{
    [Inject] private IVaultService VaultService { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private CreateVaultRequest model = new();
    private string? errorMessage;
    private bool isLoading;

    private async Task HandleCreate()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            var vault = await VaultService.CreateAsync(model);
            Nav.NavigateTo($"/vaults/{vault.Id}");
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
