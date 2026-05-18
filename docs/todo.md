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

### Sprint A7 — UX y calidad (~3h) 🟢 Baja

- [ ] Crear componente `EmptyState.razor` reutilizable y reemplazar todos los `<p class="empty-state">`
  - `feat: add reusable EmptyState component and migrate all empty states`
- [ ] Agregar ARIA labels en botones de acción
  - `fix: add ARIA labels to action buttons for accessibility`
- [ ] Agregar `ILogger<T>` en todos los servicios del servidor
  - `feat: add structured logging to all server services`
- [ ] Crear `ErrorMessages.cs` en Shared y reemplazar strings hardcodeados
  - `refactor: centralize error message strings in ErrorMessages`
- [ ] Implementar caching en `VideoGameService` y `TeamService` para catálogos estáticos
  - `feat: add in-memory cache to VideoGameService and TeamService`

---

## Sesión 13 — Light/Dark Mode
Rama: `13-theme`

### Sprint T1 — ThemeService
`feat: add theme service with localStorage persistence`

- [ ] Crear `IThemeService` en `Futelo.Client/Services/Theme/`
  - `string CurrentTheme` (→ `"dark"` | `"light"`)
  - `Task SetThemeAsync(string theme)`
  - `event Action OnChange`
- [ ] Implementar `ThemeService`:
  - Lee tema inicial de localStorage via JS interop
  - Aplica clase `light` en `<body>` via JS interop (`document.body.classList`)
  - Dispara `OnChange`
- [ ] Agregar en `wwwroot/js/theme.js`:
  - `window.themeInterop.get()` → lee de localStorage
  - `window.themeInterop.set(theme)` → guarda en localStorage + aplica clase en body
- [ ] Registrar `ThemeService` como singleton en `Program.cs`
- [ ] Importar `theme.js` en `index.html`

---

### Sprint T2 — CSS light mode
`feat: add light mode CSS theme`

- [ ] Agregar bloque `body.light { ... }` en `variables.css` con toda la paleta light:
  - `--color-bg: #F8FAFC`
  - `--color-surface: #FFFFFF`
  - `--color-surface-2: #F1F5F9`
  - `--color-border: #E2E8F0`
  - `--color-text: #0F172A`
  - `--color-text-muted: #64748B`
  - `--color-primary: #16A34A`
  - `--color-primary-hover: #15803D`
  - Todos los `--bs-*` correspondientes
- [ ] Verificar glassmorphism, rank-gold, glow y skeletons en light
- [ ] Ajustar colores fijos hardcodeados en `components.css` / `layout.css` que no usen variables

---

### Sprint T3 — ThemeSwitcher + NavMenu
`feat: add theme toggle to nav`

- [ ] Crear `ThemeSwitcher.razor` + `.razor.cs` + `.razor.css` (patrón idéntico a `LanguageSwitcher`)
- [ ] Botón con ícono ☀️ / 🌙 según tema activo
- [ ] Agregar `<ThemeSwitcher />` en `NavMenu.razor` dentro de `nav-controls` junto al `LanguageSwitcher`

---

### Sprint T4 — Anti-flash (FODT)
`fix: prevent flash of dark theme on page load`

- [ ] Agregar script inline en `index.html` (antes de cualquier CSS) que aplique la clase sincrónicamente:
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
