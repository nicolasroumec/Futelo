using Futelo.Server.Models;
using Futelo.Server.Repositories.Vault;
using Futelo.Shared.DTOs.Vault;
using Futelo.Shared.Enums;

namespace Futelo.Server.Services.Vault;

using static ErrorMessages;

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
            throw new KeyNotFoundException(VaultNotFound);
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
            throw new KeyNotFoundException(VaultNotFound);
        if (caller.Role != VaultRole.Admin)
            throw new UnauthorizedAccessException(OnlyAdminsCanUpdate);
        vault.Name = request.Name;
        await repository.UpdateAsync(vault);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var vault = await repository.GetByIdAsync(id);
        if (vault == null || vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(VaultNotFound);
        if (vault.OwnerId != userId)
            throw new UnauthorizedAccessException(OnlyOwnerCanDelete);
        await repository.DeleteAsync(vault);
    }

    public async Task RemovePlayerAsync(int vaultId, string requestingUserId, string playerId)
    {
        var vault = await repository.GetByIdAsync(vaultId);
        var caller = vault?.Players.FirstOrDefault(p => p.PlayerId == requestingUserId);
        if (vault == null || caller == null)
            throw new KeyNotFoundException(VaultNotFound);
        if (caller.Role != VaultRole.Admin)
            throw new UnauthorizedAccessException(OnlyAdminsCanRemovePlayers);
        if (vault.OwnerId == playerId)
            throw new InvalidOperationException(CannotRemoveVaultOwner);
        if (vault.Players.All(p => p.PlayerId != playerId))
            throw new KeyNotFoundException(PlayerNotFound);
        if (await repository.PlayerHasNonFinishedSeasonAsync(vaultId, playerId))
            throw new InvalidOperationException(PlayerInNonFinishedSeason);
        await repository.RemovePlayerAsync(vaultId, playerId);
    }

    public async Task<List<RecentMatchResponse>> GetRecentMatchesAsync(int id, string userId, int limit)
    {
        var vault = await repository.GetByIdAsync(id);
        if (vault == null || vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(VaultNotFound);
        var matches = await repository.GetRecentMatchesAsync(id, limit);
        return matches.Select(MapToRecentMatch).ToList();
    }

    public async Task<MatchHistoryPageResponse> GetMatchHistoryAsync(int id, string userId, int page, int pageSize, string? competitionType = null)
    {
        var vault = await repository.GetByIdAsync(id);
        if (vault == null || vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(VaultNotFound);
        var skip = (page - 1) * pageSize;
        var totalCount = await repository.CountMatchesAsync(id, competitionType);
        var items = await repository.GetMatchesPageAsync(id, skip, pageSize, competitionType);
        return new MatchHistoryPageResponse
        {
            Items = items.Select(MapToRecentMatch).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<List<FeedEventDto>> GetFeedAsync(int vaultId, string userId, int limit)
    {
        var vault = await repository.GetByIdAsync(vaultId);
        if (vault == null || vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(VaultNotFound);

        var matches = await repository.GetRecentMatchesAsync(vaultId, limit);
        if (matches.Count == 0) return [];

        var matchIds = matches.Select(m => m.Id).ToList();
        var eloMap = await repository.GetEloHistoriesForMatchesAsync(matchIds);

        return matches.Select(m =>
        {
            eloMap.TryGetValue((m.Id, m.HomePlayerId ?? ""), out var homeElo);
            eloMap.TryGetValue((m.Id, m.AwayPlayerId ?? ""), out var awayElo);
            var (type, name, season) = GetCompetitionInfo(m);
            return new FeedEventDto
            {
                MatchId          = m.Id,
                OccurredAt       = m.PlayedAt ?? DateTime.UtcNow,
                CompetitionType  = type,
                CompetitionName  = name,
                SeasonName       = season,
                HomePlayerId     = m.HomePlayerId ?? "",
                HomePlayerName   = m.HomePlayer?.DisplayName ?? "",
                AwayPlayerId     = m.AwayPlayerId ?? "",
                AwayPlayerName   = m.AwayPlayer?.DisplayName ?? "",
                HomeScore        = m.HomeScore,
                AwayScore        = m.AwayScore,
                WonOnPenaltiesId = m.WonOnPenaltiesId,
                HomePenaltyScore = m.HomePenaltyScore,
                AwayPenaltyScore = m.AwayPenaltyScore,
                HomeTeamName     = m.HomeTeam?.Name,
                AwayTeamName     = m.AwayTeam?.Name,
                HomeTeamId       = m.HomeTeamId,
                AwayTeamId       = m.AwayTeamId,
                VideoGameName    = m.VideoGame?.Name,
                HomeEloChange    = homeElo?.EloChange ?? 0,
                AwayEloChange    = awayElo?.EloChange ?? 0,
                HomeNewElo       = homeElo?.EloAfter ?? 0,
                AwayNewElo       = awayElo?.EloAfter ?? 0,
                HomeRankBefore   = homeElo?.RankBefore ?? 0,
                HomeRankAfter    = homeElo?.RankAfter ?? 0,
                AwayRankBefore   = awayElo?.RankBefore ?? 0,
                AwayRankAfter    = awayElo?.RankAfter ?? 0
            };
        }).ToList();
    }

    private static RecentMatchResponse MapToRecentMatch(Models.Match m)
    {
        var (competitionType, competitionName, seasonName) = GetCompetitionInfo(m);
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
            HomeTeamId = m.HomeTeamId,
            AwayTeamId = m.AwayTeamId,
            VideoGameName = m.VideoGame?.Name,
            PlayedAt = m.PlayedAt,
            CompetitionType = competitionType,
            CompetitionName = competitionName,
            SeasonName = seasonName
        };
    }

    private static (string Type, string Name, string Season) GetCompetitionInfo(Models.Match m)
    {
        if (m.League != null)    return ("League",   m.League.Name,         m.League.Season.Name);
        if (m.CupRound != null)  return ("Cup",      m.CupRound.Cup.Name,   m.CupRound.Cup.Season.Name);
        return                          ("SuperCup", m.SuperCup!.Name,      m.SuperCup.Season.Name);
    }

    private static VaultResponse MapToResponse(Models.Vault vault) => new()
    {
        Id = vault.Id,
        Name = vault.Name,
        OwnerId = vault.OwnerId,
        OwnerDisplayName = vault.Owner.DisplayName,
        HasActiveSeason = vault.Seasons.Any(s => s.Status == SeasonStatus.Active),
        ActiveSeasonId = vault.Seasons.FirstOrDefault(s => s.Status == SeasonStatus.Active)?.Id,
        Players = vault.Players.Select(p => new VaultPlayerResponse
        {
            PlayerId = p.PlayerId,
            DisplayName = p.Player.DisplayName,
            JoinedAt = p.JoinedAt,
            Role = p.Role,
            EloRating = p.Player.EloRating
        }).ToList()
    };
}
