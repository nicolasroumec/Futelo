using Futelo.Shared.DTOs.Auth;

namespace Futelo.Server.Services.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshAsync(string plainToken);
    Task RevokeAsync(string plainToken);
}
