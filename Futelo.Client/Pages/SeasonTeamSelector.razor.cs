using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Team;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class SeasonTeamSelector : LocalizedComponentBase
{
    [Parameter] public int? InitialTeamId { get; set; }
    [Parameter] public List<TeamResponse> Teams { get; set; } = [];
    [Parameter] public EventCallback<int?> OnSave { get; set; }

    private int? _selectedTeamId;
    private bool _dirty;

    protected override void OnParametersSet()
    {
        if (!_dirty)
            _selectedTeamId = InitialTeamId;
    }

    private void OnSelectChange(ChangeEventArgs e)
    {
        var val = e.Value?.ToString();
        _selectedTeamId = string.IsNullOrEmpty(val) ? null : int.Parse(val);
        _dirty = true;
    }

    private async Task HandleSave()
    {
        _dirty = false;
        await OnSave.InvokeAsync(_selectedTeamId);
    }
}
