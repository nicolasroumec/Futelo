using Futelo.Server.Helpers;
using Futelo.Server.Models;
using Futelo.Server.Repositories.Season;
using Futelo.Server.Repositories.Vault;
using Futelo.Shared.DTOs.Season;
using Futelo.Shared.Enums;

namespace Futelo.Server.Services.Season;

using static ErrorMessages;

public class SeasonService(ISeasonRepository seasonRepository, IVaultRepository vaultRepository, ILogger<SeasonService> logger) : ISeasonService
{
    public async Task<List<SeasonResponse>> GetByVaultAsync(int vaultId, string userId)
    {
        var vault = await vaultRepository.GetByIdAsync(vaultId);
        if (vault == null || vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(VaultNotFound);

        var seasons = await seasonRepository.GetByVaultAsync(vaultId);
        return seasons.Select(MapToResponse).ToList();
    }

    public async Task<SeasonResponse> GetByIdAsync(int id, string userId)
    {
        var season = await seasonRepository.GetByIdAsync(id);
        if (season == null || season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(SeasonNotFound);

        return MapToResponse(season);
    }

    public async Task<SeasonResponse> CreateAsync(string userId, CreateSeasonRequest request)
    {
        var vault = await vaultRepository.GetByIdAsync(request.VaultId);
        if (vault == null || vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(VaultNotFound);
        if (vault.OwnerId != userId)
            throw new UnauthorizedAccessException(OnlyOwnerCanCreateSeason);

        var season = new Models.Season
        {
            VaultId = request.VaultId,
            Name = request.Name,
            Year = request.Year,
            Status = SeasonStatus.Draft,
            VideoGameId = request.VideoGameId,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
        await seasonRepository.CreateAsync(season);
        var created = await seasonRepository.GetByIdAsync(season.Id);
        return MapToResponse(created!);
    }

    public async Task ConfigureAsync(int id, string userId, ConfigureSeasonRequest request)
    {
        var season = await seasonRepository.GetByIdAsync(id);
        if (season == null || season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(SeasonNotFound);
        if (season.Vault.OwnerId != userId)
            throw new UnauthorizedAccessException(OnlyOwnerCanConfigureSeason);
        if (request.HasSuperCup && (!request.HasLeague || !request.HasCup))
            throw new InvalidOperationException(SuperCupRequiresLeagueAndCup);

        var vaultPlayerIds = season.Vault.Players.Select(p => p.PlayerId).ToHashSet();
        if (request.PlayerIds.Any(pid => !vaultPlayerIds.Contains(pid)))
            throw new InvalidOperationException(AllPlayersMustBelongToVault);
        if (request.PlayerIds.Count < 2)
            throw new InvalidOperationException(AtLeast2PlayersRequired);

        var players = request.PlayerIds.Select(pid => new SeasonPlayer
        {
            SeasonId = id,
            PlayerId = pid,
            SeasonElo = EloCalculator.InitialElo
        }).ToList();

        await seasonRepository.ConfigureAsync(id, players, request.HasLeague, request.LeagueName, request.LeagueIsHomeAndAway, request.LeagueStartDate, request.LeagueEndDate, request.HasCup, request.CupName, request.CupStartDate, request.CupEndDate, request.HasSuperCup, request.SuperCupName, request.SuperCupStartDate, request.SuperCupEndDate);
    }

    public async Task FinishAsync(int id, string userId)
    {
        var season = await seasonRepository.GetByIdAsync(id);
        if (season == null || season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(SeasonNotFound);
        if (season.Vault.OwnerId != userId)
            throw new UnauthorizedAccessException(OnlyOwnerCanFinishSeason);
        if (season.Status != SeasonStatus.Active)
            throw new InvalidOperationException(OnlyActiveSeasonCanBeFinished);

        var pending = new List<string>();
        if (season.League != null && season.League.Status != TournamentStatus.Finished)
            pending.Add(season.League.Name);
        if (season.Cup != null && season.Cup.Status != TournamentStatus.Finished)
            pending.Add(season.Cup.Name);
        if (season.SuperCup != null && season.SuperCup.Status != TournamentStatus.Finished)
            pending.Add(season.SuperCup.Name);

        if (pending.Count > 0)
            throw new InvalidOperationException($"The following competitions must be finished first: {string.Join(", ", pending)}.");

        await seasonRepository.UpdateStatusAsync(id, SeasonStatus.Finished);
    }

    public async Task SetPlayerTeamAsync(int id, string playerId, string userId, int? teamId)
    {
        var season = await seasonRepository.GetByIdAsync(id);
        if (season == null || season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(SeasonNotFound);
        if (season.Vault.OwnerId != userId)
            throw new UnauthorizedAccessException(OnlyOwnerCanAssignTeams);
        if (season.Players.All(p => p.PlayerId != playerId))
            throw new KeyNotFoundException(PlayerNotFoundInSeason);

        await seasonRepository.SetPlayerTeamAsync(id, playerId, teamId);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var season = await seasonRepository.GetByIdAsync(id);
        if (season == null || season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(SeasonNotFound);
        if (season.Vault.OwnerId != userId)
            throw new UnauthorizedAccessException(OnlyOwnerCanDeleteSeason);

        await seasonRepository.DeleteAsync(id);
    }

    public async Task PatchVideoGameAsync(int id, string userId, int? videoGameId)
    {
        var season = await seasonRepository.GetByIdAsync(id);
        if (season == null || season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(SeasonNotFound);
        if (season.Vault.OwnerId != userId)
            throw new UnauthorizedAccessException(OnlyOwnerCanUpdateSeason);

        await seasonRepository.PatchVideoGameAsync(id, videoGameId);
    }

    public async Task ActivateAsync(int id, string userId)
    {
        var season = await seasonRepository.GetByIdAsync(id);
        if (season == null || season.Vault.Players.All(p => p.PlayerId != userId))
            throw new KeyNotFoundException(SeasonNotFound);
        if (season.Vault.OwnerId != userId)
            throw new UnauthorizedAccessException(OnlyOwnerCanActivateSeason);
        if (season.Status != SeasonStatus.Draft)
            throw new InvalidOperationException(OnlyDraftSeasonCanBeActivated);
        if (!season.Players.Any())
            throw new InvalidOperationException(SeasonMustHavePlayers);
        if (season.League == null && season.Cup == null)
            throw new InvalidOperationException(SeasonMustHaveCompetition);

        await seasonRepository.UpdateStatusAsync(id, SeasonStatus.Active);
    }

    private static SeasonResponse MapToResponse(Models.Season season) => new()
    {
        Id = season.Id,
        VaultId = season.VaultId,
        Name = season.Name,
        Year = season.Year,
        Status = season.Status.ToString(),
        StartDate = season.StartDate,
        EndDate = season.EndDate,
        VideoGameId = season.VideoGameId,
        VideoGameName = season.VideoGame?.Name,
        HasLeague = season.League != null,
        LeagueId = season.League?.Id,
        LeagueName = season.League?.Name ?? "League",
        LeagueIsHomeAndAway = season.League?.IsHomeAndAway ?? false,
        LeagueStartDate = season.League?.StartDate,
        LeagueEndDate = season.League?.EndDate,
        LeagueStatus = season.League?.Status.ToString(),
        HasCup = season.Cup != null,
        CupId = season.Cup?.Id,
        CupName = season.Cup?.Name ?? "Cup",
        CupStartDate = season.Cup?.StartDate,
        CupEndDate = season.Cup?.EndDate,
        CupStatus = season.Cup?.Status.ToString(),
        HasSuperCup = season.SuperCup != null,
        SuperCupId = season.SuperCup?.Id,
        SuperCupName = season.SuperCup?.Name ?? "SuperCup",
        SuperCupStartDate = season.SuperCup?.StartDate,
        SuperCupEndDate = season.SuperCup?.EndDate,
        SuperCupStatus = season.SuperCup?.Status.ToString(),
        Players = season.Players.Select(p => new SeasonPlayerResponse
        {
            PlayerId = p.PlayerId,
            DisplayName = p.Player.DisplayName,
            SeasonElo = p.SeasonElo,
            TeamId = p.TeamId,
            TeamName = p.Team?.Name
        }).ToList(),
        RecentMatches = BuildRecentMatches(season),
        TopStandings = BuildTopStandings(season)
    };

    private static List<SeasonRecentMatchRow> BuildRecentMatches(Models.Season season)
    {
        var played = new List<(int Id, string Home, string Away, int HS, int AS, string Comp)>();

        if (season.League != null)
            foreach (var m in season.League.Matches.Where(m => m.Status == MatchStatus.Played))
                played.Add((m.Id, m.HomePlayer?.DisplayName ?? "?", m.AwayPlayer?.DisplayName ?? "?",
                    m.HomeScore ?? 0, m.AwayScore ?? 0, season.League.Name));

        if (season.Cup != null)
            foreach (var round in season.Cup.Rounds)
                foreach (var m in round.Matches.Where(m => m.Status == MatchStatus.Played))
                    played.Add((m.Id, m.HomePlayer?.DisplayName ?? "?", m.AwayPlayer?.DisplayName ?? "?",
                        m.HomeScore ?? 0, m.AwayScore ?? 0, season.Cup.Name));

        if (season.SuperCup != null)
            foreach (var m in season.SuperCup.Matches.Where(m => m.Status == MatchStatus.Played))
                played.Add((m.Id, m.HomePlayer?.DisplayName ?? "?", m.AwayPlayer?.DisplayName ?? "?",
                    m.HomeScore ?? 0, m.AwayScore ?? 0, season.SuperCup.Name));

        return played.OrderByDescending(m => m.Id).Take(3)
            .Select(m => new SeasonRecentMatchRow
            {
                HomePlayerName = m.Home,
                AwayPlayerName = m.Away,
                HomeScore = m.HS,
                AwayScore = m.AS,
                Competition = m.Comp
            }).ToList();
    }

    private static List<Futelo.Shared.DTOs.League.StandingRow> BuildTopStandings(Models.Season season)
    {
        if (season.League == null || season.League.Status == TournamentStatus.NotStarted)
            return [];

        var played = season.League.Matches.Where(m => m.Status == MatchStatus.Played).ToList();
        return StandingsCalculator.Compute(played, season.League.Players).Take(3).ToList();
    }
}
