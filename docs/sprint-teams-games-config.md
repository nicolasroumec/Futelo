# Sprint Plan — Configuración de Juego y Equipos por Temporada

## Contexto

Los campos `Match.VideoGameId`, `Match.HomeTeamId` y `Match.AwayTeamId` existen en el modelo
pero nunca se populan. El objetivo es evitar cargar esos datos a mano en cada partido.

**Solución acordada:**
- `Season.VideoGameId` — el juego se configura una vez a nivel de temporada.
- `SeasonPlayer.TeamId` — cada jugador tiene un equipo por defecto en esa temporada.
- Al registrar un resultado (League, Cup, SuperCup), el backend auto-popula
  `Match.VideoGameId`, `Match.HomeTeamId` y `Match.AwayTeamId` desde esos valores.

**Equipos:** globales (sin scoping por juego).  
**Temporada → Juego:** opcional, configurable al crear/editar la temporada.

---

## Lo que ya está hecho (branch 9-stats)

Las siguientes features de stats ya están implementadas y commiteadas:

- `TeamUsageRow` extendido con W-D-L y goles por equipo (perfil de jugador).
- `GET /api/stats/vaults/{id}/teams` — Team Panel (equipos más usados, quién los usó, W-D-L).
- `GET /api/stats/vaults/{id}/games` — Games Ranking (mejores jugadores por videojuego).
- Componentes Blazor: `TeamPanel.razor`, `GamesRanking.razor`.
- Links en `VaultDetail` hacia Teams y Games.

---

## Sprints pendientes

### Sprint A — Modelo de datos + migración
**1 commit**

- Agregar `VideoGameId` (nullable int, FK → VideoGame) a `Season`.
- Agregar `TeamId` (nullable int, FK → Team) a `SeasonPlayer`.
- Actualizar `FuteloContext`: configurar relaciones (DeleteBehavior.SetNull).
- Generar migración: `dotnet ef migrations add AddVideoGameToSeasonAndTeamToSeasonPlayer`.

Archivos:
- `Futelo.Server/Models/Season.cs`
- `Futelo.Server/Models/SeasonPlayer.cs`
- `Futelo.Server/Data/FuteloContext.cs`
- Nueva migración en `Futelo.Server/Migrations/`

```
feat: add VideoGameId to Season and TeamId to SeasonPlayer
```

---

### Sprint B — Backend: configurar juego en la temporada
**2 commits**

**Commit 1 — DTOs**
- Agregar `VideoGameId?` y `VideoGameName?` a `SeasonResponse` (para mostrarlo en la UI).
- Agregar `VideoGameId?` a `CreateSeasonRequest` o crear `UpdateSeasonVideoGameRequest`.

Archivos:
- `Futelo.Shared/DTOs/Season/SeasonResponse.cs`
- `Futelo.Shared/DTOs/Season/CreateSeasonRequest.cs` (o nuevo DTO)

```
feat: add VideoGameId to Season DTOs
```

**Commit 2 — Service + endpoint**
- Actualizar `SeasonService.CreateSeasonAsync` para guardar `VideoGameId`.
- Agregar `PATCH /api/seasons/{id}/video-game` para cambiar el juego después de crear.
- Actualizar `SeasonRepository` si hace falta incluir `VideoGame` en las queries.

Archivos:
- `Futelo.Server/Services/Season/SeasonService.cs`
- `Futelo.Server/Controllers/SeasonController.cs`
- `Futelo.Server/Repositories/Season/SeasonRepository.cs`

```
feat: add video game configuration to Season service and endpoint
```

---

### Sprint C — Backend: equipo por defecto por jugador en la temporada
**2 commits**

**Commit 1 — DTOs**
- Agregar `TeamId?` y `TeamName?` a `SeasonPlayerResponse`.
- Crear `SetSeasonPlayerTeamRequest { int? TeamId }`.

Archivos:
- `Futelo.Shared/DTOs/Season/SeasonPlayerResponse.cs` (o donde esté)
- `Futelo.Shared/DTOs/Season/SetSeasonPlayerTeamRequest.cs` (nuevo)

```
feat: add TeamId to SeasonPlayer DTOs
```

**Commit 2 — Service + endpoint**
- Agregar `PATCH /api/seasons/{id}/players/{playerId}/team` para asignar el equipo.
- Actualizar `SeasonService` / `SeasonRepository` para guardar y retornar `TeamId`.

Archivos:
- `Futelo.Server/Services/Season/SeasonService.cs`
- `Futelo.Server/Controllers/SeasonController.cs`
- `Futelo.Server/Repositories/Season/SeasonRepository.cs`

```
feat: add default team assignment per player in Season
```

---

### Sprint D — Auto-popular VideoGame y Teams al registrar resultado
**1 commit** (el más importante)

En `LeagueService.RecordResultAsync`, `CupService.RecordResultAsync` y
`SuperCupService.RecordResultAsync`:

1. Al registrar el resultado, cargar `Season.VideoGameId`.
2. Cargar `SeasonPlayer.TeamId` para el jugador local (home) y visitante (away).
3. Setear `Match.VideoGameId`, `Match.HomeTeamId`, `Match.AwayTeamId` antes de guardar.

Si el jugador no tiene equipo asignado en la temporada, el campo queda null (sin error).

Archivos:
- `Futelo.Server/Services/League/LeagueService.cs`
- `Futelo.Server/Services/Cup/CupService.cs`
- `Futelo.Server/Services/SuperCup/SuperCupService.cs`
- Posiblemente los repositorios correspondientes si necesitan incluir Season/SeasonPlayer.

```
feat: auto-populate VideoGame and Teams on match result from Season config
```

---

### Sprint E — Frontend: selector de juego en la temporada
**2 commits**

**Commit 1 — Client service**
- Agregar método en `IVideoGameService` / `VideoGameService` para listar todos los juegos
  (`GET /api/videogames`), si no existe ya.
- Agregar `PatchSeasonVideoGameAsync(seasonId, videoGameId?)` en el cliente de Season.

```
feat: add video game selector client service methods
```

**Commit 2 — UI**
- En el formulario de creación de temporada: agregar `<select>` con los videojuegos disponibles.
- En el detalle de temporada (si ya está en estado Draft/Active): mostrar el juego asignado
  con opción de cambiarlo.

Archivos:
- `Futelo.Client/Pages/Season/CreateSeason.razor` + `.razor.cs`
- `Futelo.Client/Pages/Season/SeasonDetail.razor` + `.razor.cs`

```
feat: add VideoGame selector to Season create and detail UI
```

---

### Sprint F — Frontend: selector de equipo por jugador en la temporada
**2 commits**

**Commit 1 — Client service**
- Agregar `GetTeamsAsync()` en `ITeamService` / `TeamService` si no existe.
- Agregar `SetSeasonPlayerTeamAsync(seasonId, playerId, teamId?)`.

```
feat: add team selector client service methods for SeasonPlayer
```

**Commit 2 — UI**
- En el detalle de temporada, para cada jugador de la lista: agregar un `<select>` con los
  equipos disponibles y guardar al cambiar.
- Mostrar el equipo actual asignado.

Archivos:
- `Futelo.Client/Pages/Season/SeasonDetail.razor` + `.razor.cs`

```
feat: add default team selector per player in Season detail UI
```

---

## Orden de implementación recomendado

```
A → B → C → D → E → F
```

D depende de A+B+C (necesita los datos en la base para leerlos).  
E y F pueden hacerse en paralelo si se quiere.

## Notas

- `DeleteBehavior.SetNull` en ambas FKs nuevas: si se borra un VideoGame o Team,
  los campos quedan null en lugar de fallar.
- Los partidos ya jugados antes de configurar el juego/equipo quedan con null — es esperado.
- El override por partido (cambiar equipo/juego en un partido específico) queda para el futuro.
