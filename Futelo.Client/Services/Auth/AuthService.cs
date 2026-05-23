using System.Net.Http.Json;
using Futelo.Shared.DTOs.Auth;
using Microsoft.JSInterop;

namespace Futelo.Client.Services.Auth;

public class AuthService(HttpClient http, IJSRuntime js, FuteloAuthStateProvider authStateProvider) : IAuthService
{
    private const string TokenKey = "futelo_token";
    private const string RefreshTokenKey = "futelo_refresh_token";

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var response = await http.PostAsJsonAsync("api/auth/register", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(error);
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>()
            ?? throw new InvalidOperationException("Invalid server response.");

        await SaveTokensAsync(result.Token, result.RefreshToken);
        authStateProvider.NotifyAuthStateChanged();
        return result;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var response = await http.PostAsJsonAsync("api/auth/login", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(error);
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>()
            ?? throw new InvalidOperationException("Invalid server response.");

        await SaveTokensAsync(result.Token, result.RefreshToken);
        authStateProvider.NotifyAuthStateChanged();
        return result;
    }

    public async Task LogoutAsync()
    {
        var refreshToken = await js.InvokeAsync<string?>("localStorage.getItem", RefreshTokenKey);
        if (!string.IsNullOrEmpty(refreshToken))
        {
            try { await http.PostAsJsonAsync("api/auth/logout", new RefreshRequest { RefreshToken = refreshToken }); }
            catch { /* best effort */ }
        }

        await js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        await js.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
        authStateProvider.CurrentToken = null;
        authStateProvider.NotifyAuthStateChanged();
    }

    public async Task<string?> GetTokenAsync()
        => await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    private async Task SaveTokensAsync(string token, string refreshToken)
    {
        await js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        await js.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, refreshToken);
        authStateProvider.CurrentToken = token;
    }
}
