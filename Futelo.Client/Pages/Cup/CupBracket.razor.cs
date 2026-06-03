using Futelo.Client.Services.Language;
using Futelo.Shared.DTOs.Cup;
using Microsoft.AspNetCore.Components;

namespace Futelo.Client.Pages.Cup;

public partial class CupBracket
{
    [Inject] private ILanguageService Lang { get; set; } = null!;
    [Parameter] public CupResponse Cup { get; set; } = null!;

    private const int SlotHeight = 76;

    private int MaxTieCount => Cup.Rounds.Max(r => TieCount(r));

    private int TieCount(CupRoundResponse round) =>
        Cup.IsHomeAndAway ? round.Matches.Count / 2 : round.Matches.Count;

    private List<List<CupMatchResponse>> GetTies(CupRoundResponse round)
    {
        var ordered = round.Matches.OrderBy(m => m.Id).ToList();
        if (!Cup.IsHomeAndAway)
            return ordered.Select(m => new List<CupMatchResponse> { m }).ToList();

        var ties = new List<List<CupMatchResponse>>();
        for (int i = 0; i < ordered.Count; i += 2)
        {
            var tie = new List<CupMatchResponse> { ordered[i] };
            if (i + 1 < ordered.Count) tie.Add(ordered[i + 1]);
            ties.Add(tie);
        }
        return ties;
    }

    private record TieDisplay(
        string P1Id, string P1Name, bool P1Tbd,
        string P2Id, string P2Name, bool P2Tbd,
        string? Score1, string? Score2,
        string? WinnerId, bool IsDecided);

    private TieDisplay GetTieDisplay(List<CupMatchResponse> tie, int roundNumber)
    {
        var first = tie[0];
        string p1Id = first.HomePlayerId ?? "";
        string p1Name = first.HomePlayerName;
        bool p1Tbd = string.IsNullOrEmpty(first.HomePlayerId);
        string p2Id = first.AwayPlayerId ?? "";
        string p2Name = first.AwayPlayerName;
        bool p2Tbd = string.IsNullOrEmpty(first.AwayPlayerId);

        string? score1 = null, score2 = null;
        bool isDecided = false;

        if (!Cup.IsHomeAndAway)
        {
            if (first.Status == "Played")
            {
                score1 = first.HomeScore?.ToString();
                score2 = first.AwayScore?.ToString();
                isDecided = true;
            }
        }
        else
        {
            var leg1 = tie.FirstOrDefault(m => m.Leg == 1) ?? tie[0];
            var leg2 = tie.Count > 1 ? (tie.FirstOrDefault(m => m.Leg == 2) ?? tie[1]) : null;
            if (leg1.Status == "Played" && leg2?.Status == "Played")
            {
                int p1Goals = (leg1.HomeScore ?? 0) + (leg2!.AwayScore ?? 0);
                int p2Goals = (leg1.AwayScore ?? 0) + (leg2.HomeScore ?? 0);
                score1 = p1Goals.ToString();
                score2 = p2Goals.ToString();
                isDecided = true;
            }
        }

        string? winnerId = FindWinner(tie, roundNumber);
        return new TieDisplay(p1Id, p1Name, p1Tbd, p2Id, p2Name, p2Tbd, score1, score2, winnerId, isDecided);
    }

    private string? FindWinner(List<CupMatchResponse> tie, int roundNumber)
    {
        string? p1Id = tie[0].HomePlayerId;
        string? p2Id = tie[0].AwayPlayerId;
        if (string.IsNullOrEmpty(p1Id) || string.IsNullOrEmpty(p2Id)) return null;

        var laterMatches = Cup.Rounds
            .Where(r => r.RoundNumber > roundNumber)
            .SelectMany(r => r.Matches);

        foreach (var m in laterMatches)
        {
            if (m.HomePlayerId == p1Id || m.AwayPlayerId == p1Id) return p1Id;
            if (m.HomePlayerId == p2Id || m.AwayPlayerId == p2Id) return p2Id;
        }

        if (Cup.ChampionId == p1Id) return p1Id;
        if (Cup.ChampionId == p2Id) return p2Id;
        return null;
    }
}
