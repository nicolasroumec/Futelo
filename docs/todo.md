# TODO

> Estado actualizado tras el merge de `16-ux-ui` (PR #16).
> Detalle de features cerradas: ver `docs/features.md`.

## ✅ Cerrado — Sesiones 1–16

Todo el producto funcional está completo y en `main`:

- **Sesiones 1–11** — Dominio, auth JWT, vaults, catálogos, temporadas, Liga, Copa, SuperCopa, perfiles y estadísticas (básicas + avanzadas).
- **Sesión 12** — Sistema de diseño, i18n (ES/EN), responsive, auditoría técnica (A1–A12).
- **Sesión 13** — Light/Dark mode con persistencia y anti-flash.
- **Sesión 14** — PWA (manifest, service worker, offline UX, update prompt).
- **Sesión 15** — Fechas en temporada/competencias, diferencia de gol, reglas de desempate (drag & drop), formato de Copa (seeding + gol de visitante), ingreso manual de partidos (Liga/Copa/SuperCopa).
- **Sesión 16** — Auditoría UX/UI completa (UX1–UX8), avatares de jugador + escudos de equipo, corrección del último resultado con rollback de ELO, brackets visuales de Copa, breadcrumbs, stepper de setup, EmptyState con CTA, validación de formularios.

---

## ⏳ Sesión 17 — Pulido y despliegue
Rama: `17-deploy`

### Sprint D1 — Error boundaries en Client (~30min) ✅
`feat: add error boundaries and custom 404 page`

- [x] `<ErrorBoundary>` en `MainLayout` envolviendo `@Body` (dentro del div con `@key="Nav.Uri"` → se auto-resetea al navegar)
- [x] `Shared/ErrorFallback.razor` + `.razor.cs` + `.razor.css`: ícono, mensaje amigable, botones "Volver al inicio" (forceLoad a `/`) y "Recargar"
- [x] `Pages/NotFound.razor` rediseñada: 404 centrada con código grande + CTA al dashboard
- [x] Router ya usa `NotFoundPage="typeof(Pages.NotFound)"` (Blazor 10) en `App.razor` — sin cambios necesarios
- [x] Claves i18n `error.*` y `notFound.backHome` en `es.json` / `en.json`

---

### Sprint D2 — Security hardening (~1h) ⬜
`feat: add rate limiting to auth endpoints`

- [ ] Rate limiting en `Program.cs` con `AddRateLimiter`:
  - Política `"auth"`: 5 requests / 1 minuto por IP
  - Aplicar `[EnableRateLimiting("auth")]` en `AuthController.Login` y `AuthController.Register`

> **Ya hecho (no rehacer):**
> - ✅ Refresh tokens — access 15min + refresh 30 días, endpoints `POST /api/auth/refresh` y `/logout`, manejado en `AuthService` + `AuthTokenHandler` (Client).
> - ✅ CORS config-driven — `Program.cs` lee `AllowedOrigins` de configuración con `WithOrigins` (sin `AllowAnyOrigin`). Solo falta cargar la URL real de producción → ver D3.

---

### Sprint D3 — Configuración de producción (~45min) ⬜
`feat: add production configuration and environment variable support`

- [ ] Crear `Futelo.Server/appsettings.Production.json`:
  - `ConnectionStrings:DefaultConnection` (SQL Server en el servidor)
  - `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` leídos de env vars
  - `Cors:AllowedOrigins` con la URL del cliente
- [ ] Verificar que ningún secreto esté hardcodeado en el repo
- [ ] Crear `Futelo.Client/wwwroot/appsettings.Production.json` con la URL del API en producción

---

### Sprint D4 — Dockerfile y despliegue (~2h) ⬜
`feat: add Dockerfile and deploy`

- [ ] `Dockerfile` multi-stage en la raíz:
  - Stage `build`: `mcr.microsoft.com/dotnet/sdk:10.0` — publica `Futelo.Server` (incluye el Client como assets estáticos)
  - Stage `runtime`: `mcr.microsoft.com/dotnet/aspnet:10.0` — copia el publish output
- [ ] `.dockerignore`
- [ ] Deploy en el proveedor elegido (conectar repo GitHub; detecta Dockerfile)
- [ ] Configurar env vars: `JWT_KEY`, `JWT_ISSUER`, `JWT_AUDIENCE`, `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Verificar que la app levanta y migra la DB al arrancar (`Migrate`)

---

### Sprint D5 — PWA Lighthouse post-deploy (~30min) ⬜
`fix: PWA audit and install prompt verification`

- [ ] Auditoría Lighthouse en Chrome DevTools (score ≥ 90 en PWA) — requiere HTTPS real
- [ ] Probar install prompt en Chrome/Edge desktop
- [ ] Probar "Agregar a pantalla de inicio" en Safari iOS

---

## Features futuras (backlog)

Ver la sección **Backlog** de `docs/features.md` para el detalle. Pendientes principales:

- **Match status lifecycle** — enum `NotStarted | InProgress | Finished` en `Match`.
- **Push notifications (PWA)** — avisar partidos programados próximos.
- **Transfer vault ownership** — traspaso de admin a otro miembro.
- **Cross-season stats comparison** — comparar stats de un jugador entre temporadas.
- **Partido de desempate en Liga** — playoff automático cuando hay empate total en los criterios de desempate (no afecta ELO).
</content>
</invoke>
