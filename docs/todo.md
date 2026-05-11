# TODO — Sesión 10 (continuación): Estadísticas avanzadas

## Sprint 1 — DTOs
- [ ] `VideoGameStatsRow` — VideoGameName, Played, Won, Drawn, Lost, GoalsFor, GoalsAgainst
- [ ] `EloHistoryPoint` — Date, Elo
- [ ] `ScorerRow` — Position, PlayerId, DisplayName, Goals
- [ ] `VaultRecordsResponse` — BestWinStreak y BestUnbeatenStreak (jugador + cantidad)
- [ ] `PlayerStatsResponse` — agregar `CurrentStreak`, `BestWinStreak`, `BestUnbeatenStreak`; reemplazar `TopGames` (VideoGameUsageRow) por `GameStats` (VideoGameStatsRow)

## Sprint 2 — Server: Repository
- [ ] `GetPlayerEloHistoryInVaultAsync(playerId, vaultId)` — EloHistory histórico ordenado por fecha
- [ ] `GetAllPlayedMatchesInVaultAsync(vaultId)` — todos los partidos jugados en el vault con HomePlayer/AwayPlayer

## Sprint 3 — Server: Service + Controller
- [ ] `GetPlayerStatsAsync` — computar `CurrentStreak`, `BestWinStreak`, `BestUnbeatenStreak` y `GameStats` desde los matches ya cargados
- [ ] `GetEloHistoryAsync` → `GET api/stats/vaults/{vaultId}/players/{playerId}/elo-history`
- [ ] `GetScorersAsync` → `GET api/stats/vaults/{vaultId}/scorers`
- [ ] `GetVaultRecordsAsync` → `GET api/stats/vaults/{vaultId}/records`

## Sprint 4 — Client: Service
- [ ] `GetEloHistoryAsync(vaultId, playerId)`
- [ ] `GetScorersAsync(vaultId)`
- [ ] `GetVaultRecordsAsync(vaultId)`

## Sprint 5 — Client: PlayerProfile actualizado
- [ ] Mostrar `CurrentStreak`, `BestWinStreak`, `BestUnbeatenStreak`
- [ ] Tabla `GameStats` (reemplaza TopGames)
- [ ] Gráfico de línea ELO via Chart.js (JSInterop)
  - [ ] Agregar Chart.js CDN a `wwwroot/index.html`
  - [ ] Crear `wwwroot/js/charts.js` con `renderEloChart(canvasId, labels, data)`
  - [ ] `OnAfterRenderAsync` en `PlayerProfile.razor.cs`

## Sprint 6 — Client: Scorers + VaultRecords + links
- [ ] `Stats/Scorers.razor` + `.razor.cs` — `/vaults/{vaultId}/scorers`
- [ ] `Stats/VaultRecords.razor` + `.razor.cs` — `/vaults/{vaultId}/records`
- [ ] Links desde `VaultDetail` (Scorers, Records)
