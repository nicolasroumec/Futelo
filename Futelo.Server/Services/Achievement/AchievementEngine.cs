using Futelo.Server.Models;
using Futelo.Server.Repositories.Achievement;
using Futelo.Shared.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Futelo.Server.Services.Achievement;

public class AchievementEngine(
    IAchievementRepository achievementRepo,
    IAchievementEvaluationRepository evalRepo,
    IServiceScopeFactory scopeFactory) : IAchievementEngine
{
    public Task EvaluateAfterMatchAsync(MatchAchievementContext ctx)
        => Task.WhenAll(
            EvaluatePlayerInScopeAsync(ctx, isHome: true),
            EvaluatePlayerInScopeAsync(ctx, isHome: false));

    private async Task EvaluatePlayerInScopeAsync(MatchAchievementContext ctx, bool isHome)
    {
        using var scope = scopeFactory.CreateScope();
        var ar = scope.ServiceProvider.GetRequiredService<IAchievementRepository>();
        var er = scope.ServiceProvider.GetRequiredService<IAchievementEvaluationRepository>();
        await EvaluatePlayerAfterMatchAsync(ctx, isHome, ar, er);
    }

    private static async Task EvaluatePlayerAfterMatchAsync(MatchAchievementContext ctx, bool isHome,
        IAchievementRepository achievementRepo, IAchievementEvaluationRepository evalRepo)
    {
        string playerId = isHome ? ctx.HomePlayerId : ctx.AwayPlayerId;
        int myNewHistElo = isHome ? ctx.HomeNewHistElo : ctx.AwayNewHistElo;
        int myOldHistElo = isHome ? ctx.HomeOldHistElo : ctx.AwayOldHistElo;
        int theirOldHistElo = isHome ? ctx.AwayOldHistElo : ctx.HomeOldHistElo;
        int myScore = isHome ? ctx.HomeScore : ctx.AwayScore;
        int theirScore = isHome ? ctx.AwayScore : ctx.HomeScore;
        bool wonMatch = ctx.WonOnPenaltiesId == playerId ||
                        (ctx.WonOnPenaltiesId == null && myScore > theirScore);

        var unlocked = await achievementRepo.GetUnlockedTypesAsync(playerId, ctx.VaultId);
        var toAward = new List<PlayerAchievement>();
        var now = DateTime.UtcNow;

        void Award(AchievementType type)
        {
            if (!unlocked.Contains(type))
                toAward.Add(new PlayerAchievement { PlayerId = playerId, VaultId = ctx.VaultId, Type = type, UnlockedAt = now, SeasonId = ctx.SeasonId });
        }

        // ELO milestone badges (no extra queries needed)
        if (myNewHistElo >= 1600) Award(AchievementType.ElitePlayer);
        if (myNewHistElo >= 1700) Award(AchievementType.TopPlayer);
        if (myNewHistElo >= 1800) Award(AchievementType.Legend);

        // Won and opponent was 200+ ELO higher
        if (wonMatch && theirOldHistElo - myOldHistElo >= 200)
            Award(AchievementType.GiantKiller);

        // Scored 5+ goals in a single match
        if (myScore >= 5)
            Award(AchievementType.Poacher);

        // Won a final without conceding
        if (ctx.IsFinal && wonMatch && theirScore == 0)
            Award(AchievementType.FinalCleanSheet);

        // Streak and clean-sheet badges require last N matches
        bool needsStreaks =
            !unlocked.Contains(AchievementType.OnFire) ||
            !unlocked.Contains(AchievementType.Unstoppable) ||
            !unlocked.Contains(AchievementType.ComebackKing) ||
            !unlocked.Contains(AchievementType.TheEqualizer) ||
            !unlocked.Contains(AchievementType.CleanSheetStreak3) ||
            !unlocked.Contains(AchievementType.CleanSheetStreak5) ||
            !unlocked.Contains(AchievementType.CleanSheetStreak7);

        if (needsStreaks)
        {
            var last = await evalRepo.GetPlayerLastMatchesInVaultAsync(playerId, ctx.VaultId, 15);

            if (!unlocked.Contains(AchievementType.OnFire) && last.Count >= 5
                && last.Take(5).All(m => Won(m, playerId)))
                Award(AchievementType.OnFire);

            if (!unlocked.Contains(AchievementType.Unstoppable) && last.Count >= 10
                && last.Take(10).All(m => Won(m, playerId)))
                Award(AchievementType.Unstoppable);

            // ComebackKing: last 5 all wins AND the 3 before that all losses
            if (!unlocked.Contains(AchievementType.ComebackKing) && last.Count >= 8
                && last.Take(5).All(m => Won(m, playerId))
                && last.Skip(5).Take(3).All(m => Lost(m, playerId)))
                Award(AchievementType.ComebackKing);

            if (!unlocked.Contains(AchievementType.TheEqualizer) && last.Count >= 5
                && last.Take(5).All(m => Drew(m, playerId)))
                Award(AchievementType.TheEqualizer);

            if (!unlocked.Contains(AchievementType.CleanSheetStreak3) && last.Count >= 3
                && last.Take(3).All(m => ConcededGoals(m, playerId) == 0))
                Award(AchievementType.CleanSheetStreak3);

            if (!unlocked.Contains(AchievementType.CleanSheetStreak5) && last.Count >= 5
                && last.Take(5).All(m => ConcededGoals(m, playerId) == 0))
                Award(AchievementType.CleanSheetStreak5);

            if (!unlocked.Contains(AchievementType.CleanSheetStreak7) && last.Count >= 7
                && last.Take(7).All(m => ConcededGoals(m, playerId) == 0))
                Award(AchievementType.CleanSheetStreak7);
        }

        // Recovered from <= 1450 to >= 1600 historical ELO
        if (!unlocked.Contains(AchievementType.Phoenix) && myNewHistElo >= 1600)
        {
            var minHistElo = await evalRepo.GetMinHistoricalEloInVaultAsync(playerId, ctx.VaultId);
            if (minHistElo.HasValue && minHistElo.Value <= 1450)
                Award(AchievementType.Phoenix);
        }

        // 10 consecutive historical ELO changes all while ranked #1
        if (!unlocked.Contains(AchievementType.KingOfTheHill))
        {
            var lastElos = await evalRepo.GetPlayerLastHistoricalEloInVaultAsync(playerId, ctx.VaultId, 10);
            if (lastElos.Count >= 10 && lastElos.All(e => e.RankAfter == 1))
                Award(AchievementType.KingOfTheHill);
        }

        if (!unlocked.Contains(AchievementType.Veteran))
        {
            var count = await evalRepo.CountMatchesInVaultAsync(playerId, ctx.VaultId);
            if (count >= 100) Award(AchievementType.Veteran);
        }

        if (!unlocked.Contains(AchievementType.IceVeins) && ctx.WonOnPenaltiesId == playerId)
        {
            var penWins = await evalRepo.CountPenaltyWinsInVaultAsync(playerId, ctx.VaultId);
            if (penWins >= 3) Award(AchievementType.IceVeins);
        }

        if (!unlocked.Contains(AchievementType.Sniper) && myScore == 1 && theirScore == 0 && wonMatch)
        {
            var wins1_0 = await evalRepo.CountExact1_0WinsInVaultAsync(playerId, ctx.VaultId);
            if (wins1_0 >= 10) Award(AchievementType.Sniper);
        }

        if (toAward.Count > 0)
            await achievementRepo.AwardManyAsync(toAward);
    }

    public async Task EvaluateAfterSeasonAsync(int seasonId, int vaultId)
    {
        var season = await evalRepo.GetSeasonWithCompetitionsAsync(seasonId);
        if (season == null) return;

        var seasonMatches = await evalRepo.GetSeasonMatchesAsync(seasonId);
        var seasonPlayers = await evalRepo.GetSeasonPlayersAsync(seasonId);
        var allVaultSeasons = await evalRepo.GetVaultSeasonsWithChampionsAsync(vaultId);
        var histElos = await evalRepo.GetSeasonEloHistoriesAsync(seasonId, isSeasonElo: false);
        var seasonElos = await evalRepo.GetSeasonEloHistoriesAsync(seasonId, isSeasonElo: true);

        string? leagueChampId = season.League?.ChampionId;
        string? cupChampId = season.Cup?.ChampionId;
        string? superCupChampId = season.SuperCup?.ChampionId;

        var leagueMatches = seasonMatches.Where(m => m.LeagueId != null).ToList();

        // Historical ELO at season start = EloBefore of each player's first hist entry
        var startHistEloByPlayer = histElos
            .GroupBy(e => e.PlayerId)
            .ToDictionary(g => g.Key, g => g.OrderBy(e => e.CreatedAt).First().EloBefore);

        // Final season ELO = EloAfter of each player's last season ELO entry
        var finalSeasonEloByPlayer = seasonElos
            .GroupBy(e => e.PlayerId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(e => e.CreatedAt).First().EloAfter);

        var goalsByPlayer = new Dictionary<string, int>();
        foreach (var m in seasonMatches)
        {
            if (m.HomePlayerId != null)
                goalsByPlayer[m.HomePlayerId] = goalsByPlayer.GetValueOrDefault(m.HomePlayerId) + (m.HomeScore ?? 0);
            if (m.AwayPlayerId != null)
                goalsByPlayer[m.AwayPlayerId] = goalsByPlayer.GetValueOrDefault(m.AwayPlayerId) + (m.AwayScore ?? 0);
        }

        var now = DateTime.UtcNow;

        foreach (var sp in seasonPlayers)
        {
            string playerId = sp.PlayerId;
            var unlocked = await achievementRepo.GetUnlockedTypesAsync(playerId, vaultId);
            var toAward = new List<PlayerAchievement>();

            void Award(AchievementType type)
            {
                if (!unlocked.Contains(type))
                    toAward.Add(new PlayerAchievement { PlayerId = playerId, VaultId = vaultId, Type = type, UnlockedAt = now, SeasonId = seasonId });
            }

            bool wonLeague = leagueChampId == playerId;
            bool wonCup = cupChampId == playerId;
            bool wonSuperCup = superCupChampId == playerId;
            bool wonAny = wonLeague || wonCup || wonSuperCup;

            // Title badges
            if (wonAny)
            {
                int leagueWins = allVaultSeasons.Count(s => s.League?.ChampionId == playerId);
                int cupWins = allVaultSeasons.Count(s => s.Cup?.ChampionId == playerId);
                int superCupWins = allVaultSeasons.Count(s => s.SuperCup?.ChampionId == playerId);
                int totalWins = leagueWins + cupWins + superCupWins;

                if (totalWins == 1) Award(AchievementType.FirstTitle);
                if (wonLeague && wonCup && wonSuperCup) Award(AchievementType.Treble);
                if (leagueWins >= 3 || cupWins >= 3 || superCupWins >= 3) Award(AchievementType.HatTrickTitles);
                if (totalWins >= 5) Award(AchievementType.Dynasty);
                if (leagueWins >= 1 && cupWins >= 1 && superCupWins >= 1) Award(AchievementType.ChampionOfChampions);

                // Won league with the lowest historical ELO at season start
                if (wonLeague && startHistEloByPlayer.Count > 1
                    && startHistEloByPlayer.TryGetValue(playerId, out int myStart)
                    && myStart == startHistEloByPlayer.Values.Min())
                    Award(AchievementType.Cinderella);
            }

            // League performance badges
            if (leagueMatches.Count > 0)
            {
                var myLeague = leagueMatches
                    .Where(m => m.HomePlayerId == playerId || m.AwayPlayerId == playerId)
                    .OrderBy(m => m.PlayedAt)
                    .ToList();

                if (myLeague.Count >= 4)
                {
                    bool noLoss = !myLeague.Any(m => Lost(m, playerId));
                    bool noDraw = !myLeague.Any(m => Drew(m, playerId));
                    bool noGoalConceded = !myLeague.Any(m => ConcededGoals(m, playerId) > 0);

                    if (noLoss) Award(AchievementType.Invincible);
                    if (noLoss && noDraw) Award(AchievementType.PerfectSeason);
                    if (noGoalConceded) Award(AchievementType.BrickWall);

                    // Won league with below-average historical ELO at season start
                    if (wonLeague
                        && startHistEloByPlayer.TryGetValue(playerId, out int startElo)
                        && startElo < startHistEloByPlayer.Values.Average())
                        Award(AchievementType.UnderdogStory);

                    // Won league despite having a 3+ match losing streak at some point
                    if (wonLeague && MaxConsecutiveLosses(myLeague, playerId) >= 3)
                        Award(AchievementType.NeverGiveUp);

                    if (!unlocked.Contains(AchievementType.MrConsistent) && myLeague.Count >= 10)
                    {
                        double winRate = (double)myLeague.Count(m => Won(m, playerId)) / myLeague.Count;
                        if (winRate >= 0.6 && MaxConsecutiveLosses(myLeague, playerId) <= 2)
                            Award(AchievementType.MrConsistent);
                    }
                }
            }

            // Gained 100+ season ELO (started at 1500, ended at 1600+)
            if (finalSeasonEloByPlayer.TryGetValue(playerId, out int finalSElo) && finalSElo >= 1600)
                Award(AchievementType.MeteoricRise);

            // Top scorer across all season matches with 20+ goals
            if (goalsByPlayer.TryGetValue(playerId, out int myGoals) && myGoals >= 20
                && myGoals == goalsByPlayer.Values.Max())
                Award(AchievementType.GoldenBoot);

            if (toAward.Count > 0)
                await achievementRepo.AwardManyAsync(toAward);
        }
    }

    private static int MaxConsecutiveLosses(List<Match> matches, string playerId)
    {
        int max = 0, current = 0;
        foreach (var m in matches)
        {
            if (Lost(m, playerId)) { current++; if (current > max) max = current; }
            else current = 0;
        }
        return max;
    }

    private static bool Won(Match m, string playerId)
    {
        if (m.WonOnPenaltiesId != null) return m.WonOnPenaltiesId == playerId;
        return m.HomePlayerId == playerId ? m.HomeScore > m.AwayScore : m.AwayScore > m.HomeScore;
    }

    private static bool Lost(Match m, string playerId)
    {
        if (m.WonOnPenaltiesId != null) return m.WonOnPenaltiesId != playerId;
        return m.HomePlayerId == playerId ? m.HomeScore < m.AwayScore : m.AwayScore < m.HomeScore;
    }

    private static bool Drew(Match m, string playerId) => !Won(m, playerId) && !Lost(m, playerId);

    private static int ConcededGoals(Match m, string playerId)
        => m.HomePlayerId == playerId ? (m.AwayScore ?? 0) : (m.HomeScore ?? 0);
}
