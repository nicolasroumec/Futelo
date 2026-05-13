using Futelo.Server.Models;
using Futelo.Server.Repositories.Stats;
using Futelo.Shared.DTOs.Stats;
using Futelo.Shared.Enums;

namespace Futelo.Server.Services.Stats;

public class StatsService(IStatsRepository statsRepository) : IStatsService
{
    public async Task<PlayerStatsResponse> GetPlayerStatsAsync(string playerId, int vaultId, string requesterId)
    {
        if (!await statsRepository.IsVaultMemberAsync(requesterId, vaultId))
            throw new KeyNotFoundException("Vault not found.");

        var player = await statsRepository.GetPlayerAsync(playerId);
        if (player == null)
            throw new KeyNotFoundException("Player not found.");

        var matches = await statsRepository.GetPlayerMatchesInVaultAsync(playerId, vaultId);

        int won = 0, drawn = 0, lost = 0, goalsFor = 0, goalsAgainst = 0;

        foreach (var m in matches)
        {
            bool isHome = m.HomePlayerId == playerId;
            int scored = isHome ? (m.HomeScore ?? 0) : (m.AwayScore ?? 0);
            int conceded = isHome ? (m.AwayScore ?? 0) : (m.HomeScore ?? 0);

            if (scored > conceded) won++;
            else if (scored < conceded) lost++;
            else drawn++;

            goalsFor += scored;
            goalsAgainst += conceded;
        }

        var (currentStreak, bestWinStreak, bestUnbeatenStreak, currentStreakType) = ComputeStreaks(matches, playerId);

        var titleSeasons = await statsRepository.GetPlayerTitleSeasonsInVaultAsync(playerId, vaultId);
        var titles = new List<PlayerTitleEntry>();
        foreach (var s in titleSeasons)
        {
            if (s.League?.ChampionId == playerId)
                titles.Add(new PlayerTitleEntry { SeasonId = s.Id, SeasonName = s.Name, Year = s.Year, Competition = "League" });
            if (s.Cup?.ChampionId == playerId)
                titles.Add(new PlayerTitleEntry { SeasonId = s.Id, SeasonName = s.Name, Year = s.Year, Competition = "Cup" });
            if (s.SuperCup?.ChampionId == playerId)
                titles.Add(new PlayerTitleEntry { SeasonId = s.Id, SeasonName = s.Name, Year = s.Year, Competition = "SuperCup" });
        }

        var topTeams = matches
            .Select(m => new
            {
                TeamName = m.HomePlayerId == playerId ? m.HomeTeam?.Name : m.AwayTeam?.Name,
                IsHome = m.HomePlayerId == playerId,
                HomeScore = m.HomeScore ?? 0,
                AwayScore = m.AwayScore ?? 0
            })
            .Where(x => x.TeamName != null)
            .GroupBy(x => x.TeamName!)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g =>
            {
                int tWon = 0, tDrawn = 0, tLost = 0, tGF = 0, tGA = 0;
                foreach (var x in g)
                {
                    int scored = x.IsHome ? x.HomeScore : x.AwayScore;
                    int conceded = x.IsHome ? x.AwayScore : x.HomeScore;
                    if (scored > conceded) tWon++;
                    else if (scored < conceded) tLost++;
                    else tDrawn++;
                    tGF += scored;
                    tGA += conceded;
                }
                return new TeamUsageRow
                {
                    TeamName = g.Key,
                    TimesUsed = g.Count(),
                    Won = tWon,
                    Drawn = tDrawn,
                    Lost = tLost,
                    GoalsFor = tGF,
                    GoalsAgainst = tGA
                };
            })
            .ToList();

        var gameStats = matches
            .Where(m => m.VideoGame != null)
            .GroupBy(m => m.VideoGame!.Name)
            .OrderByDescending(g => g.Count())
            .Select(g =>
            {
                int gWon = 0, gDrawn = 0, gLost = 0, gGF = 0, gGA = 0;
                foreach (var m in g)
                {
                    bool isHome = m.HomePlayerId == playerId;
                    int s = isHome ? (m.HomeScore ?? 0) : (m.AwayScore ?? 0);
                    int c = isHome ? (m.AwayScore ?? 0) : (m.HomeScore ?? 0);
                    if (s > c) gWon++;
                    else if (s < c) gLost++;
                    else gDrawn++;
                    gGF += s;
                    gGA += c;
                }
                return new VideoGameStatsRow
                {
                    VideoGameName = g.Key,
                    Played = g.Count(),
                    Won = gWon,
                    Drawn = gDrawn,
                    Lost = gLost,
                    GoalsFor = gGF,
                    GoalsAgainst = gGA
                };
            })
            .ToList();

        return new PlayerStatsResponse
        {
            PlayerId = player.Id,
            DisplayName = player.DisplayName,
            Played = matches.Count,
            Won = won,
            Drawn = drawn,
            Lost = lost,
            GoalsFor = goalsFor,
            GoalsAgainst = goalsAgainst,
            EloRating = player.EloRating,
            CurrentStreak = currentStreak,
            CurrentStreakType = currentStreakType,
            BestWinStreak = bestWinStreak,
            BestUnbeatenStreak = bestUnbeatenStreak,
            TopTeams = topTeams,
            GameStats = gameStats,
            Titles = titles
        };
    }

    public async Task<HeadToHeadResponse> GetHeadToHeadAsync(string player1Id, string player2Id, int vaultId, string requesterId)
    {
        if (!await statsRepository.IsVaultMemberAsync(requesterId, vaultId))
            throw new KeyNotFoundException("Vault not found.");

        var player1 = await statsRepository.GetPlayerAsync(player1Id);
        var player2 = await statsRepository.GetPlayerAsync(player2Id);

        if (player1 == null || player2 == null)
            throw new KeyNotFoundException("Player not found.");

        var matches = await statsRepository.GetH2HMatchesInVaultAsync(player1Id, player2Id, vaultId);

        int p1Wins = 0, draws = 0, p2Wins = 0;

        foreach (var m in matches)
        {
            int homeScore = m.HomeScore ?? 0;
            int awayScore = m.AwayScore ?? 0;

            if (homeScore == awayScore)
                draws++;
            else if ((m.HomePlayerId == player1Id && homeScore > awayScore) ||
                     (m.AwayPlayerId == player1Id && awayScore > homeScore))
                p1Wins++;
            else
                p2Wins++;
        }

        return new HeadToHeadResponse
        {
            Player1Id = player1.Id,
            Player1DisplayName = player1.DisplayName,
            Player2Id = player2.Id,
            Player2DisplayName = player2.DisplayName,
            Played = matches.Count,
            Player1Wins = p1Wins,
            Draws = draws,
            Player2Wins = p2Wins,
            Matches = matches
                .OrderByDescending(m => m.PlayedAt)
                .Select(m => new H2HMatchRow
                {
                    MatchId = m.Id,
                    HomePlayerId = m.HomePlayerId!,
                    HomePlayerDisplayName = m.HomePlayer?.DisplayName ?? m.HomePlayerId!,
                    HomeScore = m.HomeScore ?? 0,
                    AwayScore = m.AwayScore ?? 0,
                    AwayPlayerId = m.AwayPlayerId!,
                    AwayPlayerDisplayName = m.AwayPlayer?.DisplayName ?? m.AwayPlayerId!,
                    VideoGameName = m.VideoGame?.Name,
                    PlayedAt = m.PlayedAt
                }).ToList()
        };
    }

    public async Task<List<RankingRow>> GetGeneralRankingAsync(int vaultId, string requesterId)
    {
        if (!await statsRepository.IsVaultMemberAsync(requesterId, vaultId))
            throw new KeyNotFoundException("Vault not found.");

        var vaultPlayers = await statsRepository.GetGeneralRankingAsync(vaultId);

        return vaultPlayers
            .Select((vp, i) => new RankingRow
            {
                Position = i + 1,
                PlayerId = vp.PlayerId,
                DisplayName = vp.Player.DisplayName,
                HistoricalElo = vp.Player.EloRating
            }).ToList();
    }

    public async Task<List<RankingRow>> GetRankingAsync(int seasonId, int vaultId, string requesterId)
    {
        if (!await statsRepository.IsVaultMemberAsync(requesterId, vaultId))
            throw new KeyNotFoundException("Vault not found.");

        var seasonPlayers = await statsRepository.GetSeasonRankingAsync(seasonId, vaultId);

        return seasonPlayers
            .Select((sp, i) => new RankingRow
            {
                Position = i + 1,
                PlayerId = sp.PlayerId,
                DisplayName = sp.Player.DisplayName,
                SeasonElo = sp.SeasonElo,
                HistoricalElo = sp.Player.EloRating
            }).ToList();
    }

    public async Task<List<PalmaresSeasonRow>> GetPalmaresAsync(int vaultId, string requesterId)
    {
        if (!await statsRepository.IsVaultMemberAsync(requesterId, vaultId))
            throw new KeyNotFoundException("Vault not found.");

        var seasons = await statsRepository.GetVaultPalmaresAsync(vaultId);

        return seasons
            .Where(s => s.League?.ChampionId != null || s.Cup?.ChampionId != null || s.SuperCup?.ChampionId != null)
            .Select(s => new PalmaresSeasonRow
            {
                SeasonId = s.Id,
                SeasonName = s.Name,
                Year = s.Year,
                LeagueChampion = s.League?.Champion?.DisplayName,
                CupChampion = s.Cup?.Champion?.DisplayName,
                SuperCupChampion = s.SuperCup?.Champion?.DisplayName
            }).ToList();
    }

    public async Task<List<EloHistoryPoint>> GetEloHistoryAsync(string playerId, int vaultId, string requesterId)
    {
        if (!await statsRepository.IsVaultMemberAsync(requesterId, vaultId))
            throw new KeyNotFoundException("Vault not found.");

        var history = await statsRepository.GetPlayerEloHistoryInVaultAsync(playerId, vaultId);

        if (history.Count == 0)
            return [];

        var points = new List<EloHistoryPoint>
        {
            new() { Date = history[0].CreatedAt, Elo = history[0].EloBefore }
        };
        points.AddRange(history.Select(h => new EloHistoryPoint { Date = h.CreatedAt, Elo = h.EloAfter }));

        return points;
    }

    public async Task<List<ScorerRow>> GetScorersAsync(int vaultId, string requesterId)
    {
        if (!await statsRepository.IsVaultMemberAsync(requesterId, vaultId))
            throw new KeyNotFoundException("Vault not found.");

        var matches = await statsRepository.GetAllPlayedMatchesInVaultAsync(vaultId);
        var scorerMap = new Dictionary<string, (string DisplayName, int Goals)>();

        foreach (var m in matches)
        {
            if (m.HomePlayerId != null && m.HomePlayer != null)
            {
                var entry = scorerMap.GetValueOrDefault(m.HomePlayerId, (DisplayName: m.HomePlayer.DisplayName, Goals: 0));
                scorerMap[m.HomePlayerId] = (DisplayName: entry.DisplayName, Goals: entry.Goals + (m.HomeScore ?? 0));
            }
            if (m.AwayPlayerId != null && m.AwayPlayer != null)
            {
                var entry = scorerMap.GetValueOrDefault(m.AwayPlayerId, (DisplayName: m.AwayPlayer.DisplayName, Goals: 0));
                scorerMap[m.AwayPlayerId] = (DisplayName: entry.DisplayName, Goals: entry.Goals + (m.AwayScore ?? 0));
            }
        }

        return scorerMap
            .OrderByDescending(x => x.Value.Goals)
            .Select((x, i) => new ScorerRow
            {
                Position = i + 1,
                PlayerId = x.Key,
                DisplayName = x.Value.DisplayName,
                Goals = x.Value.Goals
            })
            .ToList();
    }
    public async Task<VaultRecordsResponse> GetVaultRecordsAsync(int vaultId, string requesterId)
    {
        if (!await statsRepository.IsVaultMemberAsync(requesterId, vaultId))
            throw new KeyNotFoundException("Vault not found.");

        var matches = await statsRepository.GetAllPlayedMatchesInVaultAsync(vaultId);

        var playerIds = matches
            .SelectMany(m => new[] { m.HomePlayerId, m.AwayPlayerId })
            .Where(id => id != null)
            .Distinct()
            .ToList();

        RecordEntry? bestWinRecord = null;
        RecordEntry? bestUnbeatenRecord = null;

        foreach (var playerId in playerIds)
        {
            var playerMatches = matches
                .Where(m => m.HomePlayerId == playerId || m.AwayPlayerId == playerId)
                .ToList();

            var playerUser = playerMatches
                .Select(m => m.HomePlayerId == playerId ? m.HomePlayer : m.AwayPlayer)
                .FirstOrDefault(p => p != null);

            if (playerUser == null) continue;

            var (_, bestWin, bestUnbeaten, _) = ComputeStreaks(playerMatches, playerId!);

            if (bestWinRecord == null || bestWin > bestWinRecord.Value)
                bestWinRecord = new RecordEntry { PlayerId = playerId!, DisplayName = playerUser.DisplayName, Value = bestWin };

            if (bestUnbeatenRecord == null || bestUnbeaten > bestUnbeatenRecord.Value)
                bestUnbeatenRecord = new RecordEntry { PlayerId = playerId!, DisplayName = playerUser.DisplayName, Value = bestUnbeaten };
        }

        return new VaultRecordsResponse
        {
            BestWinStreak = bestWinRecord,
            BestUnbeatenStreak = bestUnbeatenRecord
        };
    }

    public async Task<List<GameStatsEntry>> GetGamesRankingAsync(int vaultId, string requesterId)
    {
        if (!await statsRepository.IsVaultMemberAsync(requesterId, vaultId))
            throw new KeyNotFoundException("Vault not found.");

        var matches = await statsRepository.GetAllPlayedMatchesWithVideoGameInVaultAsync(vaultId);

        var entries = new List<(int GameId, string GameName, string PlayerId, string DisplayName, bool IsWin, bool IsDraw, int GoalsFor, int GoalsAgainst)>();

        foreach (var m in matches)
        {
            if (m.VideoGame == null) continue;
            int homeScore = m.HomeScore ?? 0;
            int awayScore = m.AwayScore ?? 0;

            if (m.HomePlayer != null)
                entries.Add((m.VideoGame.Id, m.VideoGame.Name, m.HomePlayerId!, m.HomePlayer.DisplayName,
                    homeScore > awayScore, homeScore == awayScore, homeScore, awayScore));

            if (m.AwayPlayer != null)
                entries.Add((m.VideoGame.Id, m.VideoGame.Name, m.AwayPlayerId!, m.AwayPlayer.DisplayName,
                    awayScore > homeScore, homeScore == awayScore, awayScore, homeScore));
        }

        return entries
            .GroupBy(e => new { e.GameId, e.GameName })
            .OrderBy(g => g.Key.GameName)
            .Select(g =>
            {
                var players = g.GroupBy(e => e.PlayerId)
                    .Select(pg =>
                    {
                        int won = pg.Count(e => e.IsWin);
                        int drawn = pg.Count(e => e.IsDraw);
                        int lost = pg.Count(e => !e.IsWin && !e.IsDraw);
                        return new PlayerGameStatsRow
                        {
                            PlayerId = pg.Key,
                            DisplayName = pg.First().DisplayName,
                            Played = pg.Count(),
                            Won = won,
                            Drawn = drawn,
                            Lost = lost,
                            GoalsFor = pg.Sum(e => e.GoalsFor),
                            GoalsAgainst = pg.Sum(e => e.GoalsAgainst)
                        };
                    })
                    .OrderByDescending(p => p.Won)
                    .ThenByDescending(p => p.GoalsFor - p.GoalsAgainst)
                    .ToList();

                for (int i = 0; i < players.Count; i++)
                    players[i].Position = i + 1;

                return new GameStatsEntry
                {
                    VideoGameId = g.Key.GameId,
                    VideoGameName = g.Key.GameName,
                    Players = players
                };
            })
            .ToList();
    }

    public async Task<List<TeamPanelRow>> GetTeamPanelAsync(int vaultId, string requesterId)
    {
        if (!await statsRepository.IsVaultMemberAsync(requesterId, vaultId))
            throw new KeyNotFoundException("Vault not found.");

        var matches = await statsRepository.GetAllPlayedMatchesWithTeamsInVaultAsync(vaultId);

        var entries = new List<(string TeamName, string PlayerId, string PlayerDisplayName, bool IsWin, bool IsDraw, int GoalsFor, int GoalsAgainst)>();

        foreach (var m in matches)
        {
            int homeScore = m.HomeScore ?? 0;
            int awayScore = m.AwayScore ?? 0;

            if (m.HomeTeam != null && m.HomePlayer != null)
                entries.Add((m.HomeTeam.Name, m.HomePlayerId!, m.HomePlayer.DisplayName,
                    homeScore > awayScore, homeScore == awayScore, homeScore, awayScore));

            if (m.AwayTeam != null && m.AwayPlayer != null)
                entries.Add((m.AwayTeam.Name, m.AwayPlayerId!, m.AwayPlayer.DisplayName,
                    awayScore > homeScore, homeScore == awayScore, awayScore, homeScore));
        }

        return entries
            .GroupBy(e => e.TeamName)
            .Select(g => new TeamPanelRow
            {
                TeamName = g.Key,
                TotalUsed = g.Count(),
                Won = g.Count(e => e.IsWin),
                Drawn = g.Count(e => e.IsDraw),
                Lost = g.Count(e => !e.IsWin && !e.IsDraw),
                GoalsFor = g.Sum(e => e.GoalsFor),
                GoalsAgainst = g.Sum(e => e.GoalsAgainst),
                Players = g.GroupBy(e => e.PlayerId)
                    .OrderByDescending(pg => pg.Count())
                    .Select(pg => new PlayerTeamUsageRow
                    {
                        PlayerId = pg.Key,
                        DisplayName = pg.First().PlayerDisplayName,
                        TimesUsed = pg.Count()
                    }).ToList()
            })
            .OrderByDescending(t => t.TotalUsed)
            .ToList();
    }

    public async Task<List<RecentFormEntry>> GetRecentFormAsync(string playerId, int vaultId, string requesterId)
    {
        if (!await statsRepository.IsVaultMemberAsync(requesterId, vaultId))
            throw new KeyNotFoundException("Vault not found.");

        var player = await statsRepository.GetPlayerAsync(playerId);
        if (player == null)
            throw new KeyNotFoundException("Player not found.");

        var matches = await statsRepository.GetPlayerLastNMatchesAsync(playerId, vaultId, 5);

        return matches.Select(m =>
        {
            bool isHome = m.HomePlayerId == playerId;
            int scored = isHome ? (m.HomeScore ?? 0) : (m.AwayScore ?? 0);
            int conceded = isHome ? (m.AwayScore ?? 0) : (m.HomeScore ?? 0);
            var opponent = isHome ? m.AwayPlayer : m.HomePlayer;

            return new RecentFormEntry
            {
                Result = scored > conceded ? MatchResult.Win
                    : scored < conceded ? MatchResult.Loss
                    : MatchResult.Draw,
                GoalsFor = scored,
                GoalsAgainst = conceded,
                OpponentDisplayName = opponent?.DisplayName ?? string.Empty,
                PlayedAt = m.PlayedAt
            };
        }).ToList();
    }

    public async Task<TopScoringMatchResponse?> GetTopScoringMatchAsync(int vaultId, string requesterId)
    {
        if (!await statsRepository.IsVaultMemberAsync(requesterId, vaultId))
            throw new KeyNotFoundException("Vault not found.");

        var match = await statsRepository.GetTopScoringMatchInVaultAsync(vaultId);
        if (match == null) return null;

        var seasonName = match.League?.Season?.Name
            ?? match.CupRound?.Cup?.Season?.Name
            ?? match.SuperCup?.Season?.Name
            ?? string.Empty;

        return new TopScoringMatchResponse
        {
            HomePlayerDisplayName = match.HomePlayer?.DisplayName ?? string.Empty,
            AwayPlayerDisplayName = match.AwayPlayer?.DisplayName ?? string.Empty,
            HomeScore = match.HomeScore ?? 0,
            AwayScore = match.AwayScore ?? 0,
            TotalGoals = (match.HomeScore ?? 0) + (match.AwayScore ?? 0),
            PlayedAt = match.PlayedAt,
            SeasonName = seasonName
        };
    }

    private static (int current, int bestWin, int bestUnbeaten, StreakType currentType) ComputeStreaks(List<Match> matches, string playerId)
    {
        var sorted = matches
            .Where(m => m.PlayedAt.HasValue)
            .OrderBy(m => m.PlayedAt)
            .ToList();

        if (sorted.Count == 0)
            return (0, 0, 0, StreakType.Win);

        int bestWin = 0, bestUnbeaten = 0;
        int runWin = 0, runUnbeaten = 0;

        foreach (var m in sorted)
        {
            bool isHome = m.HomePlayerId == playerId;
            int s = isHome ? (m.HomeScore ?? 0) : (m.AwayScore ?? 0);
            int c = isHome ? (m.AwayScore ?? 0) : (m.HomeScore ?? 0);

            if (s > c) { runWin++; runUnbeaten++; }
            else if (s == c) { runWin = 0; runUnbeaten++; }
            else { runWin = 0; runUnbeaten = 0; }

            if (runWin > bestWin) bestWin = runWin;
            if (runUnbeaten > bestUnbeaten) bestUnbeaten = runUnbeaten;
        }

        var last = sorted.Last();
        bool lastHome = last.HomePlayerId == playerId;
        int lastS = lastHome ? (last.HomeScore ?? 0) : (last.AwayScore ?? 0);
        int lastC = lastHome ? (last.AwayScore ?? 0) : (last.HomeScore ?? 0);

        StreakType currentType = lastS > lastC ? StreakType.Win
            : lastS < lastC ? StreakType.Loss
            : StreakType.Draw;

        int current = 0;
        for (int i = sorted.Count - 1; i >= 0; i--)
        {
            bool h = sorted[i].HomePlayerId == playerId;
            int s = h ? (sorted[i].HomeScore ?? 0) : (sorted[i].AwayScore ?? 0);
            int c = h ? (sorted[i].AwayScore ?? 0) : (sorted[i].HomeScore ?? 0);

            bool sameType = currentType switch
            {
                StreakType.Win => s > c,
                StreakType.Loss => s < c,
                _ => s == c
            };

            if (sameType) current++;
            else break;
        }

        return (current, bestWin, bestUnbeaten, currentType);
    }
}
