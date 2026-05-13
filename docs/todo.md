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

#### Commit 3 — feat: i18n auth pages
- [ ] Login — título, subtítulo, labels, botón, link a register
- [ ] Register — título, subtítulo, labels, botón, link a login

#### Commit 4 — feat: i18n dashboard and vault pages
- [ ] Dashboard (My Vaults, New Vault, empty state, Open)
- [ ] VaultDetail (Players, Seasons, Invite Player, Generate Link, etc.)
- [ ] SeasonDetail (Year, Video Game, Ranking, Finish Season, Players, etc.)
- [ ] CreateVault / CreateSeason

#### Commit 5 — feat: i18n stats pages
- [ ] GeneralRanking, Ranking, Scorers, Palmares
- [ ] GamesRanking, TeamPanel, VaultRecords

#### Commit 6 — feat: i18n player and competition pages
- [ ] PlayerProfile (Stats, Recent form, Current streak, ELO History, etc.)
- [ ] HeadToHead (Match History, headers de tabla)
- [ ] LeagueView, CupView, SuperCupView (Standings, Fixture, Enter Result, etc.)

#### Commit 7 — feat: i18n teams, games and shared components
- [ ] Teams, Games (CRUD forms y labels)
- [ ] Empty states, error messages, skeleton labels

---

### 🔮 Futuro
`feat: add light mode toggle`

- [ ] Segunda definición de variables en `variables.css` activada por clase en `<body>`
- [ ] Toggle de tema guardado en localStorage

---

## Sesión 13 — Pulido y despliegue
Ver roadmap para detalle completo.
