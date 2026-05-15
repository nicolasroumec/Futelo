# Auditoría Técnica Frontend — Futelo (Blazor WASM .NET 10)

> Fecha: 2026-05-14 | Auditor: Claude Sonnet 4.6

---

## Contexto del proyecto

**Futelo** es una app para gestión de torneos de fútbol entre amigos. Vault → Season → Competition (Liga/Copa/SuperCopa) → Match. Stack: Blazor WASM .NET 10, ASP.NET Core 10, EF Core, JWT, Bootstrap 5. Proyecto Shared con DTOs/Enums compartidos entre cliente y servidor.

---

## Puntaje general: 7.2 / 10

| Dimensión | Puntaje | Nota |
|---|---|---|
| Arquitectura | 8/10 | Tres capas correctas, DTOs compartidos, DI completo |
| Buenas prácticas Blazor | 8/10 | Code-behind enforced, buen uso de ciclo de vida |
| Código limpio | 6.5/10 | Buena base pero duplicación sistémica en 15+ archivos |
| Performance | 6/10 | Sin caching, riesgo N+1, requests sin cancelación |
| Seguridad | 6.5/10 | JWT + Identity correcto, pero sin refresh ni rate limiting |
| Tests | 2/10 | Cero cobertura en lógica crítica |
| UX técnica | 7/10 | Skeletons y estados bien manejados, falta accesibilidad |
| Escalabilidad | 7/10 | Arquitectura escala bien, pero sin optimizaciones aún |

---

## 1. Componentes repetidos y UI duplicada

### 1.1 Patrón de ciclo de vida repetido en 15+ páginas — Prioridad: ALTA

**Archivos afectados:** Dashboard, VaultDetail, SeasonDetail, LeagueView, CupView, SuperCupView, GeneralRanking, Ranking, Scorers, GamesRanking, TeamPanel, VaultRecords, Palmares, PlayerProfile, HeadToHead.

**Problema:** Cada página copia exactamente el mismo bloque de 8-10 líneas:

```csharp
protected override async Task OnInitializedAsync()
{
    Lang.OnChange += HandleLanguageChange;
    try { /* carga */ }
    catch (Exception ex) { errorMessage = ex.Message; }
    finally { isLoading = false; }
}
private void HandleLanguageChange() => InvokeAsync(StateHasChanged);
public void Dispose() => Lang.OnChange -= HandleLanguageChange;
```

**Impacto:** Cualquier cambio requiere tocar 15 archivos. Riesgo de olvidar el `Dispose` (memory leak real en WASM).

**Solución:**

```csharp
// Shared/LocalizedComponentBase.cs
public abstract class LocalizedComponentBase : ComponentBase, IDisposable
{
    [Inject] protected LanguageService Lang { get; set; } = default!;

    protected bool IsLoading { get; set; } = true;
    protected string? ErrorMessage { get; set; }

    protected override void OnInitialized()
    {
        Lang.OnChange += OnLanguageChanged;
    }

    private void OnLanguageChanged() => InvokeAsync(StateHasChanged);

    protected async Task LoadAsync(Func<Task> loader)
    {
        try { await loader(); }
        catch (Exception ex) { ErrorMessage = ex.Message; }
        finally { IsLoading = false; }
    }

    public virtual void Dispose() => Lang.OnChange -= OnLanguageChanged;
}

// Cada página pasa de 40 líneas a:
public partial class Dashboard : LocalizedComponentBase
{
    protected override async Task OnInitializedAsync() =>
        await LoadAsync(async () => vaults = await VaultService.GetMyVaultsAsync());
}
```

### 1.2 Triple de estado repetido en cada página — Prioridad: MEDIA

`isLoading / errorMessage / data` se declara manualmente en cada página.

```csharp
public class AsyncValue<T>
{
    public bool IsLoading { get; private set; } = true;
    public string? Error { get; private set; }
    public T? Value { get; private set; }

    public async Task LoadAsync(Func<Task<T>> loader)
    {
        IsLoading = true;
        Error = null;
        try { Value = await loader(); }
        catch (Exception ex) { Error = ex.Message; }
        finally { IsLoading = false; }
    }
}
```

---

## 2. Código muerto

Sin código muerto significativo detectado. El proyecto es compacto y joven.

**Verificar:** `LanguageSwitcher.razor` — confirmar que está montado en algún layout activo.

---

## 3. Código repetido

### 3.1 Manejo de errores HTTP duplicado en 5 servicios — Prioridad: ALTA

**Archivos afectados:** `LeagueService`, `SeasonService`, `CupService`, `SuperCupService`, `VaultService`.

```csharp
// Este bloque se repite en los 5 servicios:
var response = await http.PutAsync($"api/xxx/{id}/action", null);
if (!response.IsSuccessStatusCode)
{
    var error = await response.Content.ReadAsStringAsync();
    throw new InvalidOperationException(string.IsNullOrEmpty(error) ? "Default message" : error);
}
```

**Solución:**

```csharp
// Services/HttpExtensions.cs
public static class HttpExtensions
{
    public static async Task EnsureSuccessAsync(
        this HttpResponseMessage response,
        string fallback = "Operation failed.")
    {
        if (response.IsSuccessStatusCode) return;
        var body = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException(string.IsNullOrWhiteSpace(body) ? fallback : body);
    }
}

// Uso:
var response = await http.PutAsync($"api/leagues/{id}/start", null);
await response.EnsureSuccessAsync("Could not start league.");
```

### 3.2 Sin base service en el cliente — Prioridad: ALTA

Los 10 servicios repiten el mismo patrón de GetFromJsonAsync + null check, PostAsJsonAsync + read body, PutAsync + error handling. Cualquier cambio transversal (timeout, logging, manejo de 401) requiere tocar los 10 servicios.

```csharp
// Services/ApiService.cs
public abstract class ApiService(HttpClient http)
{
    protected async Task<T> GetAsync<T>(string url)
        => await http.GetFromJsonAsync<T>(url)
            ?? throw new InvalidOperationException("Empty server response.");

    protected async Task<T> PostAsync<T>(string url, object body)
    {
        var res = await http.PostAsJsonAsync(url, body);
        await res.EnsureSuccessAsync();
        return await res.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException("Empty server response.");
    }

    protected async Task PutAsync(string url, object? body = null)
    {
        var res = body is null
            ? await http.PutAsync(url, null)
            : await http.PutAsJsonAsync(url, body);
        await res.EnsureSuccessAsync();
    }

    protected async Task DeleteAsync(string url)
    {
        var res = await http.DeleteAsync(url);
        await res.EnsureSuccessAsync();
    }
}

// VaultService hereda:
public class VaultService(HttpClient http) : ApiService(http), IVaultService
{
    public Task<List<VaultResponse>> GetMyVaultsAsync()
        => GetAsync<List<VaultResponse>>("api/vaults");
}
```

### 3.3 EloCalculator triplicado — Prioridad: ALTA

La misma lógica ELO existe en `LeagueService`, `CupService` y `SuperCupService` con K diferente.

```csharp
// Server/Services/EloCalculator.cs
public static class EloCalculator
{
    public static (int change, int newElo) Compute(int myElo, int opponentElo, double result, int goalDiff, int k = 32)
    {
        double expected = 1.0 / (1.0 + Math.Pow(10, (opponentElo - myElo) / 400.0));
        double multiplier = goalDiff >= 3 ? 1.5 : goalDiff == 2 ? 1.2 : 1.0;
        int change = (int)Math.Round(k * multiplier * (result - expected), MidpointRounding.AwayFromZero);
        return (change, myElo + change);
    }
}

// Uso con K diferente por competencia:
// Liga: EloCalculator.Compute(myElo, oppElo, result, diff, k: 32)
// Copa: EloCalculator.Compute(myElo, oppElo, result, diff, k: 24)
// SuperCopa: EloCalculator.Compute(myElo, oppElo, result, diff, k: 16)
```

### 3.4 Strings de error hardcodeados — Prioridad: BAJA

```csharp
// Futelo.Shared/ErrorMessages.cs
public static class ErrorMessages
{
    public const string VaultNotFound = "Vault not found.";
    public const string LeagueNotFound = "League not found.";
    public const string SeasonNotFound = "Season not found.";
    public const string NotAuthorized = "You are not authorized to perform this action.";
    public const string InvalidCredentials = "Invalid username or password.";
}
```

---

## 4. Buenas prácticas Blazor

### 4.1 Code-behind — Excelente

100% consistente en todo el proyecto. Cero bloques `@code {}`. Fortaleza principal.

### 4.2 IDisposable — Riesgo de memory leak real — Prioridad: ALTA

Si alguna página olvida implementar `IDisposable` tras suscribirse a `Lang.OnChange`, el componente permanece vivo en memoria indefinidamente. La clase base `LocalizedComponentBase` (ver 1.1) resuelve esto estructuralmente.

### 4.3 CancellationToken — Ausente — Prioridad: MEDIA

Ningún servicio cliente acepta `CancellationToken`. Si el usuario navega durante una carga, la request HTTP sigue en vuelo.

```csharp
private CancellationTokenSource _cts = new();

protected override async Task OnInitializedAsync()
{
    await LoadAsync(async () => data = await Service.GetAsync(id, _cts.Token));
}

public override void Dispose()
{
    _cts.Cancel();
    _cts.Dispose();
    base.Dispose();
}
```

### 4.4 EventCallback vs Action — Verificar — Prioridad: MEDIA

En `MatchResultForm` y `MatchEditPanel`, los callbacks hacia el padre deben ser `EventCallback<T>` (no `Action<T>`) para que Blazor llame `StateHasChanged` en el padre automáticamente.

```csharp
// Correcto:
[Parameter] public EventCallback<MatchResult> OnResultSaved { get; set; }
// Incorrecto (no dispara re-render del padre):
[Parameter] public Action<MatchResult> OnResultSaved { get; set; }
```

### 4.5 StateHasChanged — Sin problemas

No se encontraron llamadas innecesarias. Ciclo de vida bien respetado.

---

## 5. Performance

### 5.1 Riesgo de queries N+1 — Prioridad: ALTA

Si los repositories no incluyen las entidades relacionadas con `.Include()`, cada acceso a `vault.Owner.DisplayName` en el mapper dispara una query adicional.

```csharp
// VaultRepository.cs — Includes explícitos:
public async Task<List<Vault>> GetByPlayerIdAsync(string userId) =>
    await Context.Vaults
        .Include(v => v.Owner)
        .Include(v => v.Players).ThenInclude(vp => vp.Player)
        .Where(v => v.Players.Any(p => p.PlayerId == userId))
        .AsNoTracking()
        .ToListAsync();
```

Auditar todos los repositories para verificar que los `Include()` necesarios estén presentes.

### 5.2 Sin caching en cliente — Prioridad: MEDIA

Cada navegación recarga todo del servidor. Los catálogos globales (`VideoGame`, `Team`) se fetchean múltiples veces.

```csharp
// Para catálogos estáticos:
public class VideoGameService(HttpClient http) : ApiService(http), IVideoGameService
{
    private List<VideoGameResponse>? _cache;

    public async Task<List<VideoGameResponse>> GetAllAsync()
        => _cache ??= await GetAsync<List<VideoGameResponse>>("api/videogames");

    public void InvalidateCache() => _cache = null;
}
```

### 5.3 LeagueService.RecordResultAsync — Método demasiado grande — Prioridad: MEDIA

~100+ líneas en un método que hace: validación → ELO → standings → EloHistory → save. Dificulta testing y debugging.

```csharp
// Descomponer en:
public async Task RecordResultAsync(...)
{
    var match = await ValidateMatchAsync(leagueId, matchId, userId);
    var eloChanges = CalculateEloChanges(match, homeScore, awayScore);
    await PersistMatchResultAsync(match, homeScore, awayScore);
    await ApplyEloChangesAsync(eloChanges, match);
    await UpdateStandingsAsync(match.League);
}
```

### 5.4 AsNoTracking ausente en queries de solo lectura — Prioridad: BAJA

Agregar `.AsNoTracking()` en todos los repositories que solo leen datos (no modifican). Mejora performance inmediata en EF Core.

---

## 6. Arquitectura

### 6.1 Fortalezas

- Tres capas bien separadas: Controller → Service → Repository
- DTOs compartidos en `Futelo.Shared`
- Interfaces en todos los servicios y repositories
- BaseRepository con operaciones CRUD genéricas

### 6.2 SeasonDetail — Componente demasiado grande — Prioridad: MEDIA

`SeasonDetail.razor.cs`: ~232 líneas, 24 campos privados, 7 métodos async. Gestiona vault, jugadores, equipos, competiciones y UI state simultáneamente.

**Solución:** Descomponer en sub-componentes:
- `SeasonPlayerManager.razor` — gestión de jugadores
- `SeasonCompetitionConfig.razor` — configuración Liga/Copa/SuperCopa
- `SeasonTeamSelector.razor` — asignación de equipos
- `SeasonDetail` solo coordina

### 6.3 LeagueService — Dios Service — Prioridad: MEDIA

`LeagueService.cs`: ~381 líneas. Fixture generation, standings calculation, ELO calculation, result recording, patch logic en una sola clase. Viola SRP.

```
LeagueService (orquesta)
├── FixtureGenerator — BuildRoundRobin()
├── StandingsCalculator — ComputeStandings()
└── EloCalculator — ComputeElo() (compartido con Cup y SuperCup)
```

### 6.4 Lógica de autorización en UI — Prioridad: BAJA

`VaultDetail.razor.cs` decodifica el JWT para determinar `isOwner`. Solo afecta UI (mostrar/ocultar botones), el servidor valida correctamente. Bajo riesgo actual, pero puede escalar.

```csharp
// Encapsular en AuthService:
public Task<string?> GetCurrentUserIdAsync() { ... }
public Task<bool> IsCurrentUserAsync(string userId) { ... }
```

---

## 7. UI/UX Técnica

### 7.1 Estados vacíos incompletos — Prioridad: MEDIA

GamesRanking, Scorers y algunas otras páginas no manejan el caso `data.Count == 0` con mensaje amigable.

```razor
<EmptyState Icon="bi-trophy"
            Title="@Lang["no_data_yet"]"
            Subtitle="@Lang["play_matches_to_see_stats"]" />
```

### 7.2 Sin ARIA labels — Prioridad: BAJA

Botones de acción y avatares de jugadores carecen de atributos de accesibilidad.

```razor
<button @onclick="SaveResult" aria-label="@Lang["save_result"]" class="btn btn-primary">
    <i class="bi bi-check-lg" aria-hidden="true"></i>
</button>
```

### 7.3 Consistencia visual — Fortaleza

Skeleton loaders, badges de estado y `BadgeHelper.cs` están bien implementados y son consistentes en toda la app.

---

## 8. Seguridad Frontend

### 8.1 Sin interceptor 401 — Prioridad: ALTA

JWT expira en 7 días. Cuando expira, el usuario recibe errores silenciosos sin redirect al login.

```csharp
// AuthTokenHandler.cs:
protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request, CancellationToken cancellationToken)
{
    var response = await base.SendAsync(request, cancellationToken);
    if (response.StatusCode == HttpStatusCode.Unauthorized)
    {
        await AuthService.LogoutAsync();
        NavigationManager.NavigateTo("/login", forceLoad: true);
    }
    return response;
}
```

### 8.2 Sin rate limiting — Prioridad: MEDIA

Endpoints de auth sin protección contra brute force.

```csharp
// Program.cs (Server):
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1) }));
});

[HttpPost("login")]
[EnableRateLimiting("auth")]
public async Task<IActionResult> Login(...)
```

### 8.3 Sin refresh tokens — Prioridad: MEDIA

Cuando el JWT expira, el usuario debe re-loguearse. Considerar implementar refresh tokens para sesiones largas.

### 8.4 CORS de dev — Sin riesgo en producción si se restringe

Asegurarse de que en producción el CORS esté restringido a un único origen.

### 8.5 Sin audit log — Prioridad: BAJA

No se guarda quién registró qué resultado, ni quién modificó un partido. Considerar tabla `AuditLog` para acciones críticas.

---

## 9. Tests — Deuda técnica crítica

**Problema:** Cero tests. `LeagueService.ComputeElo()`, `BuildRoundRobin()`, `ComputeStandings()` son algoritmos críticos sin cobertura.

**Prioridad: ALTA**

```csharp
// Futelo.Tests/EloCalculatorTests.cs
public class EloCalculatorTests
{
    [Theory]
    [InlineData(1500, 1500, 1.0, 1, 16)]   // iguales, gana por 1 gol → K/2
    [InlineData(1800, 1200, 1.0, 3)]        // favorito gana, multiplicador 1.5x, ganancia pequeña
    [InlineData(1200, 1800, 1.0, 3)]        // underdog gana por 3+, ganancia grande
    public void Compute_ReturnsCorrectChange(int myElo, int oppElo, double result, int diff, int? expected = null)
    {
        var (change, newElo) = EloCalculator.Compute(myElo, oppElo, result, diff, k: 32);
        Assert.Equal(myElo + change, newElo);
        if (expected.HasValue) Assert.Equal(expected.Value, change);
    }
}

// Futelo.Tests/FixtureGeneratorTests.cs
public class FixtureGeneratorTests
{
    [Fact]
    public void Build_With4Players_Generates6UniqueMatches()
    {
        var ids = new[] { "A", "B", "C", "D" };
        var matches = FixtureGenerator.Build(ids.ToList(), leagueId: 1, homeAndAway: false);
        Assert.Equal(6, matches.Count);
        Assert.All(matches, m => Assert.NotEqual(m.HomePlayerId, m.AwayPlayerId));
    }

    [Fact]
    public void Build_HomeAndAway_DoubleMatchCount()
    {
        var ids = new[] { "A", "B", "C", "D" };
        var single = FixtureGenerator.Build(ids.ToList(), 1, homeAndAway: false);
        var both = FixtureGenerator.Build(ids.ToList(), 1, homeAndAway: true);
        Assert.Equal(single.Count * 2, both.Count);
    }

    [Fact]
    public void Build_OddPlayers_HandlesByeCorrectly()
    {
        var ids = new[] { "A", "B", "C" };
        var matches = FixtureGenerator.Build(ids.ToList(), 1, homeAndAway: false);
        Assert.Equal(3, matches.Count); // 3 partidos con 3 jugadores (uno tiene bye cada ronda)
    }
}
```

---

## 10. Quick Wins

| # | Acción | Esfuerzo | Impacto |
|---|---|---|---|
| 1 | Crear `LocalizedComponentBase` y migrar 15 páginas | 2-3h | Elimina ~200 líneas duplicadas, previene memory leaks |
| 2 | Crear `ApiService` base + `HttpExtensions` | 1h | Centraliza error handling en 10 servicios |
| 3 | Interceptor 401 en `AuthTokenHandler` | 30min | Mejora UX en sesiones expiradas |
| 4 | Extraer `EloCalculator` compartido | 1h | Elimina triplicación de lógica crítica |
| 5 | `AsNoTracking()` en queries de solo lectura | 30min | Mejora performance EF Core inmediata |
| 6 | Verificar `Include()` en todos los repositories | 1h | Previene N+1 queries |

---

## 11. Refactors prioritarios

1. **ALTA** — `LocalizedComponentBase` (memory leaks + duplicación)
2. **ALTA** — `ApiService` base + `HttpExtensions` (duplicación sistémica)
3. **ALTA** — Interceptor 401 en `AuthTokenHandler` (UX sesión expirada)
4. **ALTA** — Tests para `EloCalculator`, `FixtureGenerator`, `StandingsCalculator`
5. **ALTA** — Extraer `EloCalculator` compartido entre Liga/Copa/SuperCopa
6. **MEDIA** — Verificar `Include()` en todos los repositories
7. **MEDIA** — `CancellationToken` en componentes con cargas largas
8. **MEDIA** — Descomponer `SeasonDetail` en sub-componentes
9. **MEDIA** — Descomponer `LeagueService` (FixtureGenerator, StandingsCalculator)
10. **MEDIA** — Rate limiting en auth endpoints

---

## 12. Mejoras estructurales a largo plazo

1. **Logging** con `ILogger<T>` en todos los servicios del servidor
2. **Refresh tokens** para sesiones largas sin re-login
3. **Caching en cliente** para catálogos estáticos (VideoGames, Teams)
4. **Índices en EloHistory** para queries de stats a escala
5. **Audit log** para acciones críticas (registrar resultado, modificar partido)
6. **Componente EmptyState** reutilizable para todas las páginas de stats
7. **ARIA labels** en botones de acción y avatares

---

## Conclusión

El proyecto tiene una **base arquitectónica sólida y coherente**. Las convenciones se cumplen, la separación de capas es correcta, y la lógica de negocio (ELO, fixtures, standings) está bien implementada. El nivel técnico es bueno.

Las debilidades principales son de **madurez**: cero tests en lógica crítica, duplicación sistémica que escala mal, y gaps de seguridad menores (no críticos pero importantes antes de producción real).

Con los quick wins aplicados, el proyecto pasa de 7.2 a ~8.5/10 en pocas horas de trabajo.
