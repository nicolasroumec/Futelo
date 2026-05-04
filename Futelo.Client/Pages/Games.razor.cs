using Futelo.Client.Services.VideoGames;
using Futelo.Shared.DTOs.VideoGame;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class Games
{
    [Inject] private IVideoGameService VideoGameService { get; set; } = null!;

    private List<VideoGameResponse> games = [];
    private bool isLoading = true;
    private string? errorMessage;

    private bool showForm;
    private bool isSubmitting;
    private string? formError;
    private int? editingId;
    private CreateVideoGameRequest formModel = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadGames();
    }

    private async Task LoadGames()
    {
        isLoading = true;
        errorMessage = null;
        try
        {
            games = await VideoGameService.GetAllAsync();
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

    private void ShowCreateForm()
    {
        editingId = null;
        formModel = new();
        formError = null;
        showForm = true;
    }

    private void EditGame(VideoGameResponse game)
    {
        editingId = game.Id;
        formModel = new CreateVideoGameRequest { Name = game.Name };
        formError = null;
        showForm = true;
    }

    private void CancelForm()
    {
        showForm = false;
        formError = null;
    }

    private async Task HandleSubmit()
    {
        isSubmitting = true;
        formError = null;
        try
        {
            if (editingId.HasValue)
                await VideoGameService.UpdateAsync(editingId.Value, formModel);
            else
                await VideoGameService.CreateAsync(formModel);

            showForm = false;
            await LoadGames();
        }
        catch (Exception ex)
        {
            formError = ex.Message;
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private async Task DeleteGame(int id)
    {
        try
        {
            await VideoGameService.DeleteAsync(id);
            await LoadGames();
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
    }
}
