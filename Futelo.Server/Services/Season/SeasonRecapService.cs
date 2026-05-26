using Futelo.Server.Models;
using Futelo.Server.Repositories.Season;
using Futelo.Shared.DTOs.Season;

namespace Futelo.Server.Services.Season;

using static ErrorMessages;

public class SeasonRecapService(ISeasonRecapRepository recapRepository) : ISeasonRecapService
{
    public async Task<SeasonRecapResponse> GetRecapAsync(int seasonId)
    {
        var season = await recapRepository.GetSeasonWithChampionsAsync(seasonId)
            ?? throw new KeyNotFoundException(SeasonNotFound);

        var matchesTask = recapRepository.GetSeasonMatchesAsync(seasonId);
        var eloTask = recapRepository.GetSeasonEloHistoriesAsync(seasonId);
        await Task.WhenAll(matchesTask, eloTask);

        var matches = matchesTask.Result;
        var eloHistories = eloTask.Result;

        return new SeasonRecapResponse
        {
            SeasonId = season.Id,
            SeasonName = season.Name,
            Year = season.Year,
            LeagueChampion = season.League?.Champion?.DisplayName,
            CupChampion = season.Cup?.Champion?.DisplayName,
            SuperCupChampion = season.SuperCup?.Champion?.DisplayName,
            TotalMatches = matches.Count,
            TotalGoals = matches.Sum(m => (m.HomeScore ?? 0) + (m.AwayScore ?? 0)),
        }
        .WithTopScorer(matches)
        .WithMostImproved(eloHistories)
        .WithBiggestWin(matches)
        .WithLongestStreak(matches);
    }
}

file static class SeasonRecapExtensions
{
    internal static SeasonRecapResponse WithTopScorer(this SeasonRecapResponse recap, List<Match> matches)
    {
        var goals = new Dictionary<string, (string Name, int Goals)>();

        foreach (var m in matches)
        {
            if (m.HomePlayerId != null && m.HomePlayer != null && m.HomeScore.HasValue)
                goals[m.HomePlayerId] = goals.TryGetValue(m.HomePlayerId, out var h)
                    ? (h.Name, h.Goals + m.HomeScore.Value)
                    : (m.HomePlayer.DisplayName, m.HomeScore.Value);

            if (m.AwayPlayerId != null && m.AwayPlayer != null && m.AwayScore.HasValue)
                goals[m.AwayPlayerId] = goals.TryGetValue(m.AwayPlayerId, out var a)
                    ? (a.Name, a.Goals + m.AwayScore.Value)
                    : (m.AwayPlayer.DisplayName, m.AwayScore.Value);
        }

        var top = goals.Values.OrderByDescending(p => p.Goals).FirstOrDefault();
        recap.TopScorerName = top.Name;
        recap.TopScorerGoals = top.Goals;
        return recap;
    }

    internal static SeasonRecapResponse WithMostImproved(this SeasonRecapResponse recap, List<EloHistory> histories)
    {
        var best = histories
            .GroupBy(e => e.PlayerId)
            .Select(g => (Name: g.First().Player.DisplayName, Gain: g.Sum(e => e.EloChange)))
            .OrderByDescending(p => p.Gain)
            .FirstOrDefault();

        recap.MostImprovedName = best.Name;
        recap.MostImprovedEloGain = best.Gain;
        return recap;
    }

    internal static SeasonRecapResponse WithBiggestWin(this SeasonRecapResponse recap, List<Match> matches)
    {
        var best = matches
            .Where(m => m.HomeScore.HasValue && m.AwayScore.HasValue
                     && m.HomePlayer != null && m.AwayPlayer != null)
            .OrderByDescending(m => Math.Abs(m.HomeScore!.Value - m.AwayScore!.Value))
            .FirstOrDefault();

        if (best is null) return recap;

        recap.BiggestWinHome = best.HomePlayer!.DisplayName;
        recap.BiggestWinAway = best.AwayPlayer!.DisplayName;
        recap.BiggestWinHomeScore = best.HomeScore!.Value;
        recap.BiggestWinAwayScore = best.AwayScore!.Value;
        return recap;
    }

    internal static SeasonRecapResponse WithLongestStreak(this SeasonRecapResponse recap, List<Match> matches)
    {
        var ordered = matches
            .Where(m => m.HomePlayerId != null && m.AwayPlayerId != null)
            .OrderBy(m => m.PlayedAt)
            .ToList();

        var playerNames = ordered
            .SelectMany<Match, (string Id, string Name)>(m =>
            [
                (m.HomePlayerId!, m.HomePlayer?.DisplayName ?? m.HomePlayerId!),
                (m.AwayPlayerId!, m.AwayPlayer?.DisplayName ?? m.AwayPlayerId!)
            ])
            .DistinctBy(p => p.Id)
            .ToDictionary(p => p.Id, p => p.Name);

        string? bestPlayer = null;
        int bestStreak = 0;

        foreach (var (playerId, playerName) in playerNames)
        {
            int current = 0, max = 0;
            foreach (var m in ordered.Where(m => m.HomePlayerId == playerId || m.AwayPlayerId == playerId))
            {
                bool won = (m.HomePlayerId == playerId && m.HomeScore > m.AwayScore) ||
                           (m.AwayPlayerId == playerId && m.AwayScore > m.HomeScore);
                if (won) { current++; if (current > max) max = current; }
                else current = 0;
            }

            if (max > bestStreak) { bestStreak = max; bestPlayer = playerName; }
        }

        recap.LongestStreakPlayer = bestPlayer;
        recap.LongestStreak = bestStreak;
        return recap;
    }
}
