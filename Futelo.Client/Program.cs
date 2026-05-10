using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Futelo.Client;
using Futelo.Client.Services.Auth;
using Futelo.Client.Services.Invitation;
using Futelo.Client.Services.Cup;
using Futelo.Client.Services.League;
using Futelo.Client.Services.SuperCup;
using Futelo.Client.Services.Season;
using Futelo.Client.Services.Teams;
using Futelo.Client.Services.Vault;
using Futelo.Client.Services.VideoGames;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
var baseAddress = string.IsNullOrEmpty(apiBaseUrl)
    ? builder.HostEnvironment.BaseAddress
    : apiBaseUrl;

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<FuteloAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<FuteloAuthStateProvider>());
builder.Services.AddScoped<AuthTokenHandler>();
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthTokenHandler>();
    handler.InnerHandler = new HttpClientHandler();
    return new HttpClient(handler) { BaseAddress = new Uri(baseAddress) };
});
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVaultService, VaultService>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<ISeasonService, SeasonService>();
builder.Services.AddScoped<ICupService, CupService>();
builder.Services.AddScoped<ISuperCupService, SuperCupService>();
builder.Services.AddScoped<ILeagueService, LeagueService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IVideoGameService, VideoGameService>();

await builder.Build().RunAsync();
