using System.Net.Http.Json;
using Futelo.Shared.DTOs.Auth;
using Microsoft.JSInterop;

namespace Futelo.Client.Services.Auth;

public class AuthService(HttpClient http, IJSRuntime js, FuteloAuthStateProvider authStateProvider) : IAuthService
{
    private const string TokenKey = "futelo_token";

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

        await SaveTokenAsync(result.Token);
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

        await SaveTokenAsync(result.Token);
        authStateProvider.NotifyAuthStateChanged();
        return result;
    }

    public async Task LogoutAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        http.DefaultRequestHeaders.Authorization = null;
        authStateProvider.NotifyAuthStateChanged();
    }

    public async Task<string?> GetTokenAsync()
    {
        return await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    private async Task SaveTokenAsync(string token)
    {
        await js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
}
