# TODO

## ✅ Sesiones 1–12 — Completas

Todos los sprints de diseño, i18n, auditoría técnica (A1–A5), features de layout y sidebar completados en la rama `12-design-system`.

---

## ⏳ Deuda técnica pendiente

### Sprint A6 — Seguridad (~3h) 🟡 Media

- [ ] Agregar rate limiting en endpoints de auth (`/login`, `/register`)
  - `feat: add rate limiting to auth endpoints`
- [ ] Implementar refresh tokens
  - `feat: implement refresh token support for long-lived sessions`
- [ ] Restringir CORS a origen único para producción
  - `fix: restrict CORS to single origin for production`

---

### Sprint A7 — UX y calidad (~3h) 🟢 Baja ✅

- [x] Crear componente `EmptyState.razor` reutilizable y reemplazar todos los `<p class="empty-state">` (17 páginas)
  - `feat: add reusable EmptyState component and migrate all empty states`
- [x] Agregar ARIA labels en botones de paginación (4 botones en VaultMatchHistory y PlayerMatchHistory)
  - `fix: add ARIA labels to pagination buttons for accessibility`
- [x] Crear `ErrorMessages.cs` en `Futelo.Server/Services/` y reemplazar todos los strings hardcodeados (10 servicios)
  - `refactor: centralize error message strings in ErrorMessages`
- [x] Agregar `ILogger<T>` en todos los servicios del servidor
  - `feat: add structured logging to all server services`
- [x] Implementar caching en `VideoGameService` y `TeamService` para catálogos estáticos
  - `feat: add in-memory cache to VideoGameService and TeamService`

---

### Sprint A8 — Toast global (~2h) 🟢 Baja ✅

- [x] Crear `IToastService` en `Futelo.Client/Services/Toast/`
- [x] Implementar `ToastService` como singleton
- [x] Crear `ToastContainer.razor` + `.razor.css` en `Shared/`
- [x] Agregar `<ToastContainer />` en `MainLayout.razor`
- [x] Registrar `ToastService` en `Program.cs`
- [x] Migrar páginas con `ToastService.Show()`
  - `feat: add global toast service and migrate alert messages in key pages`

---

### Sprint A9 — Constantes ELO y CORS (~1h) 🟡 Media ✅

- [x] Agregar constantes en `EloCalculator`: `InitialElo`, `LeagueK`, `CupBaseK`, `SuperCupK`, `GoalDiff3x`, `GoalDiff2x`
- [x] Reemplazar `1500` en `AppUser.cs`, `SeasonPlayer.cs`, `SeasonService.cs` → `EloCalculator.InitialElo`
- [x] Reemplazar K-factors en `LeagueService.cs`, `CupService.cs`, `SuperCupService.cs` → `EloCalculator.*K`
  - `refactor: extract ELO constants into EloCalculator and replace all magic numbers`
- [x] Mover CORS origins de `Program.cs` a `appsettings.Development.json`
  - `fix: move CORS allowed origins to appsettings.json`

---

### Sprint A10 — Status constants en Client (~1h) 🟡 Media ✅

- [x] Crear `Futelo.Shared/CompetitionStatus.cs` y `Futelo.Shared/MatchStatus.cs` con string constants
- [x] Reemplazar magic strings en `BadgeHelper.cs`, `LeagueView.razor.cs`, `CupView.razor.cs`, `SuperCupView.razor.cs`, `SeasonDetail.razor.cs`
- [x] Localizar badges de estado en `LeagueView`, `CupView`, `SuperCupView` — claves `competition.status.*` en `es.json` / `en.json`
- [x] Mover `hasRightContent` de `@{}` en `.razor` a propiedad en `.razor.cs` + fix `</div>` faltante en `SeasonDetail`
  - `refactor: replace status magic strings in client and localize competition badges`

---

### Sprint A11 — CSS color variables (~45min) 🟢 Baja ✅

- [x] Agregar en `variables.css`: `--color-danger-text`, `--color-success-text`, `--color-warning-text`, `--color-info-text`, `--color-bronze`, `--color-cup`, `--color-supercup`
  - `refactor: add missing CSS variables for alert and component colors`
- [x] Reemplazar colores hardcodeados en `components.css`, `app.css`, `GeneralRanking.razor.css`, `PlayerProfile.razor.css`, `Palmares.razor.css`
  - `refactor: replace hardcoded colors with CSS variables`

---

### Sprint A12 — Controller exception middleware (~1h) 🟢 Baja ✅

- [x] Crear `Futelo.Server/Filters/ApiExceptionFilter.cs`:
  - `KeyNotFoundException` → 404
  - `UnauthorizedAccessException` → 403
  - `InvalidOperationException` → 400 + mensaje
- [x] Registrar globalmente en `Program.cs`
- [x] Eliminar try-catch de los 10 controllers (solo `AuthController.Login` conserva el suyo: retorna 401, no 403)
  - `refactor: add exception filter to eliminate try-catch boilerplate in controllers`

---

## Sesión 13 — Light/Dark Mode
Rama: `13-theme`

### Sprint T1 — ThemeService ✅
`feat: add theme service with localStorage persistence`

- [x] Crear `IThemeService` en `Futelo.Client/Services/Theme/`
  - `string CurrentTheme` (→ `"dark"` | `"light"`)
  - `Task SetThemeAsync(string theme)`
  - `event Action OnChange`
- [x] Implementar `ThemeService`:
  - Lee tema inicial de localStorage via JS interop
  - Aplica clase `light` en `<body>` via JS interop (`document.body.classList`)
  - Dispara `OnChange`
- [x] Agregar en `wwwroot/js/theme.js`:
  - `window.themeInterop.get()` → lee de localStorage
  - `window.themeInterop.set(theme)` → guarda en localStorage + aplica clase en body
- [x] Registrar `ThemeService` como singleton en `Program.cs`
- [x] Importar `theme.js` en `index.html`

---

### Sprint T2 — CSS light mode ✅
`feat: add light mode CSS theme`

- [x] Agregar bloque `body.light { ... }` en `variables.css` con toda la paleta light:
  - `--color-bg: #F8FAFC`
  - `--color-surface: #FFFFFF`
  - `--color-surface-2: #F1F5F9`
  - `--color-border: #E2E8F0`
  - `--color-text: #0F172A`
  - `--color-text-muted: #64748B`
  - `--color-primary: #16A34A`
  - `--color-primary-hover: #15803D`
  - Todos los `--bs-*` correspondientes
- [x] Verificar glassmorphism, rank-gold, glow y skeletons en light
- [x] Ajustar colores fijos hardcodeados en `components.css` / `layout.css` que no usen variables

---

### Sprint T3 — ThemeSwitcher + NavMenu ✅
`feat: add theme toggle to nav`

- [x] Crear `ThemeSwitcher.razor` + `.razor.cs` + `.razor.css` (patrón idéntico a `LanguageSwitcher`)
- [x] Botón con ícono ☀️ / 🌙 según tema activo
- [x] Agregar `<ThemeSwitcher />` en `NavMenu.razor` dentro de `nav-controls` junto al `LanguageSwitcher`

---

### Sprint T4 — Anti-flash (FODT) ✅
`fix: prevent flash of dark theme on page load`

- [x] Agregar script inline en `index.html` (antes de cualquier CSS) que aplique la clase sincrónicamente:
  ```html
  <script>
    (function() {
      if (localStorage.getItem('theme') === 'light')
        document.body.classList.add('light');
    })();
  </script>
  ```

---

## Sesión 14 — PWA
Rama: `14-pwa`

### Sprint PWA1 — Manifest e íconos ✅
`feat: add web app manifest and PWA meta tags`

- [x] Crear `wwwroot/manifest.webmanifest` (name, short_name, start_url, display: standalone, theme_color, background_color, icons)
- [x] Generar `icon-512.png` (ya existe `icon-192.png`)
- [x] Generar `apple-touch-icon.png` (180×180, para iOS)
- [x] Variante maskable del ícono (purpose: maskable any en manifest)
- [x] Agregar en `index.html`:
  - `<link rel="manifest" href="manifest.webmanifest">`
  - `<meta name="theme-color" content="#111827">`
  - `<link rel="apple-touch-icon" href="apple-touch-icon.png">`
  - `<meta name="apple-mobile-web-app-capable" content="yes">`
  - `<meta name="apple-mobile-web-app-status-bar-style" content="black-translucent">`
  - `<meta name="apple-mobile-web-app-title" content="Futelo">`

---

### Sprint PWA2 — Service Worker ✅
`feat: add service worker for offline and installability`

- [x] Crear `wwwroot/service-worker.js` (dev: network-only, sin cache)
- [x] Crear `wwwroot/service-worker.published.js` (prod: cache-first de todos los assets Blazor)
- [x] Agregar en `Futelo.Client.csproj`:
  ```xml
  <ServiceWorkerAssetsManifest>service-worker-assets.js</ServiceWorkerAssetsManifest>
  ```
  El SDK genera `service-worker-assets.js` automáticamente al publicar con la lista de todos los assets.
- [x] Registrar `<ServiceWorker>` item en `.csproj`
- [x] Registrar service worker en `index.html`

---

### Sprint PWA3 — PwaUpdater + verificación
`feat: add PwaUpdater component for service worker update detection`

- [x] Crear `PwaUpdater.razor` + `.razor.cs` + `.razor.css` en `Shared/`
- [x] Agregar `wwwroot/js/pwaUpdater.js` con `register` y `applyUpdate`
- [x] Manejar `SKIP_WAITING` en `service-worker.published.js`
- [x] Registrar `<PwaUpdater />` en `MainLayout.razor`
- [ ] Auditoría Lighthouse PWA (score ≥ 90) — hacer en despliegue real (HTTPS)
- [ ] Probar install prompt en Chrome/Edge desktop
- [ ] Probar "Agregar a pantalla de inicio" en Safari iOS

---

### Sprint PWA4 — Offline UX ✅
`feat: add offline indicator and offline fallback page`

- [x] Banner `OfflineIndicator` que aparece/desaparece con eventos `online`/`offline`
- [x] `wwwroot/offline.html` — página de fallback sin conexión
- [x] `service-worker.published.js` — sirve `offline.html` cuando navegación falla sin caché

---

## Sesión 15 — Reglas y mejoras
Rama: `15-features`

### Sprint F1 — Fechas en temporadas y competencias ✅
`feat: replace vault dates with season, competition and match scheduling dates`

- [x] Quitar `StartDate`/`EndDate` de `Vault` (modelo, DTOs, servicio, i18n)
- [x] Agregar `StartDate`/`EndDate` a `Season`, `League`, `Cup`, `SuperCup` + modelos EF Core
- [x] Agregar `ScheduledDate` a `Match`
- [x] Migración EF Core (`AddDatesToSeasonMatchAndCompetitions`)
- [x] Actualizar DTOs en `Futelo.Shared`
- [x] Endpoints PATCH `/dates` para Season, League, Cup, SuperCup (editables en cualquier momento)
- [x] `ScheduledDate` incluido en el `PATCH match` existente

---

### Sprint F2 — (integrado en F1) ✅
`feat: replace vault dates with season, competition and match scheduling dates`

- [x] `ScheduledDate` en `Match` + migración + endpoint PATCH integrado en PatchMatch

---

### Sprint F3 — Diferencia de gol en todas las tablas de stats ✅
`feat: add goal difference column to all stats tables`

- [x] Propiedad calculada `GoalDifference` en `PlayerStatsResponse`, `TeamUsageRow`, `VideoGameStatsRow`, `PlayerGameStatsRow`
- [x] Columna GD en: tabla principal de jugador, Top Teams, Performance by Game, Games Ranking

---

### Sprint F4 — Reglas de desempate en Liga ✅
`feat: add configurable tiebreaker rules for league standings`

- [x] Enum `TiebreakerRule`: `GoalDifference`, `HeadToHead`, `HeadToHeadThenGoalDifference`
- [x] `TiebreakerRule` en modelo `League` + migración (`AddTiebreakerRuleToLeague`)
- [x] `StandingsCalculator` refactorizado: algoritmo multi-grupo, H2H correcto para 3+ jugadores empatados
- [x] La regla aplica también al determinar el campeón al cerrar la liga y al widget top 3 de la temporada
- [x] UI: dropdown en configuración de liga (componente `SeasonCompetitionConfig`)

---

### Sprint F5 — Formato de Copa ✅
`feat: add away goal rule and seeding mode to cup configuration`

- [x] Crear enum `CupSeedingMode` en `Futelo.Shared`: `SeasonElo`, `LeaguePosition`, `Random`
- [x] Reemplazar `BracketMode` con `SeedingMode: CupSeedingMode` en modelo `Cup` + migración (`AddCupSeedingModeAndAwayGoalRule`)
- [x] Agregar `AwayGoalRule` (bool) al modelo `Cup` (mismo migration)
  - Se aplica solo en rondas previas a la final — la final nunca usa gol de visitante
- [x] Agregar `CupIsHomeAndAway` a `ConfigureSeasonRequest` (no estaba antes)
- [x] Actualizar `CupService`:
  - `SeasonElo`: ordena por EloRating del jugador (anterior comportamiento de Seeded)
  - `LeaguePosition`: usa `StandingsCalculator` sobre la liga activa/finalizada de la temporada
  - `Random`: shuffle aleatorio
  - Lógica de avance ida/vuelta: empate aggregate → goles visitante → penales (solo en no-finales)
- [x] Includes de `Season.League.Matches` y `Season.League.Players` en `CupRepository.GetByIdAsync`
- [x] `CupResponse` expone `SeedingMode` y `AwayGoalRule`
- [x] `SeasonResponse` expone `CupIsHomeAndAway`, `CupSeedingMode`, `CupAwayGoalRule`
- [x] UI en configuración de copa: checkbox H&A, dropdown seeding mode, checkbox gol de visitante

---

### Sprint F6 — Mejora UI configuración de torneos ✅
`feat: improve tournament configuration UI`

- [x] Rediseñar `SeasonCompetitionConfig.razor`: una card por competencia (Liga / Copa / Supercopa)
- [x] Toggle switches para activar/desactivar cada competencia; borde verde cuando activa
- [x] Textos de ayuda (`form-text`) bajo cada regla (H&A, tiebreaker, seeding mode, away goal rule)
- [x] CSS scoped en `SeasonCompetitionConfig.razor.css` usando variables del design system
- [x] Claves i18n de hint añadidas en `en.json` y `es.json`

---

### Sprint F7 — Ingreso manual de partidos: Liga ⬜
Permite cargar partidos históricos en la liga sin generar el fixture automático. El usuario activa la liga manualmente, agrega partidos (fecha + local + visitante) de a uno, y registra resultados con el flujo existente (ELO se calcula igual).

**Commit 1 — `feat: league manual - shared DTOs and server`**
- [ ] `Futelo.Shared/DTOs/PlayerSummary.cs` ← nuevo `{ string Id, string DisplayName }`
- [ ] `Futelo.Shared/DTOs/League/AddLeagueMatchRequest.cs` ← nuevo `{ int Matchday, string HomePlayerId, string AwayPlayerId }`
- [ ] `LeagueResponse.cs` ← agregar `List<PlayerSummary> SeasonPlayers`
- [ ] `ILeagueRepository` + `LeagueRepository` ← agregar `InitPlayersAsync` y `AddMatchAsync`
- [ ] `ILeagueService` + `LeagueService` ← agregar `StartManualAsync` y `AddMatchManuallyAsync` (valida jugadores en temporada, Leg = Matchday)
- [ ] `LeagueService.GetByIdAsync` ← poblar `SeasonPlayers` desde `league.Season.Players`
- [ ] `LeagueController` ← `POST /{id}/start-manual` y `POST /{id}/matches`

**Commit 2 — `feat: league manual - client service and UI`**
- [ ] `ILeagueService` + `LeagueService` (Client) ← agregar `StartManualAsync` y `AddMatchAsync`
- [ ] `LeagueView.razor` ← en NotStarted + CanEdit: botón "Ingresar Manualmente" junto a "Generar Fixture"; en Active + CanEdit: formulario inline "Agregar partido" (nro de fecha, dropdown local, dropdown visitante)
- [ ] `LeagueView.razor.cs` ← estado `addingMatch`, `newMatchday`, `newHomePlayerId`, `newAwayPlayerId`, handlers `HandleStartManual` y `HandleAddMatch`
- [ ] `en.json` / `es.json` ← agregar `league.startManual`, `league.addMatch`

---

### Sprint F8 — Ingreso manual de partidos: Copa ⬜
Permite crear rondas y partidos manualmente en la copa. Se agrega flag `IsManual` al modelo para desactivar el auto-avance de la llave cuando se registran resultados.

**Commit 1 — `feat: cup manual - model, migration and server`**
- [ ] `Cup.cs` ← agregar `bool IsManual`
- [ ] Migración EF Core: `AddIsManualToCup` (`AddColumn<bool>("IsManual", "Cups", defaultValue: false)`)
- [ ] `Futelo.Shared/DTOs/Cup/AddCupRoundRequest.cs` ← nuevo `{ string Name, int RoundNumber }`
- [ ] `Futelo.Shared/DTOs/Cup/AddCupMatchRequest.cs` ← nuevo `{ string HomePlayerId, string AwayPlayerId, int Leg }`
- [ ] `CupResponse.cs` ← agregar `List<PlayerSummary> SeasonPlayers`, `bool IsManual`
- [ ] `ICupRepository` + `CupRepository` ← agregar `InitPlayersAsync`, `AddRoundAsync`, `AddMatchToRoundAsync`
- [ ] `ICupService` + `CupService` ← agregar `StartManualAsync`, `AddRoundAsync`, `AddMatchAsync`
- [ ] `CupService.RecordResultAsync` ← cuando `cup.IsManual == true`, omitir bloque de auto-avance; detectar fin cuando todos los partidos de la ronda con mayor `RoundNumber` estén `Played` → campeón = ganador del último partido
- [ ] `CupService.GetByIdAsync` ← poblar `SeasonPlayers`
- [ ] `CupController` ← `POST /{id}/start-manual`, `POST /{id}/rounds`, `POST /{id}/rounds/{roundId}/matches`
- [ ] `ErrorMessages` ← agregar `CupRoundNotFound`

**Commit 2 — `feat: cup manual - client service and UI`**
- [ ] `ICupService` + `CupService` (Client) ← agregar `StartManualAsync`, `AddRoundAsync`, `AddMatchAsync`
- [ ] `CupView.razor` ← en NotStarted + CanEdit: botón "Ingresar Manualmente"; en Active + IsManual: formulario "Agregar ronda" (nombre + nro) y por ronda formulario "Agregar partido" (local, visitante, leg)
- [ ] `CupView.razor.cs` ← estado + handlers
- [ ] `en.json` / `es.json` ← agregar `cup.startManual`, `cup.addRound`, `cup.roundName`, `cup.roundNumber`, `cup.addMatch`

---

### Sprint F9 — Ingreso manual de partidos: SuperCopa ⬜
Permite iniciar la SuperCopa eligiendo los dos jugadores libremente, sin requerir que la Liga y Copa estén finalizadas. Útil para cargar supercopa histórica.

**Commit 1 — `feat: supercup manual - shared DTOs and server`**
- [ ] `Futelo.Shared/DTOs/SuperCup/StartSuperCupManualRequest.cs` ← nuevo `{ string Player1Id, string Player2Id }`
- [ ] `SuperCupResponse.cs` ← agregar `List<PlayerSummary> SeasonPlayers`
- [ ] `ISuperCupService` + `SuperCupService` ← agregar `StartManualAsync(int, string player1Id, string player2Id, string userId)` (valida que ambos jugadores estén en la temporada y sean distintos; reutiliza `SetParticipantsAsync` del repo; omite el check de Liga/Copa finalizadas)
- [ ] `SuperCupService.GetByIdAsync` ← poblar `SeasonPlayers`
- [ ] `SuperCupController` ← `POST /{id}/start-manual` con body `StartSuperCupManualRequest`

**Commit 2 — `feat: supercup manual - client service and UI`**
- [ ] `ISuperCupService` + `SuperCupService` (Client) ← agregar `StartManualAsync(int, StartSuperCupManualRequest)`
- [ ] `SuperCupView.razor` ← en NotStarted + CanEdit: sección "Ingresar Manualmente" con dos dropdowns de jugadores (de `superCup.SeasonPlayers`) + botón, junto al "Iniciar SuperCopa" existente
- [ ] `SuperCupView.razor.cs` ← estado `manualPlayer1Id`, `manualPlayer2Id`, `isStartingManual`, handler `HandleStartManual`
- [ ] `en.json` / `es.json` ← agregar `supercup.startManual`, `supercup.player1`, `supercup.player2`

---

## Features futuras (backlog)

### Partido de desempate en Liga
Cuando dos o más jugadores terminan igualados en todos los criterios de desempate configurados (puntos, DG, H2H, etc.), se genera automáticamente un partido extra fuera del fixture normal para determinar el campeón o la posición en disputa.
- Requiere: nuevo tipo de partido (playoff), lógica de detección al cerrar la liga, UI para registrar el resultado, y que no afecte el ELO de la temporada.

---

## Sesión 16 — UI/UX Audit
Rama: `16-ux`

### Sprint UX1 — CSS tokens base ✅
`refactor: add missing CSS tokens and fix base font size`

- [x] `variables.css` ← añadir `--radius-md: 10px`, `--radius-lg: 16px`, `--radius-full: 9999px`
- [x] `variables.css` ← añadir escala `--space-1` a `--space-10` (múltiplos de 4px)
- [x] `variables.css` ← añadir `--z-nav: 100`, `--z-overlay: 200`, `--z-modal: 300`, `--z-toast: 9999`
- [x] `variables.css` ← añadir `--color-surface-rgb: 23, 26, 33` (dark) / `255, 255, 255` (light) para glassmorphism
- [x] `variables.css` + `app.css` ← `--font-size-base: 16px` (era 15px; previene auto-zoom en iOS)
- [x] `layout.css` ← `.nav-section-label`: `opacity: 0.5` → `0.65` (contraste WCAG AA)

---

### Sprint UX2 — Fixes de CSS por componente ✅
`fix: CSS component fixes from UX audit`

- [x] `components.css` ← `.card-glass`: reemplazar `rgba(23, 26, 33, 0.6)` hardcodeado por `rgba(var(--color-surface-rgb), 0.6)` (ya tenía override light mode correcto, solo se parametriza el dark)
- [ ] `PlayerCompare.razor.css` ← `.compare-winner` / `.h2h-bar__p2` — componente no existe todavía, se aplicará cuando se implemente
- [x] `LeagueView.razor.css` ← `.match-card--played`: eliminar `opacity: 0.72` global; aplicar `opacity: 0.55` solo en `.match-meta` y `.match-card__date`
- [x] `LeagueView.razor.css` ← `.match-card--pending`: ya tenía `border-left: 3px solid var(--color-primary)` — sin cambios
- [x] `LeagueView.razor.css` ← añadir clase `.result-form-section` con fondo verde tenue
- [x] `components.css` ← `.empty-state`: convertir a flexbox centrado, `border-radius: var(--radius-lg)`, spacing con tokens

---

### Sprint UX3 — Jerarquía visual ✅
`feat: improve visual hierarchy in key screens`

- [x] `PlayerProfile.razor` + `.razor.css` ← ELO como número hero con `.elo-hero` (2.25rem, font-display, color-primary) en lugar de badge
- [x] `components.css` ← `.match-card__score`, `.match-card__player`, `.match-card__vs` movidos desde `LeagueView.razor.css` (donde eran CSS muerto por scoped CSS) a global; score a 1.35rem font-display
- [x] `Dashboard.razor` ← nombre del vault con `fw-bold`, player count como badge, CTA a ancho completo con flecha; badge "Temporada activa" pendiente (requiere `HasActiveSeason` en `VaultResponse`)

---

### Sprint UX4 — Feedback de acciones ✅
`feat: add loading spinners and improve action feedback`

- [x] Todos los botones de guardado ← añadir `spinner-border-sm` + `disabled` durante el loading (SaveResult, ActivateSeason, CreateVault, CreateSeason, etc.)
- [x] `LeagueView` ← eliminar `alert-success` permanente de resultado; redirigir al `ToastService` existente
- [x] `CupView` ← igual que LeagueView para el alert de resultado

---

### Sprint UX5 — Formularios ⬜
`feat: improve form UX with labels, validation and modals`

- [ ] `MatchResultForm.razor` ← reemplazar placeholders sueltos por labels visibles (`form-label fw-semibold`) sobre cada input de score; igual en la sección de penales
- [ ] Todos los `EditForm` ← agregar `is-invalid` + `<div class="invalid-feedback">` en campos con error; alert global solo para errores de servidor
- [ ] `SeasonDetail.razor` ← reemplazar confirmación inline de "Eliminar temporada" por Bootstrap Modal con descripción de consecuencias
- [ ] `VaultDetail.razor` (o donde aplique) ← mismo modal de confirmación para "Eliminar vault"
- [ ] `LeagueView.razor` ← form "Agregar partido": reemplazar `style="width:160px"` hardcodeados por grid Bootstrap responsive (`col-6 col-sm`)

---

### Sprint UX6 — Navegación ⬜
`feat: add breadcrumbs and improve season setup flow`

- [ ] `LeagueView.razor` ← añadir breadcrumb Bootstrap 5: Dashboard → Vault → Season → League (reemplaza o complementa el `← Back`)
- [ ] `CupView.razor` ← mismo breadcrumb
- [ ] `SeasonDetail.razor` ← mismo breadcrumb; + mover botón "Activar temporada" al top de la página (en Draft mode, mostrar banner verde con el CTA en lugar de tenerlo al fondo)
- [ ] `SeasonDetail.razor` ← añadir stepper visual de configuración (Competiciones → Jugadores → Equipos → Activar) cuando `Status == Draft`

---

### Sprint UX7 — EmptyState mejorado ⬜
`feat: enhance EmptyState component with icon, title and CTA`

- [ ] `EmptyState.razor` + `.razor.cs` ← añadir parámetros: `Title`, `Icon` (RenderFragment), `ActionLabel`, `ActionHref`, `OnAction` (EventCallback)
- [ ] `EmptyState.razor` ← nueva estructura: ícono centrado + título + mensaje + botón CTA opcional
- [ ] Migrar usos existentes de `EmptyState` para pasar `Title` y CTA donde corresponda (Dashboard sin vaults, lista de partidos vacía, etc.)

---

### Sprint UX8 — HeadToHead visual (barras y winner highlight) ⬜
`feat: improve HeadToHead page with visual stat bars and winner highlight`

La página `HeadToHead.razor` actual muestra las stats como números simples y una tabla. Este sprint la convierte en un panel visual con:

- [ ] CSS scoped `HeadToHead.razor.css` ← añadir `.h2h-stats`, `.h2h-bar`, `.h2h-bar__p1`, `.h2h-bar__p2`, `.compare-winner`, `.compare-loser`
- [ ] `HeadToHead.razor` ← reemplazar los 3 `div` de wins/draws/losses por un layout centrado con barras proporcionales (`h2h-bar__p1` verde / `h2h-bar__p2` rojo / empates neutro)
- [ ] `HeadToHead.razor` ← highlight del jugador con más victorias con `.compare-winner` (color `var(--color-success)`, `fw-bold`) y el otro con `.compare-loser` (`opacity: 0.45`)
- [ ] `HeadToHead.razor` ← añadir fila de resumen de goles totales con el mismo formato de barras

---

## Sesión 17 — Pulido y despliegue
Rama: `16-deploy`

### Sprint D1 — Error boundaries en Client (~30min) ⬜
`feat: add error boundaries and custom 404 page`

- [ ] Envolver `<Router>` en `MainLayout.razor` con `<ErrorBoundary>` de Blazor
- [ ] Crear `Shared/ErrorFallback.razor` + `.razor.cs`: mensaje amigable + botón "Volver al inicio"
- [ ] Crear `Pages/NotFound.razor`: página 404 con link al dashboard
- [ ] Configurar template `<NotFound>` en `Router` para usar `NotFound.razor`

---

### Sprint D2 — Security hardening (~1h) ⬜
`feat: add rate limiting and restrict CORS for production`

- [ ] Agregar rate limiting en `Program.cs` con `AddRateLimiter` (.NET 8+):
  - Política `"auth"`: 5 requests / 1 minuto por IP
  - Aplicar con `[EnableRateLimiting("auth")]` en `AuthController.Login` y `AuthController.Register`
- [ ] Restringir CORS en `appsettings.Production.json` a la URL del cliente en producción

---

### Sprint D3 — Configuración de producción (~45min) ⬜
`feat: add production configuration and environment variable support`

- [ ] Crear `Futelo.Server/appsettings.Production.json`:
  - `ConnectionStrings:DefaultConnection` (ruta SQLite en el servidor)
  - `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` leídos de env vars (`${JWT_KEY}`, etc.)
  - `Cors:AllowedOrigins` con la URL del cliente
- [ ] Verificar que ningún secreto esté hardcodeado en el repo
- [ ] Crear `Futelo.Client/wwwroot/appsettings.Production.json` con la URL del API en producción

---

### Sprint D4 — Dockerfile y despliegue (~2h) ⬜
`feat: add Dockerfile and deploy to Railway`

- [ ] Crear `Dockerfile` multi-stage en la raíz:
  - Stage `build`: `mcr.microsoft.com/dotnet/sdk:10.0` — publica `Futelo.Server` (incluye el Client como assets estáticos)
  - Stage `runtime`: `mcr.microsoft.com/dotnet/aspnet:10.0` — copia publish output
- [ ] Crear `.dockerignore`
- [ ] Deploy en Railway (recomendado: conectar repo GitHub, Railway detecta Dockerfile automáticamente)
- [ ] Configurar variables de entorno en Railway: `JWT_KEY`, `JWT_ISSUER`, `JWT_AUDIENCE`, `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Verificar que la app levanta en producción (DB se crea/migra al arrancar con `EnsureCreated` o `Migrate`)

---

### Sprint D5 — PWA Lighthouse post-deploy (~30min) ⬜
`fix: PWA audit and install prompt verification`

- [ ] Auditoría Lighthouse en Chrome DevTools (score ≥ 90 en PWA)
- [ ] Probar install prompt en Chrome/Edge desktop
- [ ] Probar "Agregar a pantalla de inicio" en Safari iOS
