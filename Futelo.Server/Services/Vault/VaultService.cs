using Futelo.Server.Models;
using Futelo.Server.Repositories.Vault;
using Futelo.Shared.DTOs.Vault;
using Futelo.Shared.Enums;

namespace Futelo.Server.Services.Vault;

public class VaultService(IVaultRepository repository) : IVaultService
{
    public async Task<List<VaultResponse>> GetUserVaultsAsync(string userId)
    {
        var vaults = await repository.GetByUserAsync(userId);
        return vaults.Select(MapToResponse).ToList();
    }

    public async Task<VaultResponse> GetByIdAsync(int id, string userId)
    {
        var vault = await repository.GetByIdAsync(id);
        if (vault == null || vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("Vault not found.");
        return MapToResponse(vault);
    }

    public async Task<VaultResponse> CreateAsync(string ownerId, CreateVaultRequest request)
    {
        var vault = new Models.Vault
        {
            Name = request.Name,
            OwnerId = ownerId,
            Players = [new VaultPlayer { PlayerId = ownerId, JoinedAt = DateTime.UtcNow, Role = VaultRole.Admin }]
        };
        await repository.CreateAsync(vault);
        var created = await repository.GetByIdAsync(vault.Id);
        return MapToResponse(created!);
    }

    public async Task UpdateAsync(int id, string userId, UpdateVaultRequest request)
    {
        var vault = await repository.GetByIdAsync(id);
        var caller = vault?.Players.FirstOrDefault(p => p.PlayerId == userId);
        if (vault == null || caller == null)
            throw new KeyNotFoundException("Vault not found.");
        if (caller.Role != VaultRole.Admin)
            throw new UnauthorizedAccessException("Only vault admins can update the vault.");
        vault.Name = request.Name;
        await repository.UpdateAsync(vault);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var vault = await repository.GetByIdAsync(id);
        if (vault == null || vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("Vault not found.");
        if (vault.OwnerId != userId)
            throw new UnauthorizedAccessException("Only the vault owner can delete the vault.");
        await repository.DeleteAsync(vault);
    }

    public async Task<List<RecentMatchResponse>> GetRecentMatchesAsync(int id, string userId, int limit)
    {
        var vault = await repository.GetByIdAsync(id);
        if (vault == null || vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("Vault not found.");
        var matches = await repository.GetRecentMatchesAsync(id, limit);
        return matches.Select(MapToRecentMatch).ToList();
    }

    public async Task<MatchHistoryPageResponse> GetMatchHistoryAsync(int id, string userId, int page, int pageSize)
    {
        var vault = await repository.GetByIdAsync(id);
        if (vault == null || vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException("Vault not found.");
        var skip = (page - 1) * pageSize;
        var totalCount = await repository.CountMatchesAsync(id);
        var items = await repository.GetMatchesPageAsync(id, skip, pageSize);
        return new MatchHistoryPageResponse
        {
            Items = items.Select(MapToRecentMatch).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    private static RecentMatchResponse MapToRecentMatch(Models.Match m)
    {
        string competitionType, competitionName, seasonName;
        if (m.League != null)
        {
            competitionType = "League";
            competitionName = m.League.Name;
            seasonName = m.League.Season.Name;
        }
        else if (m.CupRound != null)
        {
            competitionType = "Cup";
            competitionName = m.CupRound.Cup.Name;
            seasonName = m.CupRound.Cup.Season.Name;
        }
        else
        {
            competitionType = "SuperCup";
            competitionName = m.SuperCup!.Name;
            seasonName = m.SuperCup.Season.Name;
        }

        return new RecentMatchResponse
        {
            Id = m.Id,
            HomePlayerId = m.HomePlayerId ?? string.Empty,
            HomePlayerName = m.HomePlayer?.DisplayName ?? string.Empty,
            AwayPlayerId = m.AwayPlayerId ?? string.Empty,
            AwayPlayerName = m.AwayPlayer?.DisplayName ?? string.Empty,
            HomeScore = m.HomeScore,
            AwayScore = m.AwayScore,
            WonOnPenaltiesId = m.WonOnPenaltiesId,
            HomePenaltyScore = m.HomePenaltyScore,
            AwayPenaltyScore = m.AwayPenaltyScore,
            HomeTeamName = m.HomeTeam?.Name,
            AwayTeamName = m.AwayTeam?.Name,
            VideoGameName = m.VideoGame?.Name,
            PlayedAt = m.PlayedAt,
            CompetitionType = competitionType,
            CompetitionName = competitionName,
            SeasonName = seasonName
        };
    }

    private static VaultResponse MapToResponse(Models.Vault vault) => new()
    {
        Id = vault.Id,
        Name = vault.Name,
        OwnerId = vault.OwnerId,
        OwnerDisplayName = vault.Owner.DisplayName,
        Players = vault.Players.Select(p => new VaultPlayerResponse
        {
            PlayerId = p.PlayerId,
            DisplayName = p.Player.DisplayName,
            JoinedAt = p.JoinedAt,
            Role = p.Role
        }).ToList()
    };
}
