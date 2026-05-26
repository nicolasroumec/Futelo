# Futelo – Domain Model

## Entity hierarchy

```
Vault
└── Season  (Draft → Active → Finished)
    ├── League?
    │   └── Match[]         (LeagueId set)
    ├── Cup?
    │   └── CupRound[]
    │       └── Match[]     (CupRoundId set)
    └── SuperCup?
        └── Match[]         (SuperCupId set)
```

A `Vault` is a group of players (friends). Each `Season` belongs to one vault and can have up to three competitions: a round-robin `League`, a knockout `Cup`, and a `SuperCup` (1v1 final between league champion and cup champion). All three are optional and configured before the season starts.

## Season lifecycle

1. **Draft** – owner configures players and competitions via `PUT /api/seasons/{id}/configure`.
2. **Active** – competitions are generated. Each competition starts `NotStarted` and must be started individually.
3. **Finished** – manually triggered once all competitions are done.

Competition status: `NotStarted → Active → Finished`.

## Key models

### Match
Single row used by all competition types. Discriminated by which FK is non-null (`LeagueId`, `CupRoundId`, `SuperCupId`). `Leg` = 1 or 2 for home-and-away formats. `WonOnPenaltiesId` is set when a Cup/SuperCup match is decided on penalties.

### League
Round-robin. `IsHomeAndAway` doubles the fixture. `TiebreakerRule` resolves equal-points situations:
- `GoalDifference` (default)
- `HeadToHead`
- `HeadToHeadThenGoalDifference`

Standings are computed on the fly by `StandingsCalculator.Compute()` (server helper).

### Cup
Single-elimination bracket. Key config:
- `SeedingMode`: `SeasonElo` (default) | `LeaguePosition` | `Random`
- `IsHomeAndAway`: each tie is two legs; away goal rule applies in non-final rounds when `AwayGoalRule = true`
- `AwayGoalRule`: only triggers when `roundFromEnd > 0` (not the final)

### SuperCup
Fixed 1v1 between League champion and Cup champion. Optional home-and-away (2 legs). If players aren't resolved yet when the SuperCup is configured, `Player1Id`/`Player2Id` are null and filled in when `POST /api/supercups/{id}/start` is called.

## ELO

Every recorded match result updates both players' ELO. `EloHistory` rows are appended for each match. `SeasonElo` is tracked per `SeasonPlayer` row and resets each season. Global ELO lives on `AppUser`.

## Auth

JWT Bearer. All controllers are `[Authorize]`. `UserId` = `ClaimTypes.NameIdentifier` (ASP.NET Identity). Ownership checks are done in services, not controllers. `CanEdit = (vault.OwnerId == userId)` is computed server-side and exposed on response DTOs.

## Invitations

`VaultInvitation` is created with a token and sent by email. Accepting adds the user to `VaultPlayer`. Route: `POST /api/vaults/{id}/invite` + `POST /api/invitations/accept`.
