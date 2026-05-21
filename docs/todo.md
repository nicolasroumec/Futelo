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

### Sprint F6 — Mejora UI configuración de torneos (~1h) ⬜
`feat: improve tournament configuration UI`

- [ ] Rediseñar el formulario de configuración de Liga/Copa para que las nuevas reglas sean claras y estén bien agrupadas
- [ ] Usar cards o secciones colapsables por competencia
- [ ] Tooltips o descripciones cortas para cada opción de regla

---

## Features futuras (backlog)

### Partido de desempate en Liga
Cuando dos o más jugadores terminan igualados en todos los criterios de desempate configurados (puntos, DG, H2H, etc.), se genera automáticamente un partido extra fuera del fixture normal para determinar el campeón o la posición en disputa.
- Requiere: nuevo tipo de partido (playoff), lógica de detección al cerrar la liga, UI para registrar el resultado, y que no afecte el ELO de la temporada.

---

## Sesión 16 — Pulido y despliegue
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
