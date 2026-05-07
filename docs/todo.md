# TODO — Sesión 7: League

## Sprint 1 — Server: Repository + Service base
- [ ] `ILeagueRepository` + `LeagueRepository` (GetById con Players y Matches)
- [ ] `ILeagueService` + `LeagueService` (esqueleto con DI)
- [ ] Registrar en `Program.cs`

## Sprint 2 — Server: Generación de fixture

- [ ] `LeagueService.GenerateFixtureAsync` — genera fixture round-robin al activar la liga
  - Soporta cantidad impar (bye automático — jugador vs null)
  - Si `IsHomeAndAway`: duplica el fixture invirtiendo local/visitante
  - Crea los `Match` con `Status = Pending`
- [ ] Re-sorteo: solo permitido mientras `League.Status = NotStarted` (ningún partido Played)

## Sprint 3 — Server: Resultado + Tabla + ELO

- [ ] `LeagueService.RecordResultAsync(matchId, homeScore, awayScore)`
  - Valida que el match sea de esta liga y esté `Pending`
  - Guarda scores, `Status = Played`, `PlayedAt`
  - Actualiza ELO: `SeasonPlayer.SeasonElo` **y** `AppUser.EloRating` (histórico)
    - K=32, multiplicador por diferencia de goles (×1.0 / ×1.2 / ×1.5)
  - Graba `EloHistory` (dos entradas por jugador: `IsSeasonElo=true` y `IsSeasonElo=false`)
  - Si todos los matches están `Played` → `League.Status = Finished`
- [ ] `LeagueService.GetStandingsAsync` — tabla de posiciones
  - Puntos (W=3, D=1, L=0)
  - Desempate: diferencia de goles → goles a favor → head-to-head

## Sprint 4 — Server: Controller + DTOs

- [ ] DTOs: `LeagueResponse`, `StandingRow`, `MatchResponse`, `RecordResultRequest`, `EloChangeResult`
- [ ] `LeagueController`:
  - `GET /api/leagues/{id}` — fixture + standings
  - `POST /api/leagues/{id}/start` — genera fixture y activa liga
  - `PUT /api/leagues/{id}/start` — re-sorteo (solo si `NotStarted`)
  - `PUT /api/leagues/{id}/matches/{matchId}/result` — carga resultado
    - Respuesta incluye `EloChangeResult` (ELO antes/después por jugador)

## Sprint 5 — Client

- [ ] `ILeagueService` + `LeagueService` (HTTP calls)
- [ ] `League/LeagueView.razor` — tabla de posiciones + fixture agrupado por fecha
- [ ] Link a la liga desde `SeasonDetail`
