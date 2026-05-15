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

### Sprint A2 — Lógica de negocio compartida (~2h) 🔴 ALTA

- [ ] Extraer `EloCalculator` como clase estática en `Futelo.Server/Services/`
  - `refactor: extract shared EloCalculator from League/Cup/SuperCup services`
- [ ] Reemplazar los 3 usos duplicados de ELO por llamadas a `EloCalculator`
  - `refactor: replace inline ELO logic with EloCalculator in all competition services`

---

### Sprint A3 — Tests (~4h) 🔴 ALTA

- [ ] Crear proyecto `Futelo.Tests` con xUnit
  - `chore: add Futelo.Tests project with xUnit`
- [ ] Tests para `EloCalculator` (casos normales, underdog, favorito, multiplicadores)
  - `test: add EloCalculator unit tests`
- [ ] Tests para `FixtureGenerator` (par/impar, ida y vuelta, bye)
  - `test: add FixtureGenerator unit tests`
- [ ] Tests para `StandingsCalculator` (puntos, diferencia de goles, orden)
  - `test: add StandingsCalculator unit tests`

---

### Sprint A4 — Performance (~2h) 🟡 MEDIA/ALTA

- [ ] Auditar todos los repositories y agregar `AsNoTracking()` en queries de solo lectura
  - `perf: add AsNoTracking to all read-only repository queries`
- [ ] Verificar y completar `Include()` faltantes en repositories
  - `fix: add missing Include() calls to prevent N+1 queries`
- [ ] Agregar `CancellationToken` en los servicios cliente y componentes con carga larga
  - `refactor: add CancellationToken support to client services and components`

---

### Sprint A5 — Refactors de tamaño (~4h) 🟡 MEDIA

- [ ] Extraer `SeasonPlayerManager`, `SeasonCompetitionConfig`, `SeasonTeamSelector` de `SeasonDetail`
  - `refactor: decompose SeasonDetail into focused sub-components`
- [ ] Extraer `FixtureGenerator` de `LeagueService`
  - `refactor: extract FixtureGenerator from LeagueService`
- [ ] Extraer `StandingsCalculator` de `LeagueService`
  - `refactor: extract StandingsCalculator from LeagueService`
- [ ] Descomponer `RecordResultAsync` en métodos privados
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
