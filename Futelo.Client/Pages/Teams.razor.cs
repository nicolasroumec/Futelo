using Futelo.Client.Services.Teams;
using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Team;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class Teams : LocalizedComponentBase
{
    [Inject] private ITeamService TeamService { get; set; } = null!;

    private List<TeamResponse> teams = [];
    private bool isLoading = true;
    private string? errorMessage;

    private bool showForm;
    private bool isSubmitting;
    private string? formError;
    private int? editingId;
    private CreateTeamRequest formModel = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadTeams();
    }

    private async Task LoadTeams()
    {
        isLoading = true;
        errorMessage = null;
        try
        {
            teams = await TeamService.GetAllAsync(ComponentToken);
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

    private void EditTeam(TeamResponse team)
    {
        editingId = team.Id;
        formModel = new CreateTeamRequest { Name = team.Name };
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
                await TeamService.UpdateAsync(editingId.Value, formModel);
            else
                await TeamService.CreateAsync(formModel);

            showForm = false;
            await LoadTeams();
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

    private async Task DeleteTeam(int id)
    {
        try
        {
            await TeamService.DeleteAsync(id);
            await LoadTeams();
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
    }

    private async Task HandleShieldUpload(int teamId, byte[] data)
    {
        try
        {
            await TeamService.UploadShieldAsync(teamId, data);
            var team = teams.FirstOrDefault(t => t.Id == teamId);
            if (team != null)
            {
                team.ShieldUrl = $"/api/teams/{teamId}/shield?v={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
    }

    private async Task HandleShieldDelete(int teamId)
    {
        try
        {
            await TeamService.DeleteShieldAsync(teamId);
            var team = teams.FirstOrDefault(t => t.Id == teamId);
            if (team != null)
            {
                team.ShieldUrl = null;
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
    }
}
