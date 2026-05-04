using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Futelo.Client;
using Futelo.Client.Services.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
var baseAddress = string.IsNullOrEmpty(apiBaseUrl)
    ? builder.HostEnvironment.BaseAddress
    : apiBaseUrl;
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<FuteloAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<FuteloAuthStateProvider>());
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();
