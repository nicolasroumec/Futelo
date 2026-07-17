# Tarea: mover el ELO histórico de global (AppUser) a por-vault (VaultPlayer)

## Objetivo

Hoy el ELO histórico vive en `AppUser.EloRating` — un único valor **global** por
usuario, compartido entre todos los vaults. Eso es incorrecto: cada vault es un
grupo de jugadores aislado (pools disjuntos), y un ranking histórico global mezcla
poblaciones que nunca jugaron entre sí.

El histórico debe ser **por vault**: un jugador tiene un ELO histórico distinto en
cada vault al que pertenece, que se arrastra entre temporadas dentro de ese vault.

El grano correcto es `VaultPlayer` (fila por (VaultId, PlayerId)).

**Decisión de datos ya tomada: reset a 1500.** La columna nueva arranca en
`EloCalculator.InitialElo` (1500) para todos. NO se hace backfill desde `EloHistory`.
Se pierde el ranking histórico actual (aceptado — no hay datos productivos que preservar).

## Contexto que ya está resuelto

- **Todo consumidor del histórico ya tiene `vaultId` en contexto.** `GetPlayerStatsAsync`,
  rankings, seeding de copa — todos reciben `vaultId`. No hay ninguna pantalla
  verdaderamente global. Por eso el cambio encaja sin agregar parámetros a la API pública.
- **`season.Vault.Players` (colección de `VaultPlayer`) ya viene cargada** en los tres
  repos de competición vía `.Include(...).ThenInclude(s => s.Vault).ThenInclude(v => v.Players)`:
  - `CupRepository.cs:13`
  - `LeagueRepository.cs:13`
  - `SuperCupRepository.cs:13`
  Así que en los servicios se puede leer el ELO histórico desde `season.Vault.Players`.
- **`RollbackMatchEloAsync(matchId, p1, p2, seasonId)`** lo llaman los tres servicios
  (League:220, Cup:216, SuperCup:118) siempre con `seasonId`. Derivando `vaultId` desde
  `seasonId` dentro del repo se cubren los tres sin tocar firmas.
- El núcleo del cálculo (`EloCalculator.Compute`) **no cambia**. Es correcto.

## El cálculo NO cambia

`Futelo.Server/Helpers/EloCalculator.cs` queda igual. Solo cambia **de dónde se lee** y
**a dónde se escribe** el ELO histórico (el "hist" en el código). El ELO de temporada
(`SeasonPlayer.SeasonElo`) NO se toca — ya está bien.

---

## Cambios, archivo por archivo

### 1. Modelos

**`Futelo.Server/Models/VaultPlayer.cs`** — agregar la propiedad:
```csharp
using Futelo.Server.Helpers; // agregar el using
// ...
public int EloRating { get; set; } = EloCalculator.InitialElo;
```

**`Futelo.Server/Models/AppUser.cs:9`** — eliminar:
```csharp
public int EloRating { get; set; } = EloCalculator.InitialElo;  // BORRAR
```
(Se puede quitar el `using Futelo.Server.Helpers;` si queda sin uso.)

### 2. Migración EF

Crear una migración nueva (`dotnet ef migrations add EloPerVault -p Futelo.Server`):
- Agrega columna `EloRating` (int, NOT NULL, default 1500) a la tabla `VaultPlayers`.
- Elimina columna `EloRating` de `AspNetUsers`.

Verificar que EF la genere sola a partir de los cambios de modelo. Si hace falta default
explícito en la columna nueva, usar `defaultValue: 1500`.

### 3. Servicios — lado LECTURA (reemplazar `sp.Player.EloRating` por VaultPlayer)

En cada servicio, construir un dict con los ELO históricos del vault:
```csharp
var vaultElos = season.Vault.Players.ToDictionary(vp => vp.PlayerId, vp => vp.EloRating);
```
y leer de ahí en lugar de `homesp.Player.EloRating` / `awaysp.Player.EloRating` /
`seasonPlayers.ToDictionary(... sp.Player.EloRating)`.

**`Futelo.Server/Services/League/LeagueService.cs`**
- `ComputeEloBlock` (líneas ~338, 339): `homesp.Player.EloRating` → `vaultElos[homesp.PlayerId]`,
  idem away. El método recibe `seasonPlayers`; pasarle también `vaultElos` (o el `season`).
- Línea ~351: el dict `histElos` se arma desde `sp.Player.EloRating` → usar `vaultElos`.
- Líneas ~264, 266 (contexto de achievements `HomeOldHistElo` / `AwayOldHistElo`):
  `homesp.Player.EloRating` → `vaultElos[homesp.PlayerId]`.

**`Futelo.Server/Services/Cup/CupService.cs`**
- Línea ~62 (seeding `SeedingMode.SeasonElo`... ojo: usa histórico): 
  `seasonPlayers.OrderByDescending(sp => sp.Player.EloRating)` → ordenar por `vaultElos[sp.PlayerId]`.
- Líneas ~254, 255: `homesp.Player.EloRating` / `awaysp.Player.EloRating` → `vaultElos[...]`.
- Línea ~267: `histElos` desde `sp.Player.EloRating` → `vaultElos`.

**`Futelo.Server/Services/SuperCup/SuperCupService.cs`**
- Líneas ~142, 143: → `vaultElos[...]`.
- Línea ~155: `histElos` → `vaultElos`.

> El `vaultId` en cada servicio es `season.VaultId` (League usa `league.Season`,
> Cup `cup.Season`, SuperCup `superCup.Season`).

### 4. Servicios — pasar VaultId al record de escritura

Los tres records de resultado necesitan `VaultId` para que el repo sepa a qué
`VaultPlayer` escribir:

- **`Futelo.Server/Repositories/League/MatchResultData.cs`** — agregar `public int VaultId { get; init; }`.
- **`Futelo.Server/Repositories/Cup/CupMatchResultData.cs`** — idem.
- **`Futelo.Server/Repositories/SuperCup/SuperCupMatchResultData.cs`** — idem.

Y en cada servicio, al construir el record (LeagueService ~239, y equivalentes en Cup/SuperCup),
setear `VaultId = season.VaultId`.

### 5. Repositorios — lado ESCRITURA (reemplazar write a AppUser por VaultPlayer)

**`Futelo.Server/Repositories/League/LeagueRepository.cs:73-79`** — reemplazar:
```csharp
var homeUser = await Context.Set<AppUser>().FindAsync(data.HomePlayerId);
if (homeUser != null) homeUser.EloRating = data.HomeNewHistoricalElo;
var awayUser = await Context.Set<AppUser>().FindAsync(data.AwayPlayerId);
if (awayUser != null) awayUser.EloRating = data.AwayNewHistoricalElo;
```
por:
```csharp
var homeVp = await Context.Set<VaultPlayer>()
    .FirstOrDefaultAsync(vp => vp.VaultId == data.VaultId && vp.PlayerId == data.HomePlayerId);
if (homeVp != null) homeVp.EloRating = data.HomeNewHistoricalElo;
var awayVp = await Context.Set<VaultPlayer>()
    .FirstOrDefaultAsync(vp => vp.VaultId == data.VaultId && vp.PlayerId == data.AwayPlayerId);
if (awayVp != null) awayVp.EloRating = data.AwayNewHistoricalElo;
```

**`Futelo.Server/Repositories/Cup/CupRepository.cs:83-87`** — mismo patrón.
**`Futelo.Server/Repositories/SuperCup/SuperCupRepository.cs:71-75`** — mismo patrón.

### 6. Rollback

**`Futelo.Server/Repositories/Shared/EloRollbackRepository.cs:41-45`** — la rama `else`
(`!IsSeasonElo`) hoy hace:
```csharp
var user = await context.Set<AppUser>().FindAsync(h.PlayerId);
if (user != null) user.EloRating = h.EloBefore;
```
Cambiar a escribir el `VaultPlayer`. El método ya recibe `seasonId`; derivar el `vaultId`
una vez al principio:
```csharp
var vaultId = await context.Set<Season>().Where(s => s.Id == seasonId).Select(s => s.VaultId).FirstAsync();
```
y en la rama:
```csharp
var vp = await context.Set<VaultPlayer>()
    .FirstOrDefaultAsync(vp => vp.VaultId == vaultId && vp.PlayerId == h.PlayerId);
if (vp != null) vp.EloRating = h.EloBefore;
```
(No cambia la firma de `RollbackMatchEloAsync` ni sus llamadores.)

### 7. Stats (lecturas del histórico)

**`Futelo.Server/Repositories/Stats/StatsRepository.cs:49`** (ranking general):
```csharp
.OrderByDescending(vp => vp.Player.EloRating)   →   .OrderByDescending(vp => vp.EloRating)
```

**`Futelo.Server/Services/Stats/StatsService.cs:214`** (ranking general):
```csharp
HistoricalElo = vp.Player.EloRating   →   HistoricalElo = vp.EloRating
```

**`Futelo.Server/Services/Stats/StatsService.cs:232`** (ranking de temporada, columna histórico):
`sp.Player.EloRating` es SeasonPlayer→AppUser. Necesita el ELO del `VaultPlayer` de ese vault.
Traer un dict de VaultPlayer del vault y mapear por `PlayerId`. Agregar método en
`StatsRepository` (ej. `GetVaultEloMapAsync(int vaultId)` que devuelva
`Dictionary<string,int>` de `VaultPlayers` filtrados por vault), y en `GetRankingAsync`
usar `vaultElos[sp.PlayerId]` para `HistoricalElo`.

**`Futelo.Server/Services/Stats/StatsService.cs:134`** (perfil de jugador, ya recibe `vaultId`):
`EloRating = player.EloRating` → traer el `VaultPlayer` de `(vaultId, playerId)` y usar su
`EloRating`. Agregar en `StatsRepository` un `GetVaultPlayerAsync(vaultId, playerId)` o
reutilizar `GetVaultEloMapAsync`.

### 8. Vault

**`Futelo.Server/Services/Vault/VaultService.cs:202`** — `p` ya es un `VaultPlayer`:
```csharp
EloRating = p.Player.EloRating   →   EloRating = p.EloRating
```

### 9. DTOs y cliente — SIN cambios

`VaultPlayerResponse.EloRating`, `PlayerStatsResponse.EloRating`, `RankingRow.HistoricalElo`
mantienen la misma forma; solo cambia la fuente en el servidor. El cliente Blazor no se toca
(`VaultDetail.razor:122`, `PlayerProfile`, `PlayerCompare`, etc. consumen los mismos DTOs).

### 10. Docs a actualizar

- `docs/domain.md:51-52` — "Global ELO lives on AppUser" → ahora por `VaultPlayer`.
- `docs/modelo-de-datos.md:25` y `:252` — mover `EloRating` de `AppUser` a `VaultPlayer`.
- `docs/roadmap.md:159` — "Ranking general del vault por `AppUser.EloRating`" → `VaultPlayer.EloRating`.

---

## Checklist de verificación

1. `dotnet build` sin errores (compilar toda la solución).
2. `dotnet test` — `EloCalculatorTests` debe seguir pasando sin cambios (el cálculo no se tocó).
3. Aplicar la migración a una DB limpia y verificar: columna `EloRating` en `VaultPlayers`,
   ausente en `AspNetUsers`.
4. Prueba de humo end-to-end (usar el skill `/verify` o correr la app):
   - Cargar un resultado de liga → ambos jugadores cambian `VaultPlayer.EloRating` de ese vault,
     no de otros vaults en los que estén.
   - Un jugador en dos vaults mantiene ELO histórico independiente por vault.
   - Corregir el último partido (rollback) → `VaultPlayer.EloRating` vuelve al valor previo.
   - Ranking general y perfil de jugador muestran el ELO por vault.

## Grep de control (no deben quedar referencias al histórico global)

Al terminar, `grep -rn "Player.EloRating\|AppUser.*EloRating\|\.EloRating"` en
`Futelo.Server` no debe encontrar ningún **histórico** apuntando a `AppUser`. Las únicas
apariciones válidas de `EloRating` deben ser sobre `VaultPlayer`. `AppUser` ya no tiene la propiedad.
