# TODO

Sesiones 1–11 completas. Pendiente:

## Sesión 12 — Diseño y UI

### ✅ Hecho
- Design system: variables CSS (paleta, tipografía, layout)
- Fuentes Google: Barlow Condensed + Inter
- Layout responsive mobile-first (sidebar fijo en desktop, navbar colapsable en mobile)
- Componentes base: cards, buttons, forms, badges, alerts, tables, list groups

---

### ✅ Sprint 1 — Fundamentos visuales
`feat: style auth pages and empty states`

- [x] Auth pages — Login y Register centrados en card con presencia visual
- [x] Empty states — estilizar los "no data yet" de todas las páginas

---

### ✅ Sprint 2 — Loading experience
`feat: add skeleton loading`

- [x] Skeleton loading — componentes SkeletonTable, SkeletonList, SkeletonCards aplicados en las 17 páginas con `Loading...`

---

### ✅ Sprint 3 — Micro-interactions
`feat: add nav indicator, glow and page transitions`

- [x] NavLink con indicador animado — línea verde que se desliza entre links en la sidebar
- [x] Glow verde en #1 — sutil resplandor en la fila del jugador top del ranking
- [x] Transición de página — fade in suave al navegar

---

### ✅ Sprint 4 — Componentes de stats
`feat: add player avatars, win rate bar and form visualization`

- [x] Avatar con iniciales — círculo con color único generado del nombre (rankings, listas, profile)
- [x] Forma reciente visual — bloques W/D/L con animación de entrada en cascada (PlayerProfile)
- [x] Barra de win rate animada — progress bar que se llena al cargar (PlayerProfile)

---

### ✅ Sprint 5 — Páginas destacadas
`feat: podium, ELO counter animation and chart theme`

- [x] Podio top 3 — rediseño de General Ranking con podio para 1°/2°/3°
- [x] Contador animado — ELO y stats cuentan desde 0 al cargar
- [x] Chart.js custom — colores y tooltip del gráfico ELO que matcheen la paleta

---

### ✅ Sprint 6 — Efectos visuales
`feat: glassmorphism cards`

- [x] Glassmorphism — `backdrop-filter: blur` con borde semitransparente en cards destacadas

---

### Sprint 7 — i18n

#### ✅ Commit 1 — feat: add i18n infrastructure (LanguageService + resource files)
- [x] Definir interfaz `ILanguageService` con método `Get(string key)`
- [x] Crear archivos de recursos `en.json` y `es.json` en `wwwroot/i18n/`
- [x] Implementar `LanguageService` que carga el JSON y expone el diccionario
- [x] Registrar el servicio en `Program.cs`
- [x] Persistir idioma seleccionado en localStorage

#### ✅ Commit 2 — feat: i18n shared components and navbar
- [x] Componente `LanguageSwitcher` en la sidebar (toggle ES / EN)
- [x] Strings del NavMenu (Dashboard, Teams, Games, Logout, Login, Register)

#### ✅ Commit 3 — feat: i18n auth pages
- [x] Login — título, subtítulo, labels, botón, link a register
- [x] Register — título, subtítulo, labels, botón, link a login

#### ✅ Commit 4 — feat: i18n dashboard and vault pages
- [x] Dashboard (My Vaults, New Vault, empty state, Open)
- [x] VaultDetail (Players, Seasons, Invite Player, Generate Link, etc.)
- [x] SeasonDetail (Year, Video Game, Ranking, Finish Season, Players, etc.)
- [x] CreateVault / CreateSeason

#### ✅ Commit 5 — feat: i18n stats pages
- [x] GeneralRanking, Ranking, Scorers, Palmares
- [x] GamesRanking, TeamPanel, VaultRecords

#### ✅ Commit 6 — feat: i18n player and competition pages
- [x] PlayerProfile (Stats, Recent form, Current streak, ELO History, etc.)
- [x] HeadToHead (Match History, headers de tabla)
- [x] LeagueView, CupView, SuperCupView (Standings, Fixture, Enter Result, etc.)

#### ✅ Commit 7 — feat: i18n teams, games and shared components
- [x] Teams, Games (CRUD forms y labels)
- [x] Empty states, error messages, skeleton labels

---

### 🔮 Futuro
`feat: add light mode toggle`

- [ ] Segunda definición de variables en `variables.css` activada por clase en `<body>`
- [ ] Toggle de tema guardado en localStorage

---

## Sesión 13 — Pulido y despliegue
Ver roadmap para detalle completo.

---

## Auditoría técnica — Deuda técnica (2026-05-14)

### Sprint A1 — Arquitectura base (~5h) ✅ COMPLETO

- [x] Crear `LocalizedComponentBase` en `Futelo.Client/Shared/`
- [x] Migrar las 23 páginas a heredar de `LocalizedComponentBase`
- [x] Crear `HttpExtensions.EnsureSuccessAsync` en `Futelo.Client/Services/`
- [x] Crear `ApiService` base y hacer que los 9 servicios cliente hereden de él
- [x] Implementar `AuthTokenHandler` con redirect automático al expirar el JWT

**Commit:** `refactor: sprint A1 — LocalizedComponentBase, ApiService, HttpExtensions, 401 redirect`

---

### Sprint A2 — Lógica de negocio compartida (~2h) ✅ COMPLETO

- [x] Extraer `EloCalculator` como clase estática en `Futelo.Server/Helpers/`
  - `refactor: extract shared EloCalculator from League/Cup/SuperCup services`
- [x] Reemplazar los 3 usos duplicados de ELO por llamadas a `EloCalculator`
  - `refactor: replace inline ELO logic with EloCalculator in all competition services`

---

### Sprint A3 — Tests (~4h) ✅ COMPLETO

- [x] Crear proyecto `Futelo.Tests` con xUnit
  - `chore: add Futelo.Tests project with xUnit`
- [x] Tests para `EloCalculator` (casos normales, underdog, favorito, multiplicadores, K)
  - `test: add EloCalculator unit tests`
- [x] Tests para `FixtureGenerator` (par/impar, ida y vuelta, bye, pares únicos)
  - `test: add FixtureGenerator unit tests`
- [x] Tests para `StandingsCalculator` (puntos, GD, GF, H2H, alfabético)
  - `test: add StandingsCalculator unit tests`

---

### Sprint A4 — Performance (~2h) ✅ COMPLETO

- [x] Auditar todos los repositories — `AsNoTracking()` e `Include()` ya correctos en toda la base de datos; `BaseRepository` usa `AsNoTrackingWithIdentityResolution` globalmente
- [x] Agregar `CancellationToken` en los servicios cliente y componentes con carga larga
  - `refactor: add CancellationToken support to client services and base component`
  - `refactor: pass ComponentToken to all service calls in components`

---

### Sprint A5 — Refactors de tamaño (~4h) ✅ COMPLETO

- [x] Extraer `SeasonPlayerManager`, `SeasonCompetitionConfig`, `SeasonTeamSelector` de `SeasonDetail`
  - `refactor: decompose SeasonDetail into focused sub-components`
- [x] Extraer `FixtureGenerator` de `LeagueService` → `Futelo.Server/Helpers/`
  - `refactor: extract FixtureGenerator from LeagueService`
- [x] Extraer `StandingsCalculator` de `LeagueService` → `Futelo.Server/Helpers/`
  - `refactor: extract StandingsCalculator from LeagueService`
- [x] Descomponer `RecordResultAsync` en métodos privados
  - `refactor: break down RecordResultAsync into focused private methods`

---

### Sprint A6 — Seguridad (~3h) 🟡 MEDIA

- [ ] Agregar rate limiting en endpoints de auth (`/login`, `/register`)
  - `feat: add rate limiting to auth endpoints`
- [ ] Implementar refresh tokens
  - `feat: implement refresh token support for long-lived sessions`
- [ ] Restringir CORS a origen único para producción
  - `fix: restrict CORS to single origin for production`

---

### Sprint A7 — UX y calidad (~3h) 🟢 BAJA

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

## Sesión 12 (rama `12-design-system`) — Desktop layout + UX

### ✅ Completado en esta sesión

- [x] Componente `MatchDisplay` unificado — reemplaza el bloque match en League, Cup, SuperCup, y todas las páginas de historial
- [x] Click en partido abre resultado/edición (sprint 2 de diseño)
- [x] `MatchEditPanel` reorganizado para reflejar jerarquía de la card
- [x] Récords ELO: `LongestTop1Reign` + `TotalMatchesAtTop1` en vault y perfil de jugador
- [x] Desktop layout baseline — `max-width` global en pantallas muy anchas, override de constraints angostos en columnas Bootstrap
- [x] LeagueView — layout de dos columnas en desktop (standings | fixture)
- [x] PlayerProfile — layout de dos columnas en desktop (stats/forma/títulos | récords/gráfico ELO)
- [x] VaultDetail — layout de dos columnas en desktop (jugadores/temporadas | partidos recientes)
- [x] CupView — ties en grid de dos columnas en desktop
- [x] VaultRecords — cards en grid de dos columnas

---

### 🔴 Fix urgente — pendiente

#### ✅ Fix 1: Tablas demasiado anchas en desktop
**Problema:** El override `max-width: none !important` en `.table-responsive` es demasiado agresivo. Tablas de 3 columnas (Ranking, Goleadores) se estiran a ~1200px.
**Solución:** Cambiar el selector para que solo aplique dentro de columnas Bootstrap (`[class*="col-"] .table-responsive`), no globalmente.
**Archivos:** `Futelo.Client/wwwroot/css/components.css`
**Commit:** `fix: scope table-responsive desktop override to bootstrap columns only`

#### ✅ Fix 2: Gráfico ELO — eje X por número de partido en lugar de fecha
**Problema:** Partidos jugados el mismo día muestran fechas repetidas en el eje X del gráfico.
**Solución:** Cambiar labels a número secuencial ("Inicio", "1", "2"...). Solo una línea en el `.razor.cs`.
**Archivos:** `Futelo.Client/Pages/Player/PlayerProfile.razor.cs` (línea ~73)
**Commit:** `fix: use match number instead of date on ELO history chart`

---

### 🟡 Reorganización de información

#### ✅ Mejora 1: Fusionar Goleadores en la página de Ranking General
**Problema:** `/vaults/{id}/scorers` es una página de solo 3 columnas. No justifica existir sola.
**Solución:** Agregar sección de goleadores en `GeneralRanking.razor`, debajo del ranking de ELO. Eliminar el botón "Scorers" de VaultDetail (quedan 5 botones).
**Archivos:**
- `Futelo.Client/Pages/Stats/GeneralRanking.razor` + `.cs` — agregar llamada a scorers y sección
- `Futelo.Client/Pages/VaultDetail.razor` — eliminar botón Scorers
- `Futelo.Client/Pages/Stats/Scorers.razor` — se puede eliminar
**Commit:** `feat: merge scorers into general ranking page`

#### Mejora 2: Mostrar ELO en la lista de jugadores del VaultDetail
**Problema:** La lista de jugadores solo muestra Nombre + Rol. No hay datos de ELO a simple vista.
**Solución:** Mostrar el ELO histórico al lado del nombre (badge pequeño). Verificar si `VaultPlayerResponse` ya incluye `EloRating`; si no, agregarlo.
**Archivos:**
- `Futelo.Shared/DTOs/Vault/VaultDetailResponse.cs` — verificar/agregar `EloRating` en `VaultPlayerResponse`
- `Futelo.Server/Repositories/Vault/VaultRepository.cs` — incluir ELO si falta
- `Futelo.Client/Pages/VaultDetail.razor` — mostrar badge de ELO
**Commit:** `feat: show player ELO in vault detail player list`

#### Mejora 3: Enriquecer SeasonDetail
**Problema:** En temporada activa/finalizada, el usuario solo ve año, videojuego, badges y lista de jugadores. No hay resultados ni standings inline.
**Solución:** Agregar debajo de los jugadores: mini tabla de posiciones de la liga (top 3) y últimos 3 resultados de cualquier competencia de la temporada.
**Archivos:**
- `Futelo.Shared/DTOs/Season/SeasonDetailResponse.cs` — agregar `RecentMatches` y `TopStandings`
- `Futelo.Server/Services/Season/SeasonService.cs` — poblar nuevos campos
- `Futelo.Client/Pages/SeasonDetail.razor` — mostrar nueva sección
**Commit:** `feat: show recent results and mini standings in season detail`

#### Mejora 4: H2H histórico en SuperCupView
**Problema:** La SuperCopa enfrenta dos jugadores pero no muestra contexto histórico entre ellos.
**Solución:** Agregar un panel con el H2H entre los dos jugadores (partidos, victorias, empates). Reutiliza el endpoint `GetHeadToHeadAsync` existente.
**Archivos:**
- `Futelo.Client/Pages/SuperCup/SuperCupView.razor` — agregar sección H2H
- `Futelo.Client/Pages/SuperCup/SuperCupView.razor.cs` — llamar `GetHeadToHeadAsync` con los dos jugadores
**Commit:** `feat: show head-to-head history in supercup view`

---

### 🟢 Mejoras de layout desktop

#### Layout 1: GamesRanking — grid de dos columnas
**Problema:** Múltiples tablas apiladas verticalmente. En desktop con varios juegos queda muy larga.
**Solución:** Mostrar las tablas en grid de dos columnas en desktop (`col-lg-6` por juego).
**Archivos:** `Futelo.Client/Pages/Stats/GamesRanking.razor`
**Commit:** `style: two-column desktop layout for games ranking`

#### Layout 2: SeasonDetail — dos columnas en desktop
**Problema:** Vista de temporada activa es una columna larga.
**Solución:** Columna izquierda: info + jugadores. Columna derecha: mini standings + últimos resultados (requiere Mejora 3 implementada).
**Archivos:** `Futelo.Client/Pages/SeasonDetail.razor`
**Commit:** `style: two-column desktop layout for season detail`

---

### 🔵 Features nuevas

#### Feature 1: Filtros en páginas de historial
**Problema:** VaultMatchHistory y PlayerMatchHistory solo paginan. Sin filtro por competencia.
**Solución:** Selector de tipo (Liga / Copa / Supercopa / Todas) como query param. Requiere cambios en endpoint y repositorio.
**Archivos:**
- `Futelo.Client/Pages/VaultMatchHistory.razor` + `.cs`
- `Futelo.Client/Pages/Player/PlayerMatchHistory.razor` + `.cs`
- `Futelo.Server/Repositories/Stats/StatsRepository.cs` — parámetro de filtro
- `Futelo.Server/Services/Stats/StatsService.cs` — pasar filtro
**Commit:** `feat: add competition filter to match history pages`

---

### 📋 Orden sugerido de implementación

| # | Tarea | Prioridad | Tamaño |
|---|-------|-----------|--------|
| 1 | Fix tablas anchas | 🔴 Urgente | XS |
| 2 | Fix gráfico ELO | 🔴 Urgente | XS |
| 3 | Fusionar Ranking + Goleadores | 🟡 Alto | S |
| 4 | ELO en lista de jugadores | 🟡 Alto | S |
| 5 | GamesRanking dos columnas | 🟢 Medio | XS |
| 6 | H2H en SuperCupView | 🟡 Alto | M |
| 7 | SeasonDetail enriquecido | 🟡 Alto | M |
| 8 | SeasonDetail dos columnas | 🟢 Medio | S |
| 9 | Filtros en historial | 🔵 Nuevo | L |
