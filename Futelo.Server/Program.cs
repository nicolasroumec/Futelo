using System.Text;
using Futelo.Server.Data;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Invitation;
using Futelo.Server.Repositories.Cup;
using Futelo.Server.Repositories.League;
using Futelo.Server.Repositories.Season;
using Futelo.Server.Repositories.Teams;
using Futelo.Server.Repositories.Vault;
using Futelo.Server.Repositories.VideoGames;
using Futelo.Server.Services.Auth;
using Futelo.Server.Services.Invitation;
using Futelo.Server.Repositories.Stats;
using Futelo.Server.Repositories.SuperCup;
using Futelo.Server.Services.Cup;
using Futelo.Server.Services.League;
using Futelo.Server.Services.Stats;
using Futelo.Server.Services.SuperCup;
using Futelo.Server.Services.Season;
using Futelo.Server.Services.Teams;
using Futelo.Server.Services.Vault;
using Futelo.Server.Services.VideoGames;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevClient", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddDbContext<FuteloContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<AppUser>()
    .AddEntityFrameworkStores<FuteloContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.UseSecurityTokenValidators = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVaultRepository, VaultRepository>();
builder.Services.AddScoped<IVaultService, VaultService>();
builder.Services.AddScoped<IInvitationRepository, InvitationRepository>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IVideoGameRepository, VideoGameRepository>();
builder.Services.AddScoped<IVideoGameService, VideoGameService>();
builder.Services.AddScoped<ISeasonRepository, SeasonRepository>();
builder.Services.AddScoped<ISeasonService, SeasonService>();
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
builder.Services.AddScoped<ILeagueService, LeagueService>();
builder.Services.AddScoped<ICupRepository, CupRepository>();
builder.Services.AddScoped<ICupService, CupService>();
builder.Services.AddScoped<ISuperCupRepository, SuperCupRepository>();
builder.Services.AddScoped<ISuperCupService, SuperCupService>();
builder.Services.AddScoped<IStatsRepository, StatsRepository>();
builder.Services.AddScoped<IStatsService, StatsService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("DevClient");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
