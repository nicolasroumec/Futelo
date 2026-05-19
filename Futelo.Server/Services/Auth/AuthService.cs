using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Futelo.Server.Models;
using Futelo.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Futelo.Server.Services.Auth;

using static ErrorMessages;

public class AuthService(UserManager<AppUser> userManager, IConfiguration config, ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await userManager.FindByEmailAsync(request.Email) != null)
            throw new InvalidOperationException(EmailAlreadyInUse);

        if (await userManager.FindByNameAsync(request.Username) != null)
            throw new InvalidOperationException(UsernameAlreadyInUse);

        var user = new AppUser
        {
            Email = request.Email,
            UserName = request.Username,
            DisplayName = request.DisplayName
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(" ", result.Errors.Select(e => e.Description)));

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = request.UsernameOrEmail.Contains('@')
            ? await userManager.FindByEmailAsync(request.UsernameOrEmail)
            : await userManager.FindByNameAsync(request.UsernameOrEmail);

        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException(InvalidCredentials);

        return BuildAuthResponse(user);
    }

    private AuthResponse BuildAuthResponse(AppUser user) => new()
    {
        Token = GenerateJwt(user),
        UserId = user.Id,
        Username = user.UserName!,
        DisplayName = user.DisplayName,
        Email = user.Email!
    };

    private string GenerateJwt(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddDays(int.Parse(config["Jwt:ExpirationDays"]!));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new Claim("displayName", user.DisplayName)
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
