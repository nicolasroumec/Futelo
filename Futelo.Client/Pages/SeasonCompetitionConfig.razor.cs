using Futelo.Client.Shared;
using Futelo.Shared.DTOs.Season;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages;

public partial class SeasonCompetitionConfig : LocalizedComponentBase
{
    [Parameter] public ConfigureSeasonRequest Model { get; set; } = new();
}
