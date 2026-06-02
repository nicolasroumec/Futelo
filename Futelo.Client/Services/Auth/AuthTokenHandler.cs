using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Futelo.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Futelo.Client.Services.Auth;

public class AuthTokenHandler(
    FuteloAuthStateProvider authProvider,
    IJSRuntime js,
    NavigationManager navigation) : DelegatingHandler
{
    private const string TokenKey = "futelo_token";
    private const string RefreshTokenKey = "futelo_refresh_token";

    private static readonly SemaphoreSlim RefreshLock = new(1, 1);

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tokenAtSend = authProvider.CurrentToken;

        if (!string.IsNullOrEmpty(tokenAtSend))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenAtSend);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        // Auth endpoints return 401 to signal bad/expired credentials, not an
        // expired access token — let those flow back to the caller untouched.
        if (request.RequestUri?.AbsolutePath.Contains("api/auth/") == true)
            return response;

        await RefreshLock.WaitAsync(cancellationToken);
        try
        {
            // If another concurrent request already refreshed, just retry with the new token
            if (authProvider.CurrentToken != tokenAtSend && !string.IsNullOrEmpty(authProvider.CurrentToken))
            {
                // fall through to retry below
            }
            else
            {
                var refreshToken = await js.InvokeAsync<string?>("localStorage.getItem", RefreshTokenKey);
                if (string.IsNullOrEmpty(refreshToken))
                {
                    await ClearSessionAsync();
                    return response;
                }

                var refreshMsg = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh")
                {
                    Content = JsonContent.Create(new RefreshRequest { RefreshToken = refreshToken })
                };

                var refreshResp = await base.SendAsync(refreshMsg, cancellationToken);

                if (!refreshResp.IsSuccessStatusCode)
                {
                    await ClearSessionAsync();
                    return response;
                }

                var auth = await refreshResp.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken);
                if (auth == null)
                {
                    await ClearSessionAsync();
                    return response;
                }

                await js.InvokeVoidAsync("localStorage.setItem", TokenKey, auth.Token);
                await js.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, auth.RefreshToken);
                authProvider.CurrentToken = auth.Token;
                authProvider.NotifyAuthStateChanged();
            }
        }
        finally
        {
            RefreshLock.Release();
        }

        var retry = await CloneRequestAsync(request);
        retry.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authProvider.CurrentToken!);
        return await base.SendAsync(retry, cancellationToken);
    }

    private async Task ClearSessionAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        await js.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
        authProvider.CurrentToken = null;
        authProvider.NotifyAuthStateChanged();
        navigation.NavigateTo("/login", forceLoad: false);
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri);

        if (original.Content != null)
        {
            var ms = new MemoryStream();
            await original.Content.CopyToAsync(ms);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);
            foreach (var h in original.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
        }

        foreach (var h in original.Headers)
            clone.Headers.TryAddWithoutValidation(h.Key, h.Value);

        return clone;
    }
}
