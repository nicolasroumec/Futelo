using Futelo.Client.Services.Toast;
using Futelo.Client.Services.VideoGames;
using Futelo.Client.Shared;
using Futelo.Shared.DTOs.VideoGame;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class Games : LocalizedComponentBase
{
    [Inject] private IVideoGameService VideoGameService { get; set; } = null!;
    [Inject] private IToastService Toast { get; set; } = null!;

    private List<VideoGameResponse> games = [];
    private bool isLoading = true;
    private string? errorMessage;

    private bool showForm;
    private bool isSubmitting;
    private bool isDeleting;
    private int? confirmingDeleteId;
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
            games = await VideoGameService.GetAllAsync(ComponentToken);
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
            Toast.Show(Lang.Get("games.saved"));
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

    private void StartDeleteGame(int id)
    {
        confirmingDeleteId = id;
    }

    private void CancelDelete()
    {
        confirmingDeleteId = null;
    }

    private async Task DeleteGame(int id)
    {
        isDeleting = true;
        try
        {
            await VideoGameService.DeleteAsync(id);
            confirmingDeleteId = null;
            Toast.Show(Lang.Get("games.deleted"));
            await LoadGames();
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            confirmingDeleteId = null;
        }
        finally
        {
            isDeleting = false;
        }
    }
}
