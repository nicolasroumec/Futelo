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

### Sprint PWA1 — Manifest e íconos
`feat: add web app manifest and PWA meta tags`

- [ ] Crear `wwwroot/manifest.webmanifest` (name, short_name, start_url, display: standalone, theme_color, background_color, icons)
- [ ] Generar `icon-512.png` (ya existe `icon-192.png`)
- [ ] Generar `apple-touch-icon.png` (180×180, para iOS)
- [ ] Variante maskable del ícono (con padding de safe zone)
- [ ] Agregar en `index.html`:
  - `<link rel="manifest" href="manifest.webmanifest">`
  - `<meta name="theme-color" content="#111827">`
  - `<link rel="apple-touch-icon" href="apple-touch-icon.png">`
  - `<meta name="apple-mobile-web-app-capable" content="yes">`
  - `<meta name="apple-mobile-web-app-status-bar-style" content="black-translucent">`
  - `<meta name="apple-mobile-web-app-title" content="Futelo">`

---

### Sprint PWA2 — Service Worker
`feat: add service worker for offline and installability`

- [ ] Crear `wwwroot/service-worker.js` (dev: network-only, sin cache)
- [ ] Crear `wwwroot/service-worker.published.js` (prod: cache-first de todos los assets Blazor)
- [ ] Agregar en `Futelo.Client.csproj`:
  ```xml
  <ServiceWorkerAssetsManifest>service-worker-assets.js</ServiceWorkerAssetsManifest>
  ```
  El SDK genera `service-worker-assets.js` automáticamente al publicar con la lista de todos los assets.

---

### Sprint PWA3 — Verificación
`chore: verify PWA installability`

- [ ] Auditoría Lighthouse → sección PWA (score ≥ 90)
- [ ] Probar install prompt en Chrome/Edge desktop
- [ ] Probar "Agregar a pantalla de inicio" en Safari iOS

---

### Sprint PWA4 — Offline UX (opcional)
`feat: add offline indicator`

- [ ] Banner o toast cuando la app detecta que no hay conexión
- [ ] Página de fallback offline (para navegación sin caché)

---

## Sesión 15 — Pulido y despliegue
Ver roadmap para detalle completo.
