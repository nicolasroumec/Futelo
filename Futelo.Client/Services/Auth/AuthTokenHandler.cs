using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Futelo.Client.Services.Auth;

public class AuthTokenHandler(
    FuteloAuthStateProvider authProvider,
    IJSRuntime js,
    NavigationManager navigation) : DelegatingHandler
{
    private const string TokenKey = "futelo_token";

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(authProvider.CurrentToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authProvider.CurrentToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            authProvider.CurrentToken = null;
            authProvider.NotifyAuthStateChanged();
            navigation.NavigateTo("/login", forceLoad: false);
        }

        return response;
    }
}
