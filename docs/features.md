# Futelo – Feature Registry

Status: ✅ done · 🔄 in progress · 📋 planned

## Core

| Feature | Status | Key files |
|---------|--------|-----------|
| Auth (register / login / JWT) | ✅ | `AuthController`, `AuthService`, `Pages/Login.razor`, `Pages/Register.razor` |
| Vault CRUD + player list | ✅ | `VaultController`, `VaultService`, `Pages/VaultDetail.razor` |
| Vault invitations (email token) | ✅ | `InvitationsController`, `InvitationService`, `Pages/AcceptInvitation.razor` |
| Invite by link (F4) | ✅ | `InvitationsController.GetPreview`, `Pages/InviteJoin.razor` |
| Season recap page (F5) | ✅ | `SeasonRecapService`, `GET /api/seasons/{id}/recap`, `Pages/SeasonRecap.razor` |
| Season lifecycle (Draft→Active→Finished) | ✅ | `SeasonController`, `SeasonService`, `Pages/SeasonDetail.razor` |
| Season configuration UI | ✅ | `Pages/SeasonCompetitionConfig.razor` + `.razor.css` |

## League

| Feature | Status | Key files |
|---------|--------|-----------|
| Fixture generation (round-robin) | ✅ | `LeagueService.GenerateFixtureAsync` |
| Fixture reshuffle (before any result) | ✅ | `LeagueService.RegenerateFixtureAsync` |
| Record result + ELO update | ✅ | `LeagueService.RecordResultAsync` |
| Standings (computed on the fly) | ✅ | `Server/Helpers/StandingsCalculator.cs` |
| Tiebreaker rule (GD / H2H / H2H+GD) | ✅ | `TiebreakerRule` enum, `LeagueService` |
| Home-and-away mode | ✅ | `League.IsHomeAndAway` |
| Matchday tabs + pending dot indicator | ✅ | `Pages/League/LeagueView.razor` |
| Pending matches per player summary | ✅ | `LeagueView.razor` — `PendingByPlayer` |
| Inline match edit (team / video game / date) | ✅ | `Shared/MatchEditPanel.razor` |

## Cup

| Feature | Status | Key files |
|---------|--------|-----------|
| Bracket generation | ✅ | `CupService.GenerateBracketAsync` |
| Seeding modes (SeasonElo / LeaguePosition / Random) | ✅ | `CupSeedingMode` enum, `CupService` |
| Home-and-away legs | ✅ | `Cup.IsHomeAndAway`, `CupView.razor` — `GetTies()` |
| Away goal rule | ✅ | `Cup.AwayGoalRule`, `CupService.RecordResultAsync` |
| Penalties support | ✅ | `WonOnPenaltiesId`, `Shared/MatchResultForm.razor` |
| Aggregate score display | ✅ | `CupView.razor` — `ComputeAggregate()` |
| Inline match edit | ✅ | `Shared/MatchEditPanel.razor` |

## SuperCup

| Feature | Status | Key files |
|---------|--------|-----------|
| 1v1 match (league champ vs cup champ) | ✅ | `SuperCupService.StartAsync` |
| Home-and-away (2 legs) | ✅ | `SuperCup.IsHomeAndAway` |
| Penalties support | ✅ | `SuperCupView.razor` |
| Head-to-head stats display | ✅ | `SuperCupView.razor`, `StatsService.GetHeadToHeadAsync` |
| Inline match edit | ✅ | `Shared/MatchEditPanel.razor` |

## Dates (F1)

| Feature | Status | Key files |
|---------|--------|-----------|
| StartDate / EndDate on all competitions | ✅ | Models + DTOs (`Vault`, `Season`, `League`, `Cup`, `SuperCup`) |
| ScheduledDate per match | ✅ | `Match.ScheduledDate`, `MatchEditPanel` |
| Inline date editor — Season | ✅ | `Pages/SeasonDetail.razor` |
| Inline date editor — League | ✅ | `Pages/League/LeagueView.razor` |
| Inline date editor — Cup | ✅ | `Pages/Cup/CupView.razor` |
| Inline date editor — SuperCup | ✅ | `Pages/SuperCup/SuperCupView.razor` |

## Stats

| Feature | Status | Key files |
|---------|--------|-----------|
| General ranking (global ELO) | ✅ | `Stats/GeneralRanking.razor` |
| Season ranking (season ELO) | ✅ | `Stats/Ranking.razor` |
| Palmarés (titles per player) | ✅ | `Stats/Palmares.razor` |
| ELO history chart | ✅ | `Pages/Player/PlayerProfile.razor` |
| Head-to-head | ✅ | `Pages/Player/HeadToHead.razor` |
| Scorers | ✅ | `Stats/Scorers.razor` |
| Vault records (longest streak, etc.) | ✅ | `Stats/VaultRecords.razor` |
| Games ranking (by video game) | ✅ | `Stats/GamesRanking.razor` |
| Team panel | ✅ | `Stats/TeamPanel.razor` |
| Recent form | ✅ | `PlayerProfile.razor` |

## UI / UX

| Feature | Status | Key files |
|---------|--------|-----------|
| Theme toggle (light / dark) | ✅ | CSS variables, `Shared/ThemeToggle.razor` |
| Skeleton loaders | ✅ | `Shared/SkeletonTable.razor`, inline skeleton blocks |
| Toast notifications | ✅ | `Services/Toast/`, `Shared/ToastContainer.razor` |
| PWA (offline + installable) | ✅ | `wwwroot/manifest.json`, `wwwroot/service-worker.js` |
| PWA update prompt | ✅ | `Shared/PwaUpdater.razor` |
| Offline indicator | ✅ | `Shared/OfflineIndicator.razor` |
| i18n (ES / EN) | ✅ | `wwwroot/i18n/`, `Services/Language/LanguageService.cs` |

## API surface (all routes require JWT)

```
POST   /api/auth/register
POST   /api/auth/login

GET    /api/vaults
POST   /api/vaults
GET    /api/vaults/{id}
PUT    /api/vaults/{id}
DELETE /api/vaults/{id}
GET    /api/vaults/{id}/matches
GET    /api/vaults/{id}/recent-matches
POST   /api/vaults/{id}/invite

GET    /api/invitations/{token}          (public)
POST   /api/invitations/{token}/accept

GET    /api/seasons/{id}/recap           (public)

GET    /api/seasons?vaultId={id}
POST   /api/seasons
GET    /api/seasons/{id}
PUT    /api/seasons/{id}/configure
PUT    /api/seasons/{id}/activate
PUT    /api/seasons/{id}/finish
DELETE /api/seasons/{id}
PATCH  /api/seasons/{id}/dates
PATCH  /api/seasons/{id}/video-game
PATCH  /api/seasons/{id}/players/{playerId}/team

GET    /api/leagues/{id}
POST   /api/leagues/{id}/start
PUT    /api/leagues/{id}/reshuffle
PATCH  /api/leagues/{id}/dates
PATCH  /api/leagues/{id}/matches/{matchId}
PUT    /api/leagues/{id}/matches/{matchId}/result

GET    /api/cups/{id}
POST   /api/cups/{id}/start
PATCH  /api/cups/{id}/dates
PATCH  /api/cups/{id}/matches/{matchId}
PUT    /api/cups/{id}/matches/{matchId}/result

GET    /api/supercups/{id}
POST   /api/supercups/{id}/start
PATCH  /api/supercups/{id}/dates
PATCH  /api/supercups/{id}/matches/{matchId}
PUT    /api/supercups/{id}/matches/{matchId}/result

GET    /api/stats/vaults/{vaultId}/players/{playerId}
GET    /api/stats/vaults/{vaultId}/ranking
GET    /api/stats/vaults/{vaultId}/h2h?player1Id=&player2Id=
GET    /api/stats/vaults/{vaultId}/seasons/{seasonId}/ranking
GET    /api/stats/vaults/{vaultId}/palmares
GET    /api/stats/vaults/{vaultId}/scorers
GET    /api/stats/vaults/{vaultId}/records
GET    /api/stats/vaults/{vaultId}/games
GET    /api/stats/vaults/{vaultId}/teams
GET    /api/stats/vaults/{vaultId}/players/{playerId}/elo-history
GET    /api/stats/vaults/{vaultId}/players/{playerId}/recent-matches
GET    /api/stats/vaults/{vaultId}/players/{playerId}/matches
GET    /api/stats/vaults/{vaultId}/players/{playerId}/recent-form
GET    /api/stats/vaults/{vaultId}/players/{playerId}/records
GET    /api/stats/vaults/{vaultId}/records/top-scoring-match

GET    /api/teams
POST   /api/teams
DELETE /api/teams/{id}

GET    /api/videogames
POST   /api/videogames
DELETE /api/videogames/{id}
```
