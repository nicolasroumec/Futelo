using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Futelo.Server.Services.Auth;

using static ErrorMessages;

public class AuthService(UserManager<AppUser> userManager, IConfiguration config, FuteloContext context) : IAuthService
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

        return await BuildAuthResponseAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = request.UsernameOrEmail.Contains('@')
            ? await userManager.FindByEmailAsync(request.UsernameOrEmail)
            : await userManager.FindByNameAsync(request.UsernameOrEmail);

        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException(InvalidCredentials);

        return await BuildAuthResponseAsync(user);
    }

    public async Task<AuthResponse> RefreshAsync(string plainToken)
    {
        var hash = Hash(plainToken);

        var rt = await context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.TokenHash == hash && !r.IsRevoked && r.ExpiresAt > DateTime.UtcNow);

        if (rt == null)
            throw new UnauthorizedAccessException(InvalidCredentials);

        rt.IsRevoked = true;
        return await BuildAuthResponseAsync(rt.User);
    }

    public async Task RevokeAsync(string plainToken)
    {
        var hash = Hash(plainToken);
        var rt = await context.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == hash && !r.IsRevoked);
        if (rt == null) return;
        rt.IsRevoked = true;
        await context.SaveChangesAsync();
    }

    private async Task<AuthResponse> BuildAuthResponseAsync(AppUser user)
    {
        var refreshToken = await GenerateAndSaveRefreshTokenAsync(user);
        return new()
        {
            Token = GenerateJwt(user),
            RefreshToken = refreshToken,
            UserId = user.Id,
            Username = user.UserName!,
            DisplayName = user.DisplayName,
            Email = user.Email!
        };
    }

    private async Task<string> GenerateAndSaveRefreshTokenAsync(AppUser user)
    {
        var plainToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var days = int.Parse(config["Jwt:RefreshTokenExpirationDays"] ?? "30");

        context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = Hash(plainToken),
            ExpiresAt = DateTime.UtcNow.AddDays(days),
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        return plainToken;
    }

    private string GenerateJwt(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var minutes = int.Parse(config["Jwt:AccessTokenExpirationMinutes"] ?? "15");

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
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string Hash(string input) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(input)));
}
