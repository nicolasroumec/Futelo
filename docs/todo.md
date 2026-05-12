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

### Sprint 3 — Micro-interactions
`feat: add nav indicator, glow and page transitions`

- [ ] NavLink con indicador animado — línea verde que se desliza entre links en la sidebar
- [ ] Glow verde en #1 — sutil resplandor en la fila del jugador top del ranking
- [ ] Transición de página — fade in suave al navegar

---

### Sprint 4 — Componentes de stats
`feat: add player avatars, win rate bar and form visualization`

- [ ] Avatar con iniciales — círculo con color único generado del nombre (rankings, listas, profile)
- [ ] Forma reciente visual — bloques W/D/L con animación de entrada en cascada (PlayerProfile)
- [ ] Barra de win rate animada — progress bar que se llena al cargar (PlayerProfile)

---

### Sprint 5 — Páginas destacadas
`feat: podium, ELO counter animation and chart theme`

- [ ] Podio top 3 — rediseño de General Ranking con podio para 1°/2°/3°
- [ ] Contador animado — ELO y stats cuentan desde 0 al cargar
- [ ] Chart.js custom — colores y tooltip del gráfico ELO que matcheen la paleta

---

### Sprint 6 — Efectos visuales
`feat: glassmorphism cards`

- [ ] Glassmorphism — `backdrop-filter: blur` con borde semitransparente en cards destacadas

---

### Sprint 7 — i18n
`feat: add i18n ES/EN with language switcher`

- [ ] Mecanismo de traducción (archivos de recursos o diccionarios)
- [ ] Extraer todos los strings hardcodeados de los componentes
- [ ] Componente `LanguageSwitcher` en la navbar
- [ ] Persistir idioma seleccionado en localStorage

---

### 🔮 Futuro
`feat: add light mode toggle`

- [ ] Segunda definición de variables en `variables.css` activada por clase en `<body>`
- [ ] Toggle de tema guardado en localStorage

---

## Sesión 13 — Pulido y despliegue
Ver roadmap para detalle completo.
