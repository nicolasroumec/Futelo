using Futelo.Server.Models;
using Futelo.Server.Repositories.Season;
using Futelo.Server.Repositories.Vault;
using Futelo.Shared.DTOs.Season;

namespace Futelo.Server.Services.Season;

public class SeasonService(ISeasonRepository seasonRepository, IVaultRepository vaultRepository) : ISeasonService
{
    public async Task<List<SeasonResponse>> GetByVaultAsync(int vaultId, string userId)
    {
        var vault = await vaultRepository.GetByIdAsync(vaultId);
        if (vault == null || vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("Vault not found.");

        var seasons = await seasonRepository.GetByVaultAsync(vaultId);
        return seasons.Select(MapToResponse).ToList();
    }

    public async Task<SeasonResponse> GetByIdAsync(int id, string userId)
    {
        var season = await seasonRepository.GetByIdAsync(id);
        if (season == null || season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("Season not found.");

        return MapToResponse(season);
    }

    public async Task<SeasonResponse> CreateAsync(string userId, CreateSeasonRequest request)
    {
        var vault = await vaultRepository.GetByIdAsync(request.VaultId);
        if (vault == null || vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("Vault not found.");
        if (vault.OwnerId != userId)
            throw new UnauthorizedAccessException("Only the vault owner can create seasons.");

        var season = new Models.Season
        {
            VaultId = request.VaultId,
            Name = request.Name,
            Year = request.Year,
            Status = SeasonStatus.Draft
        };
        await seasonRepository.CreateAsync(season);
        var created = await seasonRepository.GetByIdAsync(season.Id);
        return MapToResponse(created!);
    }

    public async Task ConfigureAsync(int id, string userId, ConfigureSeasonRequest request)
    {
        var season = await seasonRepository.GetByIdAsync(id);
        if (season == null || season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("Season not found.");
        if (season.Vault.OwnerId != userId)
            throw new UnauthorizedAccessException("Only the vault owner can configure seasons.");
        if (request.HasSuperCup && (!request.HasLeague || !request.HasCup))
            throw new InvalidOperationException("SuperCup requires both League and Cup.");

        var vaultPlayerIds = season.Vault.Players.Select(p => p.PlayerId).ToHashSet();
        if (request.PlayerIds.Any(pid => !vaultPlayerIds.Contains(pid)))
            throw new InvalidOperationException("All players must belong to the vault.");
        if (request.PlayerIds.Count < 2)
            throw new InvalidOperationException("At least 2 players are required.");

        var players = request.PlayerIds.Select(pid => new SeasonPlayer
        {
            SeasonId = id,
            PlayerId = pid,
            SeasonElo = 1500
        }).ToList();

        await seasonRepository.ConfigureAsync(id, players, request.HasLeague, request.LeagueName, request.LeagueIsHomeAndAway, request.HasCup, request.CupName, request.HasSuperCup, request.SuperCupName);
    }

    public async Task ActivateAsync(int id, string userId)
    {
        var season = await seasonRepository.GetByIdAsync(id);
        if (season == null || season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("Season not found.");
        if (season.Vault.OwnerId != userId)
            throw new UnauthorizedAccessException("Only the vault owner can activate a season.");
        if (season.Status != SeasonStatus.Draft)
            throw new InvalidOperationException("Only Draft seasons can be activated.");
        if (!season.Players.Any())
            throw new InvalidOperationException("Season must have players before activation.");
        if (season.League == null && season.Cup == null)
            throw new InvalidOperationException("Season must have at least one competition before activation.");

        await seasonRepository.UpdateStatusAsync(id, SeasonStatus.Active);
    }

    private static SeasonResponse MapToResponse(Models.Season season) => new()
    {
        Id = season.Id,
        VaultId = season.VaultId,
        Name = season.Name,
        Year = season.Year,
        Status = season.Status.ToString(),
        HasLeague = season.League != null,
        LeagueId = season.League?.Id,
        LeagueName = season.League?.Name ?? "League",
        LeagueIsHomeAndAway = season.League?.IsHomeAndAway ?? false,
        HasCup = season.Cup != null,
        CupName = season.Cup?.Name ?? "Cup",
        HasSuperCup = season.SuperCup != null,
        SuperCupName = season.SuperCup?.Name ?? "SuperCup",
        Players = season.Players.Select(p => new SeasonPlayerResponse
        {
            PlayerId = p.PlayerId,
            DisplayName = p.Player.DisplayName,
            SeasonElo = p.SeasonElo
        }).ToList()
    };
}
