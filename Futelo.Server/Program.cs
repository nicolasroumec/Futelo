using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using Futelo.Server.Data;
using Futelo.Server.Filters;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Achievement;
using Futelo.Server.Repositories.Invitation;
using Futelo.Server.Repositories.Cup;
using Futelo.Server.Repositories.Shared;
using Futelo.Server.Repositories.League;
using Futelo.Server.Repositories.Season;
using Futelo.Server.Repositories.Teams;
using Futelo.Server.Repositories.Users;
using Futelo.Server.Repositories.Vault;
using Futelo.Server.Repositories.VideoGames;
using Futelo.Server.Services.Achievement;
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
using Futelo.Server.Services.Users;
using Futelo.Server.Services.Vault;
using Futelo.Server.Services.VideoGames;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
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

// Railway (and most managed Postgres) expose a single DATABASE_URL in URI form.
// Npgsql only accepts keyword format, and building it by hand breaks when the
// password contains special chars (;, =, @...). Parse the URI here instead.
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
var connectionString = string.IsNullOrWhiteSpace(databaseUrl)
    ? builder.Configuration.GetConnectionString("DefaultConnection")
    : BuildNpgsqlConnectionString(databaseUrl);

builder.Services.AddDbContext<FuteloContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentityCore<AppUser>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
    })
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

builder.Services.AddScoped<IAchievementRepository, AchievementRepository>();
builder.Services.AddScoped<IAchievementEvaluationRepository, AchievementEvaluationRepository>();
builder.Services.AddScoped<IAchievementEngine, AchievementEngine>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
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
builder.Services.AddScoped<ISeasonRecapRepository, SeasonRecapRepository>();
builder.Services.AddScoped<ISeasonService, SeasonService>();
builder.Services.AddScoped<ISeasonRecapService, SeasonRecapService>();
builder.Services.AddScoped<IEloRollbackRepository, EloRollbackRepository>();
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
builder.Services.AddScoped<ILeagueService, LeagueService>();
builder.Services.AddScoped<ICupRepository, CupRepository>();
builder.Services.AddScoped<ICupService, CupService>();
builder.Services.AddScoped<ISuperCupRepository, SuperCupRepository>();
builder.Services.AddScoped<ISuperCupService, SuperCupService>();
builder.Services.AddScoped<IStatsRepository, StatsRepository>();
builder.Services.AddScoped<IStatsService, StatsService>();

builder.Services.AddMemoryCache();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            }));
});

builder.Services.AddControllers(options => options.Filters.Add<ApiExceptionFilter>());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FuteloContext>();
    await db.Database.MigrateAsync();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseCors("DevClient");
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/api/version", () =>
    Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "unknown")
    .AllowAnonymous();
app.MapFallbackToFile("index.html");

app.Run();

// Converts a postgres://user:pass@host:port/db URI into an Npgsql keyword
// connection string. NpgsqlConnectionStringBuilder handles all escaping, so
// special characters in the password no longer break the connection.
static string BuildNpgsqlConnectionString(string databaseUrl)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);
    var csb = new Npgsql.NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port,
        Database = uri.AbsolutePath.TrimStart('/'),
        Username = Uri.UnescapeDataString(userInfo[0]),
        Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : null,
        SslMode = Npgsql.SslMode.Prefer
    };
    return csb.ConnectionString;
}
