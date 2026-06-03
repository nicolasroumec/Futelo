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
| Manual match entry (F7) | ✅ | `LeagueService.StartManualAsync` / `AddMatchManuallyAsync` |
| Correct last played result (ELO rollback) | ✅ | `LeagueService`, `MatchEditPanel` — `IsLastPlayed` |

## Cup

| Feature | Status | Key files |
|---------|--------|-----------|
| Bracket generation | ✅ | `CupService.GenerateBracketAsync` |
| Seeding modes (SeasonElo / LeaguePosition / Random) | ✅ | `CupSeedingMode` enum, `CupService` |
| Home-and-away legs | ✅ | `Cup.IsHomeAndAway`, `CupView.razor` — `GetTies()` |
| Away goal rule | ✅ | `Cup.AwayGoalRule`, `CupService.RecordResultAsync` |
| Visual bracket view | ✅ | `Pages/Cup/CupView.razor` — round toggle + horizontal scroll |
| Manual match entry (F8) | ✅ | `CupService.StartManualAsync` / `AddRoundAsync` / `AddMatchAsync` |
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
| Manual match entry (F9) | ✅ | `SuperCupService.StartManualAsync` |

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

## Backlog

### Match status lifecycle

**Scope:** Add a `MatchStatus` enum (`NotStarted | InProgress | Finished`) to the `Match` model. All existing matches default to `Finished` on migration.

**State machine:**
```
NotStarted → InProgress → Finished
```
- `NotStarted` → `InProgress`: vault member marks the match as started.
- `InProgress` → `Finished`: vault member records the final result (triggers ELO update).

**Edit / correction rules:**
- Score can be freely updated while `InProgress` (no ELO committed yet).
- Once `Finished`, result is locked **unless** it is the most recent finished match for **both** players in the vault. In that case the vault owner can reset it to `InProgress`, which rolls back the single ELO delta for that match. Multi-match rollback is out of scope.
- `NotStarted` matches show no score; `InProgress` matches show a live score badge; `Finished` matches show the final result as today.

**Provisional standings (future):**
- While a match is `InProgress`, standings/ELO could be projected with the current score. Requires a read-only recalculation endpoint — no SignalR, no real-time push. Deferred until match status is stable.

**API changes needed:**
```
PATCH  /api/leagues/{id}/matches/{matchId}/status      (NotStarted → InProgress)
PATCH  /api/cups/{id}/matches/{matchId}/status
PATCH  /api/supercups/{id}/matches/{matchId}/status
PUT    /api/leagues/{id}/matches/{matchId}/result       (InProgress → Finished, same endpoint)
```

**Other backlog items:**
| Feature | Priority | Notes |
|---------|----------|-------|
| ~~Remove player from vault~~ | ~~High~~ | ~~done~~ |
| ~~Clone season config~~ | ~~Medium~~ | ~~done~~ |
| Push notifications (PWA) | Medium | Service Worker already in place; notify players about upcoming scheduled matches |
| Transfer vault ownership | Low | Owner hands off admin rights to another vault member |
| Cross-season stats comparison | Low | Table comparing a player's key stats (ELO delta, W/L, goals) across all seasons |
| **Player avatars + team shields** | **Medium** | **See full plan below** |

### Player avatars + team shields (planned)

Store images as `byte[]` in DB (no external file storage). Images are resized **client-side** via JS Canvas API interop before upload — no server-side image library needed.

**Technical decisions:**
- Avatar: 128×128px WebP ~5KB per user
- Shield: 64×64px WebP ~2KB per team
- Max upload: 5MB input
- Queries: bytes excluded by default via `.Select()`; served via dedicated endpoints
- Frontend fallback: CSS circle with initials when no image set
- DTOs receive `AvatarUrl`/`ShieldUrl` string pointing to serve endpoint (not Base64 inline)

**New endpoints:**
```
PUT    /api/users/me/avatar
DELETE /api/users/me/avatar
GET    /api/users/{id}/avatar
PUT    /api/teams/{id}/shield
DELETE /api/teams/{id}/shield
GET    /api/teams/{id}/shield
```

**Display locations — avatar:** player profile (large), vault player list, standings/rankings (32px), match cards (32px), H2H.
**Display locations — shield:** MatchEditPanel team selector, match display, stats team panel.

**Implementation increments:**

| # | Scope | Commit message |
|---|-------|---------------|
| ~~1~~ | ~~Backend: `byte[]` fields on `AppUser`/`Team`, migration, 6 endpoints, update queries to exclude bytes, add `AvatarUrl`/`ShieldUrl` to DTOs~~ | ~~`feat: add avatar and team shield storage endpoints`~~ |
| ~~2~~ | ~~Reusable `<ImageUpload>` component (drag & drop, preview, validation) + `<PlayerAvatar>` component with initials fallback + avatar upload on player profile page~~ | ~~`feat: image upload component and player avatar on profile page`~~ |
| ~~3~~ | ~~`<PlayerAvatar>` throughout app: standings, rankings, vault player list, match cards, H2H~~ | ~~`feat: show player avatars across standings, rankings and match views`~~ |
| ~~4~~ | ~~`<TeamShield>` component + upload in team management + display in MatchEditPanel, match display, team panel~~ | ~~`feat: team shield upload and display in match and stats views`~~ |

---

### UX / UI fixes (from branch 16-ux-ui audit)

| Item | Type | Detail |
|------|------|--------|
| ~~`PageTitle` hardcoded in competition views~~ | ~~Bug~~ | ~~done~~ |
| ~~"Recap" / "Copy recap link" not i18n~~ | ~~Bug~~ | ~~done~~ |
| ~~Scorers missing from VaultDetail stats row~~ | ~~Missing~~ | ~~done~~ |
| ~~Dashboard empty state uses inline card~~ | ~~Inconsistency~~ | ~~done~~ |
| ~~No current-user row highlight in standings~~ | ~~UX~~ | ~~done~~ |
| ~~Not-found states are bare text~~ | ~~UX~~ | ~~done~~ |
| ~~Active season badge doesn't link~~ | ~~UX~~ | ~~done~~ |

---

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
