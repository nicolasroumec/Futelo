# TODO — Sesión 7: League

## Sprint 1 — Server: Repository + Service base
- [x] `ILeagueRepository` + `LeagueRepository` (GetById con Players y Matches)
- [x] `ILeagueService` + `LeagueService` (esqueleto con DI)
- [x] Registrar en `Program.cs`

## Sprint 2 — Server: Generación de fixture
- [x] `LeagueService.GenerateFixtureAsync` — genera fixture round-robin
  - Soporta cantidad impar (bye automático)
  - Si `IsHomeAndAway`: duplica el fixture invirtiendo local/visitante
  - Crea los `Match` con `Status = Pending`, setea `League.Status = Active`
- [x] `LeagueService.RegenerateFixtureAsync` — reshuffle mientras no hay resultados

## Sprint 3 — Server: Resultado + Tabla + ELO
- [x] `LeagueService.RecordResultAsync`
  - Valida match de esta liga y `Pending`
  - Guarda scores, `Status = Played`, `PlayedAt`
  - Actualiza `SeasonPlayer.SeasonElo` y `AppUser.EloRating`
  - K=32, multiplicador por diferencia de goles (×1.0 / ×1.2 / ×1.5)
  - 4 entradas `EloHistory` por partido (2 por jugador: season + histórico)
  - Al terminar: `League.Status = Finished` + graba `LeaguePlayer.LeaguePosition`
- [x] `LeagueService.GetStandingsAsync` — Pts / GD / GF / head-to-head

## Sprint 4 — Server: Controller + DTOs
- [x] DTOs: `LeagueResponse`, `MatchResponse`, `StandingRow`, `RecordResultRequest`, `EloChangeResult`, `RecordResultResponse`
- [x] `LeagueController`:
  - `GET /api/leagues/{id}` — fixture + standings
  - `POST /api/leagues/{id}/start` — genera fixture
  - `PUT /api/leagues/{id}/reshuffle` — re-sorteo
  - `PUT /api/leagues/{id}/matches/{matchId}/result` — carga resultado con ELO change en respuesta

## Sprint 5 — Client
- [x] `ILeagueService` + `LeagueService` (HTTP calls)
- [x] `League/LeagueView.razor` — tabla + fixture agrupado por fecha + entrada de resultados inline + feedback ELO
- [x] Link a la liga desde `SeasonDetail` (badge clickeable)
- [x] `LeagueId` agregado a `SeasonResponse`
- [x] `_Imports.razor` actualizado
