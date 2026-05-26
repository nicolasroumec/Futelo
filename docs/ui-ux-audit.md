# Futelo — Auditoría UI/UX y Sistema de Diseño

> Generado: 2026-05-22

---

## PASO 1 — Auditoría visual del estado actual

### 1. Sistema de colores — 4/5
El proyecto tiene un sistema de design tokens en CSS custom properties (`variables.css`) con soporte real de dark/light mode. Paleta coherente: verde primario, semánticos (rojo/amarillo/azul), grises de superficie. El problema son las **fugas**: `.compare-winner` usa `var(--bs-success)` y `.h2h-bar__p2` usa `var(--bs-danger)` — variables de Bootstrap que no respetan el tema y producen colores inconsistentes en light mode.

### 2. Tipografía — 3.5/5
El dúo Barlow Condensed (display) + Inter (body) es una elección sólida con personalidad. El problema es que **`--font-size-base: 15px`** está por debajo del estándar de 16px recomendado en accesibilidad, y al aplicar `.875rem` a tablas y botones el texto resultante es ~13.1px — difícil de leer en pantallas pequeñas. No existe una escala tipográfica formal documentada; los tamaños son arbitrarios por componente.

### 3. Espaciado — 3/5
El sistema usa rem pero **sin una escala fija**. Conviven valores como `0.35rem`, `0.45rem`, `0.55rem`, `0.65rem` (que no son múltiplos de 4px ni 8px) junto a valores "redondos". El resultado son micro-inconsistencias acumuladas que el usuario no nota conscientemente pero percibe como "algo está raro".

### 4. Jerarquía visual — 3.5/5
Las páginas de competición (LeagueView, CupView) son densas: standings + tabs de jornadas + formulario + edición inline compiten visualmente. No hay un punto focal claro. El primer golpe de vista es confuso. Las páginas de stats (Palmares, GeneralRanking) funcionan mejor porque el podio y las barras crean jerarquía natural.

### 5. Componentes — 3.5/5
Cards, botones, badges y formularios son internamente consistentes. Dos gaps técnicos: `--radius-md` se referencia en `.record-card` pero **nunca se define en `variables.css`** (solo existen `--radius: 8px` y `--radius-sm: 5px`) — los record cards tienen un border-radius indefinido en runtime. El sistema de alertas y toasts es sólido.

### 6. Estados interactivos — 3.5/5
Hover y focus están definidos. El estado `disabled` no es consistente — algunos botones disabled se ven igual que habilitados. No hay estados `loading` en botones (post-click). El foco verde es accesible y coherente con el primario.

### 7. Dark mode — 3.5/5
Funciona bien en general. Dos roturas: `.card-glass` usa `rgba(23, 26, 33, 0.6)` hardcodeado (superficie dark) — en light mode aparece como una sombra oscura flotante. El color del banner offline (`#7F1D1D`) es solo válido para dark. La lógica de detección de tema (script en `index.html`) es correcta y previene el FODT.

### 8. Accesibilidad básica — 2.5/5
Tres problemas concretos: (1) Font base 15px, (2) las `.nav-section-label` tienen `font-size: 0.65rem` + `opacity: 0.5` — sobre `#9AA4B2` en dark mode el ratio de contraste es ~1.8:1, muy por debajo del 4.5:1 requerido por WCAG AA, (3) `.match-card--played` aplica `opacity: 0.72` al card completo, reduciendo el contraste de nombres y scores ya en 14px.

---

## PASO 2 — Problemas encontrados (por impacto)

---

**Problema 1 — `--radius-md` indefinido**
**Dónde ocurre:** `PlayerProfile.razor.css` → `.record-card`
**Por qué importa:** El browser interpreta `var(--radius-md)` como `initial` cuando la variable no existe, cayendo al valor por defecto de `border-radius: 0`. Las record cards aparecen con esquinas cuadradas en lugar de redondeadas, rompiendo la consistencia visual sin que sea obvio de debuggear.
**Fix rápido:**
```css
/* variables.css */
--radius-sm:   4px;
--radius:      8px;
--radius-md:  10px;
--radius-lg:  16px;
--radius-full: 9999px;
```

---

**Problema 2 — Base font-size 15px**
**Dónde ocurre:** `app.css` → `html, body` + `--font-size-base`
**Por qué importa:** Con base 15px, `0.875rem` (botones, tablas, form labels) = 13.1px. En móvil o pantallas de baja resolución, este tamaño está por debajo del umbral de legibilidad cómoda. iOS auto-zooms en inputs menores a 16px.
**Fix rápido:**
```css
/* variables.css */
--font-size-base: 16px;

/* app.css */
html, body { font-size: var(--font-size-base); }
```

---

**Problema 3 — Section labels en nav con contraste insuficiente**
**Dónde ocurre:** `layout.css` → `.nav-section-label`
**Por qué importa:** `font-size: 0.65rem` + `opacity: 0.5` sobre `--color-text-muted: #9AA4B2` en dark = ratio de contraste ~1.8:1. Falla WCAG AA (requiere 4.5:1 en texto pequeño). El usuario literalmente no puede leer estas etiquetas.
**Fix rápido:**
```css
.nav-section-label {
  font-size: 0.6875rem;
  font-weight: 700;
  letter-spacing: 0.1em;
  opacity: 0.65;         /* era 0.5 */
  color: var(--color-text-muted);
}
```

---

**Problema 4 — `.card-glass` rota en light mode**
**Dónde ocurre:** `components.css` → `.card-glass`
**Por qué importa:** El efecto glassmorphism usa `rgba(23, 26, 33, 0.6)` — un tono de la superficie dark. En light mode, este componente aparece como un recuadro gris oscuro semitransparente sobre fondo blanco, completamente fuera de lugar.
**Fix rápido:**
```css
/* Dark mode (default) */
.card-glass {
  background: rgba(var(--color-surface-rgb), 0.6);
  backdrop-filter: blur(12px);
}

/* En variables.css añadir: */
/* dark:  */ --color-surface-rgb: 23, 26, 33;
/* light: */ --color-surface-rgb: 255, 255, 255;
```

---

**Problema 5 — Bootstrap variables mezcladas con el sistema de tokens**
**Dónde ocurre:** `PlayerCompare.razor.css` → `.compare-winner` (usa `var(--bs-success)`), `.h2h-bar__p2` (usa `var(--bs-danger)`)
**Por qué importa:** `--bs-success` y `--bs-danger` son los verdes/rojos de Bootstrap sin customizar — no cambian con el tema. En light mode producen un verde o rojo diferente al `--color-primary` y `--color-danger` del sistema, rompiendo la consistencia de color.
**Fix rápido:**
```css
.compare-winner { color: var(--color-success); font-weight: 600; }
.h2h-bar__p2   { background: var(--color-danger); flex: 1; }
```

---

**Problema 6 — Match cards jugadas con opacity global**
**Dónde ocurre:** `LeagueView.razor.css` → `.match-card--played`
**Por qué importa:** `opacity: 0.72` en el card completo reduce el contraste de todos sus hijos (nombres de jugadores, scores, fechas). El score de un partido jugado debería ser el dato más claro de la card, no el más tenue.
**Fix rápido:**
```css
.match-card--played {
  /* Quitar: opacity: 0.72 */
  border-left: 3px solid transparent;
  background: var(--color-surface);
}
.match-card--played .match-meta {
  opacity: 0.55;  /* solo los metadatos se atenúan */
}
```

---

**Problema 7 — Jerarquía visual débil en páginas de competición**
**Dónde ocurre:** `LeagueView.razor`, `CupView.razor`
**Por qué importa:** Standings + control de jornadas + lista de partidos + formulario de resultado compiten al mismo nivel visual. El usuario no sabe qué acción hacer primero.
**Fix rápido:**
```css
.result-form-section {
  background: rgba(34, 197, 94, 0.04);
  border: 1px solid rgba(34, 197, 94, 0.2);
  border-radius: var(--radius);
  padding: 1rem 1.25rem;
}
```

---

**Problema 8 — Espaciado inconsistente en escala sub-8px**
**Dónde ocurre:** Todo el codebase de CSS
**Por qué importa:** Valores como `0.35rem`, `0.45rem`, `0.55rem`, `0.65rem` no corresponden a múltiplos de 4px. El efecto es acumulativo: layouts que "casi" se ven bien pero tienen una incomodidad difusa.
**Fix rápido:** Definir y adoptar spacing tokens `--space-*` (ver Paso 3).

---

## PASO 3 — Sistema de diseño propuesto

### Paleta de colores

```css
/* === PRIMARIO — Verde fútbol === */
--color-primary-500: #22C55E;   /* dark mode */
--color-primary-600: #16A34A;   /* light mode + hover dark */
--color-primary-700: #15803D;   /* light mode hover */
--color-primary:       var(--color-primary-500);
--color-primary-hover: var(--color-primary-600);

/* === SUPERFICIE (dark mode) === */
--color-bg:        #0F1115;
--color-surface:   #171A21;
--color-surface-2: #1F2430;
--color-border:    #2B3240;
--color-surface-rgb: 23, 26, 33;  /* para glassmorphism */

/* === SUPERFICIE (light mode) === */
--color-bg:        #F4F6F9;
--color-surface:   #FFFFFF;
--color-surface-2: #F1F5F9;
--color-border:    #E2E8F0;
--color-surface-rgb: 255, 255, 255;

/* === TEXTO === */
/* dark  */ --color-text: #F5F7FA;  --color-text-muted: #9AA4B2;
/* light */ --color-text: #0F172A;  --color-text-muted: #5A6478;

/* === SEMÁNTICOS === */
--color-success: #22C55E;
--color-danger:  #EF4444;
--color-warning: #FACC15;
--color-info:    #3B82F6;

/* Texto sobre fondos semitransparentes (dark) */
--color-success-text: #86efac;
--color-danger-text:  #fca5a5;
--color-warning-text: #fde68a;
--color-info-text:    #93c5fd;

/* === COMPETICIONES === */
--color-league:   #22C55E;
--color-cup:      #60A5FA;
--color-supercup: #C084FC;

/*
  REGLA DE USO:
  Primary    → CTAs principales, elementos activos, highlights
  Success    → Victorias, confirmaciones, estados positivos
  Danger     → Eliminaciones, errores, acciones destructivas
  Warning    → Empates, alertas no críticas, medallas
  Info       → Información neutral, Copa competition
  Supercup   → Solo para SuperCopa (violeta)
  Text-muted → Labels, metadatos, datos secundarios
  Surface-2  → Hover backgrounds, inputs, zonas inactivas
*/
```

---

### Tipografía

```css
/* === FUENTES === */
--font-display: 'Barlow Condensed', system-ui, sans-serif;
--font-body:    'Inter', system-ui, -apple-system, sans-serif;

/* === ESCALA === */
--text-xs:   0.75rem;    /* 12px — labels, badges, timestamps */
--text-sm:   0.875rem;   /* 14px — metadatos, captions, form labels */
--text-base: 1rem;       /* 16px — cuerpo principal */
--text-lg:   1.125rem;   /* 18px — destacados, card titles */
--text-xl:   1.375rem;   /* 22px — section headings */
--text-2xl:  1.75rem;    /* 28px — page titles */
--text-3xl:  2.25rem;    /* 36px — display / hero numbers */
--text-4xl:  3rem;       /* 48px — ELO destacado, podio */

/*
  PESOS:
  400 (regular)  → cuerpo de texto, descripciones
  500 (medium)   → labels, nav links, botones
  600 (semibold) → títulos de card, badges, datos clave
  700 (bold)     → encabezados de sección, nombres propios
  800 (extrabold)→ brand, podio, números hero (Barlow Condensed)

  LINE-HEIGHT:
  Texto corrido  → 1.6   (body, párrafos)
  Títulos        → 1.2   (headings, display)
  UI compacta    → 1.35  (cards, tablas, nav items)
  Single line    → 1     (badges, chips, números)

  LETTER-SPACING:
  Body           → 0
  Headings       → 0.01em
  Labels upper.  → 0.08em
  Display/Brand  → 0.04em
*/
```

---

### Espaciado

```css
/* Escala de 4px */
--space-1:  0.25rem;   /*  4px — gap mínimo entre iconos e inline elements */
--space-2:  0.5rem;    /*  8px — gaps internos, padding de badges/chips */
--space-3:  0.75rem;   /* 12px — padding de botones, gap entre form elements */
--space-4:  1rem;      /* 16px — padding de cards compactos, gap de listas */
--space-5:  1.25rem;   /* 20px — padding estándar de cards */
--space-6:  1.5rem;    /* 24px — gap entre secciones, padding de modales */
--space-8:  2rem;      /* 32px — separación entre bloques principales */
--space-10: 2.5rem;    /* 40px — padding de páginas en desktop */
--space-12: 3rem;      /* 48px — separación de hero sections */
--space-16: 4rem;      /* 64px — márgenes generosos en páginas de stats */

/*
  CUÁNDO USAR CADA STEP:
  1 (4px)   → gap entre ícono y texto, padding de elementos pill muy pequeños
  2 (8px)   → gap entre items de una lista inline, padding de badges
  3 (12px)  → padding vertical de botones sm, gap de form fields
  4 (16px)  → padding horizontal de nav links, gap vertical de listas
  5 (20px)  → padding de cards (reemplaza 1.25rem)
  6 (24px)  → gap entre cards en grid, padding lateral en tablet
  8 (32px)  → separación entre secciones de página
  10 (40px) → padding lateral en desktop
  12-16     → separación de bloques hero o secciones de landing
*/
```

---

### Border radius

```css
--radius-sm:   4px;      /* inputs, badges, chips, toasts */
--radius:      8px;      /* cards estándar, botones */
--radius-md:  10px;      /* record cards, panels */
--radius-lg:  16px;      /* modales, drawers, cards grandes */
--radius-full: 9999px;   /* avatares, pills, progress bars */
```

---

## PASO 4 — Mejoras por componente

---

**Componente: variables.css (tokens base)**
Antes: Faltan `--radius-md`, `--radius-lg`, `--radius-full`; escala de spacing ad-hoc; base font 15px.
Después:
```css
:root {
  --font-size-base: 16px;

  --radius-sm:   4px;
  --radius:      8px;
  --radius-md:  10px;
  --radius-lg:  16px;
  --radius-full: 9999px;

  --space-1:  0.25rem;
  --space-2:  0.5rem;
  --space-3:  0.75rem;
  --space-4:  1rem;
  --space-5:  1.25rem;
  --space-6:  1.5rem;
  --space-8:  2rem;
  --space-10: 2.5rem;

  --color-surface-rgb: 23, 26, 33;  /* dark */

  --z-nav:     100;
  --z-overlay: 200;
  --z-modal:   300;
  --z-toast:   9999;
}
[data-theme="light"], body.light {
  --color-surface-rgb: 255, 255, 255;
}
```

---

**Componente: .nav-section-label**
Antes: `font-size: 0.65rem; opacity: 0.5` — contraste ~1.8:1, falla WCAG.
Después:
```css
.nav-section-label {
  font-size: 0.6875rem;
  font-weight: 700;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: var(--color-text-muted);
  opacity: 0.65;
  padding: 1rem var(--space-6) 0.25rem;
}
```

---

**Componente: .match-card y .match-card--played**
Antes: `opacity: 0.72` en todo el card reduce contraste de scores y nombres.
Después:
```css
.match-card {
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-left: 3px solid transparent;
  border-radius: var(--radius);
  transition: border-color 0.15s;
}
.match-card--pending {
  border-left-color: var(--color-primary);
}
.match-card--played {
  /* Eliminar opacity: 0.72 */
  border-left-color: transparent;
}
.match-card--played .match-meta,
.match-card--played .match-card__date {
  opacity: 0.55;
}
.match-card--editable:hover {
  border-color: rgba(34, 197, 94, 0.35);
  cursor: pointer;
}
```

---

**Componente: .card-glass**
Antes: `rgba(23, 26, 33, 0.6)` hardcodeado — roto en light mode.
Después:
```css
.card-glass {
  background: rgba(var(--color-surface-rgb), 0.65);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.06);
  box-shadow:
    0 4px 24px rgba(0, 0, 0, 0.2),
    inset 0 1px 0 rgba(255, 255, 255, 0.04);
  transition: box-shadow 0.2s;
}
.card-glass:hover {
  box-shadow:
    0 4px 28px rgba(0, 0, 0, 0.25),
    inset 0 1px 0 rgba(255, 255, 255, 0.06);
}
```

---

**Componente: .compare-winner / .h2h-bar (PlayerCompare)**
Antes: Usa `var(--bs-success)` y `var(--bs-danger)` — no temea correctamente.
Después:
```css
.compare-winner {
  color: var(--color-success);
  font-weight: 600;
}
.compare-loser {
  opacity: 0.45;
}
.h2h-bar__p1 {
  background: var(--color-primary);
}
.h2h-bar__p2 {
  background: var(--color-danger);
  flex: 1;
}
```

---

**Componente: Botones — estado loading**
Antes: No existe estado de carga post-click; el botón queda clickeable mientras se envía la request.
Después:
```css
.btn:disabled,
.btn[disabled] {
  opacity: 0.55;
  cursor: not-allowed;
  pointer-events: none;
}
.btn--loading {
  position: relative;
  color: transparent !important;
  pointer-events: none;
}
.btn--loading::after {
  content: '';
  position: absolute;
  width: 14px;
  height: 14px;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  border: 2px solid currentColor;
  border-right-color: transparent;
  border-radius: 50%;
  animation: btn-spin 0.6s linear infinite;
}
@keyframes btn-spin { to { transform: translate(-50%, -50%) rotate(360deg); } }
```
> En Blazor, añadir/quitar la clase `btn--loading` junto con el atributo `disabled` en el click handler del botón.

---

**Componente: .result-form-section (LeagueView / CupView)**
Antes: El formulario de resultado se confunde visualmente con la lista de partidos.
Después:
```css
.result-form-section {
  background: rgba(34, 197, 94, 0.04);
  border: 1px solid rgba(34, 197, 94, 0.18);
  border-radius: var(--radius);
  padding: var(--space-5);
  margin-top: var(--space-4);
}
.result-form-section__title {
  font-family: var(--font-display);
  font-size: var(--text-lg);
  font-weight: 700;
  color: var(--color-text);
  margin-bottom: var(--space-4);
}
```

---

**Componente: .empty-state**
Antes: `padding: 2rem 1.5rem` genérico, sin ícono ni estructura interna.
Después:
```css
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--space-3);
  padding: var(--space-10) var(--space-6);
  background: var(--color-surface);
  border: 1px dashed var(--color-border);
  border-radius: var(--radius-lg);
  text-align: center;
  color: var(--color-text-muted);
  font-size: var(--text-sm);
  max-width: 420px;
  margin: var(--space-6) auto;
}
.empty-state__icon {
  font-size: 2.5rem;
  opacity: 0.4;
  line-height: 1;
}
.empty-state__title {
  font-family: var(--font-display);
  font-size: var(--text-xl);
  font-weight: 700;
  color: var(--color-text);
}
```

---

## PASO 5 — Antes / después visual por pantalla

---

### Dashboard (lista de Vaults)

**Antes:** Fondo `#0F1115` plano con cards `#171A21`. Las cards son rectángulos simples con nombre, "Owner:" y un contador. El botón "Abrir vault" es idéntico en todas las cards sin jerarquía. Si el usuario tiene 1 solo vault (caso común), la pantalla se ve mayoritariamente vacía con un card flotando.

**Después:** El background del cuerpo de página recibe un sutil `radial-gradient` centrado que rompe la monotonía sin distraer. Las cards ganan un `border-left: 3px solid var(--color-primary)` en hover para indicar interactividad. El nombre del vault usa `var(--font-display)` en `--text-2xl` peso 700 — se lee como título, no como texto. El ELO del usuario y el número de temporadas activas aparecen como chips de color debajo del nombre. El botón CTA principal se mueve al header de la card (visible de inmediato) en lugar del footer. La sensación pasa de "lista de archivos" a "tablero de competiciones".

---

### League View (la pantalla más usada)

**Antes:** Dos columnas: standings a la izquierda (tabla densa), jornadas a la derecha (tabs + lista de partidos + formulario inline). Todo al mismo nivel visual. El usuario que entra a "registrar el resultado de hoy" tiene que escanear toda la pantalla para encontrar el partido pendiente.

**Después:** Los partidos pendientes se separan visualmente al principio del tab activo con un highlight `result-form-section` en verde muy tenue. Los partidos jugados visualmente "retroceden" (border transparente, sin el verde lateral) pero el score se lee con la misma claridad (ya no hay opacity global). Las pestañas de jornada con el punto verde son el único call-to-action que destaca. La tabla de standings gana un header más prominente (`--text-xl`, Barlow Condensed) que la separa como "sección propia". La sensación pasa de "hoja de cálculo" a "marcador de liga".

---

### Player Profile

**Antes:** El gráfico de ELO y las record cards compiten al mismo nivel que la tabla de estadísticas y el historial de partidos. El badge de ELO en el header usa el mismo tamaño que cualquier otro badge. La progress bar de win rate es el único elemento con animación, pero pasa desapercibida porque está rodeada de texto.

**Después:** El ELO en el header se renderiza en `--text-4xl` (Barlow Condensed 800) como número hero — el primer dato que el ojo ve. La win rate bar pasa de 6px de altura a 8px y gana un label inline con el porcentaje exacto. Las record cards ya tienen su `--radius-md` correcto y se organizan en grid 2-col en móvil / 3-col en desktop con más `gap`. El gráfico de ELO tiene su sección propia con título ("Evolución ELO") claramente demarcada. La sensación pasa de "página de estadísticas densas" a "perfil de jugador con personalidad".

---

### General Ranking (Podio)

**Antes:** El podio es el elemento más logrado visualmente de toda la app — con buena escala, colores correctos y animación. El único problema: el podio está `max-width: 480px` centrado, y debajo hay una tabla estándar sin relación visual clara. La transición entre "top 3" y "el resto" es abrupta.

**Después:** Añadir un separador visual entre el podio y la tabla: un `divider` con el texto "Clasificación completa" en `--text-xs uppercase letter-spacing`. La tabla debajo gana un rank coloreado: 🥇🥈🥉 en las primeras tres filas como emoji en la celda de posición. Los jugadores con ELO en subida reciben `color: var(--color-success)` en su celda de cambio. La sensación pasa de "tabla con podio decorativo" a "pantalla de clasificación cohesionada".

---

## Deuda técnica de diseño

**DT-D1 — Spacing tokens no usados consistentemente:** Para implementar cualquier componente nuevo (activity feed, season recap), el developer tendrá que elegir valores de padding/gap ad-hoc, perpetuando la inconsistencia. Adoptar `--space-*` como estándar antes de añadir features es un esfuerzo de 2-3 horas con alto retorno.

**DT-D2 — Sin definición de z-index layers:** El OfflineIndicator (z:1200), PwaUpdater (z:1100), NavMenu (z:100), y potenciales modales/dropdowns futuros no tienen un sistema de capas definido. Cuando se añadan notificaciones in-app o un modal de draft, los z-index chocarán.
```css
/* Añadir a variables.css */
--z-nav:     100;
--z-overlay: 200;
--z-modal:   300;
--z-toast:   9999;
```

---

---

# Auditoría Bootstrap 5 — Cuatro Dimensiones

> Complemento al audit anterior. Todo el código usa **Bootstrap 5**. CSS custom solo cuando Bootstrap no cubre el caso.

---

## DIMENSIÓN 1 — Navegación y flujos

### Auditoría

**Problema grave #1 — Sin breadcrumbs en páginas de profundidad 3+**
**Dónde:** LeagueView, CupView, SuperCupView, PlayerProfile, todas las páginas de stats.
**Por qué importa:** El usuario que está en `/leagues/42` no sabe visualmente que esa liga pertenece a "Temporada 2025" del Vault "Los pibes". El único mecanismo de orientación es el botón `← Back` que solo lleva un nivel arriba. Si el usuario llegó via un link directo (compartido), está completamente desorientado.

```html
<!-- Bootstrap 5 — breadcrumb para LeagueView / CupView / SeasonDetail -->
<!-- Reemplaza el actual btn btn-outline-secondary btn-sm "← Back" -->
<nav aria-label="breadcrumb" class="mb-3">
  <ol class="breadcrumb mb-0 small">
    <li class="breadcrumb-item">
      <a href="/dashboard" class="text-decoration-none text-muted">Dashboard</a>
    </li>
    <li class="breadcrumb-item">
      <a href="/vaults/@VaultId" class="text-decoration-none text-muted">@VaultName</a>
    </li>
    <li class="breadcrumb-item">
      <a href="/seasons/@SeasonId" class="text-decoration-none text-muted">@SeasonName</a>
    </li>
    <li class="breadcrumb-item active fw-semibold" aria-current="page">@LeagueName</li>
  </ol>
</nav>
```

```css
/* Adaptar colores del breadcrumb al tema */
.breadcrumb-item + .breadcrumb-item::before {
  color: var(--color-border);
}
.breadcrumb-item a {
  color: var(--color-text-muted);
  transition: color 0.15s;
}
.breadcrumb-item a:hover {
  color: var(--color-text);
}
.breadcrumb-item.active {
  color: var(--color-text);
}
```

---

**Problema grave #2 — Setup de temporada sin guía de pasos (5+ clics sin contexto)**
**Dónde:** Flujo Create Season → SeasonCompetitionConfig → SeasonPlayerManager → SeasonTeamSelector → Activate.
**Por qué importa:** El usuario no sabe cuántos pasos faltan ni en cuál está. Abandona el flujo o activa la temporada sin completar la configuración.

```html
<!-- Bootstrap 5 — stepper de configuración de temporada -->
<!-- Insertar en SeasonDetail cuando Status = Draft -->
<div class="d-flex align-items-center gap-0 mb-4 overflow-auto pb-1">

  <!-- Paso 1: Config completado -->
  <div class="d-flex align-items-center gap-2 flex-shrink-0">
    <span class="badge rounded-pill bg-success" style="width:28px;height:28px;
          display:inline-flex;align-items:center;justify-content:center">✓</span>
    <span class="small fw-semibold">Competiciones</span>
  </div>

  <div class="flex-fill mx-2" style="height:2px;background:var(--color-border);min-width:24px"></div>

  <!-- Paso 2: Activo -->
  <div class="d-flex align-items-center gap-2 flex-shrink-0">
    <span class="badge rounded-pill bg-primary" style="width:28px;height:28px;
          display:inline-flex;align-items:center;justify-content:center;font-size:.75rem">2</span>
    <span class="small fw-semibold">Jugadores</span>
  </div>

  <div class="flex-fill mx-2" style="height:2px;background:var(--color-border);min-width:24px"></div>

  <!-- Paso 3: Pendiente -->
  <div class="d-flex align-items-center gap-2 flex-shrink-0">
    <span class="badge rounded-pill bg-secondary text-muted" style="width:28px;height:28px;
          display:inline-flex;align-items:center;justify-content:center;font-size:.75rem">3</span>
    <span class="small text-muted">Equipos</span>
  </div>

  <div class="flex-fill mx-2" style="height:2px;background:var(--color-border);min-width:24px"></div>

  <!-- Paso 4: Pendiente -->
  <div class="d-flex align-items-center gap-2 flex-shrink-0">
    <span class="badge rounded-pill bg-secondary text-muted" style="width:28px;height:28px;
          display:inline-flex;align-items:center;justify-content:center;font-size:.75rem">4</span>
    <span class="small text-muted">Activar</span>
  </div>

</div>
```

---

**Problema grave #3 — CTA "Activar temporada" enterrado al fondo de la página**
**Dónde:** `SeasonDetail.razor` — el botón Start Season está en el bottom de la columna derecha, después de standings y partidos recientes.
**Por qué importa:** En Draft mode, la única acción que importa es activar la temporada. Ponerla al fondo obliga a scrollear y el usuario puede no encontrarla.

```html
<!-- Bootstrap 5 — sticky action bar para Draft seasons -->
<!-- Reemplaza la ubicación actual del botón al fondo -->
@if (season.Status == SeasonStatus.Draft && CanEdit)
{
  <div class="d-flex align-items-center justify-content-between p-3 mb-4 rounded-3"
       style="background:rgba(34,197,94,.06);border:1px solid rgba(34,197,94,.2)">
    <div>
      <div class="fw-semibold small">Temporada en borrador</div>
      <div class="text-muted" style="font-size:.8rem">
        Completá los pasos y activá la temporada para comenzar.
      </div>
    </div>
    <button class="btn btn-success btn-sm fw-semibold ms-3 flex-shrink-0"
            @onclick="ActivateSeason" disabled="@isActivating">
      @if (isActivating)
      {
        <span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>
      }
      Activar temporada →
    </button>
  </div>
}
```

**Regla general de navegación:** Todo destino de profundidad ≥ 2 lleva breadcrumb. Los back buttons se reemplazan o complementan con breadcrumbs (no se eliminan — el back button en mobile sigue siendo útil). Las acciones primarias de una página van siempre en el header, nunca al fondo.

---

## DIMENSIÓN 2 — Formularios e inputs

### Auditoría

**Formulario problemático #1 — MatchResultForm sin labels visibles**
**Dónde:** `MatchResultForm.razor`
**Por qué importa:** Los inputs de score usan solo `placeholder="Home"` / `placeholder="Away"`. Al escribir el primer número, el placeholder desaparece y el usuario pierde el contexto — especialmente crítico en el formulario de penales donde hay 4 inputs sin labels.

```html
<!-- Bootstrap 5 — MatchResultForm mejorado con labels -->
<div class="border-top pt-3 mt-2">

  <!-- Score principal -->
  <div class="d-flex align-items-end gap-3 mb-3">
    <div class="text-center">
      <label class="form-label form-label-sm fw-semibold mb-1 d-block">@HomePlayerName</label>
      <input type="number" class="form-control form-control-sm text-center fw-bold"
             style="width:72px;font-size:1.2rem"
             min="0" @bind="HomeScore" placeholder="0" />
    </div>
    <div class="pb-1 text-muted fw-bold fs-5">—</div>
    <div class="text-center">
      <label class="form-label form-label-sm fw-semibold mb-1 d-block">@AwayPlayerName</label>
      <input type="number" class="form-control form-control-sm text-center fw-bold"
             style="width:72px;font-size:1.2rem"
             min="0" @bind="AwayScore" placeholder="0" />
    </div>
  </div>

  <!-- Penales (condicional) -->
  @if (ShowPenalties)
  {
    <div class="p-2 rounded-2 mb-3" style="background:var(--color-surface-2)">
      <div class="small fw-semibold text-muted mb-2 text-uppercase"
           style="font-size:.7rem;letter-spacing:.06em">Penales</div>
      <div class="mb-2">
        <label class="form-label form-label-sm mb-1">¿Quién ganó?</label>
        <select class="form-select form-select-sm" style="max-width:200px" @bind="WonOnPenaltiesId">
          <option value="">— Seleccionar —</option>
          <option value="@HomePlayerId">@HomePlayerName</option>
          <option value="@AwayPlayerId">@AwayPlayerName</option>
        </select>
      </div>
      @if (WonOnPenaltiesId is not null)
      {
        <div class="d-flex align-items-end gap-3">
          <div class="text-center">
            <label class="form-label form-label-sm mb-1">@HomePlayerName</label>
            <input type="number" class="form-control form-control-sm text-center"
                   style="width:72px" min="0" @bind="HomePenalties" placeholder="0" />
          </div>
          <div class="pb-1 text-muted fw-bold">—</div>
          <div class="text-center">
            <label class="form-label form-label-sm mb-1">@AwayPlayerName</label>
            <input type="number" class="form-control form-control-sm text-center"
                   style="width:72px" min="0" @bind="AwayPenalties" placeholder="0" />
          </div>
        </div>
      }
    </div>
  }

  <!-- Acciones -->
  <div class="d-flex gap-2">
    <button class="btn btn-success btn-sm fw-semibold" @onclick="SaveResult" disabled="@isSaving">
      @if (isSaving)
      {
        <span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>
      }
      Guardar resultado
    </button>
    <button class="btn btn-outline-secondary btn-sm" @onclick="Cancel">Cancelar</button>
  </div>

</div>
```

---

**Formulario problemático #2 — Delete confirmation inline (peligroso)**
**Dónde:** `SeasonDetail.razor` → botón "Eliminar temporada"
**Por qué importa:** La confirmación inline `"¿Estás seguro? Sí / No"` es clickeable accidentalmente con doble-click. Para una acción destructiva irreversible, Bootstrap Modal con texto de confirmación explícito es el patrón correcto.

```html
<!-- Bootstrap 5 — Modal de confirmación de eliminación -->
<!-- Trigger (reemplaza el botón inline de confirmación) -->
<button class="btn btn-outline-danger btn-sm"
        data-bs-toggle="modal" data-bs-target="#deleteSeasonModal">
  Eliminar temporada
</button>

<!-- Modal -->
<div class="modal fade" id="deleteSeasonModal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog modal-dialog-centered modal-sm">
    <div class="modal-content" style="background:var(--color-surface);border-color:var(--color-border)">
      <div class="modal-body p-4">
        <h6 class="fw-bold mb-1">Eliminar "@season.Name"</h6>
        <p class="text-muted small mb-4">
          Esta acción eliminará todos los partidos, resultados y cambios de ELO
          de esta temporada. No se puede deshacer.
        </p>
        <div class="d-flex gap-2 justify-content-end">
          <button type="button" class="btn btn-outline-secondary btn-sm"
                  data-bs-dismiss="modal">Cancelar</button>
          <button type="button" class="btn btn-danger btn-sm fw-semibold"
                  @onclick="DeleteSeason" data-bs-dismiss="modal">
            Eliminar definitivamente
          </button>
        </div>
      </div>
    </div>
  </div>
</div>
```

---

**Formulario problemático #3 — "Add Match" inline con widths hardcodeados**
**Dónde:** `LeagueView.razor` — sección de entrada manual de partidos
**Por qué importa:** Los selects de jugadores tienen `style="width:160px"` que en tablet (768px) con dos columnas dejan el formulario completamente comprimido.

```html
<!-- Bootstrap 5 — Add Match form responsive -->
<div class="card mb-3" style="border-color:rgba(34,197,94,.2);background:rgba(34,197,94,.03)">
  <div class="card-body p-3">
    <h6 class="fw-semibold mb-3 small text-uppercase"
        style="letter-spacing:.06em;color:var(--color-text-muted)">Agregar partido</h6>
    <div class="row g-2 align-items-end">
      <div class="col-6 col-sm-auto">
        <label class="form-label form-label-sm">Jornada</label>
        <input type="number" class="form-control form-control-sm"
               style="max-width:80px" min="1" @bind="NewMatchMatchday" />
      </div>
      <div class="col-6 col-sm">
        <label class="form-label form-label-sm">Local</label>
        <select class="form-select form-select-sm" @bind="NewMatchHomeId">
          <option value="">— Jugador —</option>
          @foreach (var p in Players)
          {
            <option value="@p.Id">@p.DisplayName</option>
          }
        </select>
      </div>
      <div class="col-6 col-sm">
        <label class="form-label form-label-sm">Visitante</label>
        <select class="form-select form-select-sm" @bind="NewMatchAwayId">
          <option value="">— Jugador —</option>
          @foreach (var p in Players)
          {
            <option value="@p.Id">@p.DisplayName</option>
          }
        </select>
      </div>
      <div class="col-6 col-sm-auto">
        <button class="btn btn-primary btn-sm w-100" @onclick="AddMatch" disabled="@isAddingMatch">
          Agregar
        </button>
      </div>
    </div>
  </div>
</div>
```

**Reglas que aplican a TODOS los formularios de la app:**
1. **Siempre labels visibles** — nunca solo placeholder como único identificador de campo.
2. **Campos required** — marcar con `<span class="text-danger ms-1" aria-hidden="true">*</span>` junto al label.
3. **Acciones destructivas** — siempre en Bootstrap Modal, nunca inline.
4. **Submit buttons** — siempre con spinner `spinner-border-sm` durante el loading y `disabled` durante el envío.
5. **Formularios de más de 4 campos** — agrupar en secciones con `<hr class="my-3">` o cards separadas.
6. **Errores** — usar `is-invalid` + `<div class="invalid-feedback">` en el campo que falló, además del alert general.

---

## DIMENSIÓN 3 — Jerarquía visual

### Auditoría

**Pantalla: Dashboard**
- **Actual:** `<h3 class="mb-0">` sin peso extra. Cards con `card-title` igual al de cualquier card interna.
- **Ideal:** El nombre del vault es el dato primario. ELO y temporadas activas son secundarios. El CTA es terciario.

```html
<!-- Bootstrap 5 — Vault card con jerarquía clara -->
<div class="card h-100">
  <div class="card-body">
    <!-- Primario: nombre del vault -->
    <h2 class="h5 fw-bold mb-1" style="font-family:var(--font-display)">@vault.Name</h2>
    <!-- Secundario: metadatos -->
    <div class="d-flex align-items-center gap-2 mb-3">
      <span class="badge bg-secondary">@vault.PlayerCount jugadores</span>
      @if (vault.HasActiveSeason)
      {
        <span class="badge" style="background:rgba(34,197,94,.15);color:var(--color-primary)">
          Temporada activa
        </span>
      }
    </div>
    <!-- Terciario: owner info -->
    <p class="text-muted small mb-0">
      Creado por <span class="fw-semibold">@vault.OwnerName</span>
    </p>
  </div>
  <!-- CTA en footer, siempre visible -->
  <div class="card-footer bg-transparent pt-0 border-0 pb-3 px-3">
    <a href="/vaults/@vault.Id" class="btn btn-outline-primary btn-sm w-100">
      Abrir vault →
    </a>
  </div>
</div>
```

---

**Pantalla: PlayerProfile — ELO como KPI hero**
- **Actual:** ELO es un `badge` del mismo tamaño que los demás badges.
- **Ideal:** El ELO global es el número más importante del perfil. Debe tener el peso visual de un KPI.

```html
<!-- Bootstrap 5 — ELO como número hero en header de PlayerProfile -->
<div class="d-flex align-items-center gap-4 mb-4">
  <!-- Avatar -->
  <PlayerAvatar DisplayName="@player.DisplayName" Size="72" />

  <!-- Info -->
  <div class="flex-fill">
    <h1 class="h3 fw-bold mb-1" style="font-family:var(--font-display)">
      @player.DisplayName
    </h1>
    <div class="d-flex align-items-baseline gap-3 flex-wrap">
      <!-- ELO como número grande -->
      <div>
        <span class="fw-bold" style="font-family:var(--font-display);
              font-size:2.25rem;color:var(--color-primary);line-height:1">
          @player.EloRating
        </span>
        <span class="text-muted small ms-1 text-uppercase fw-semibold"
              style="font-size:.7rem;letter-spacing:.06em">ELO</span>
      </div>
      <!-- Separador -->
      <span class="text-muted opacity-25">|</span>
      <!-- Partidos jugados — secundario -->
      <div class="text-muted small">
        <span class="fw-semibold text-body">@player.TotalMatches</span> partidos
      </div>
    </div>
  </div>
</div>
```

---

**Pantalla: LeagueView — Score como dato primario de la match card**
- **Actual:** Score y nombres de jugadores tienen el mismo peso tipográfico.
- **Ideal:** El resultado (ej "3-1") es el dato hero. Los nombres son secundarios.

```html
<!-- Bootstrap 5 — Match card con jerarquía score > nombres -->
<div class="match-card @GetMatchCardClass(match) p-2 mb-2">
  <div class="d-flex align-items-center gap-2">

    <!-- Jugador local -->
    <div class="flex-fill text-end">
      <span class="small fw-semibold">@match.HomePlayerName</span>
      @if (!string.IsNullOrEmpty(match.HomeTeamName))
      {
        <div class="text-muted" style="font-size:.72rem">@match.HomeTeamName</div>
      }
    </div>

    <!-- Score — dato primario -->
    <div class="text-center flex-shrink-0 px-2">
      @if (match.IsPlayed)
      {
        <div class="fw-bold" style="font-family:var(--font-display);
             font-size:1.35rem;line-height:1;min-width:60px">
          @match.HomeScore&nbsp;–&nbsp;@match.AwayScore
        </div>
        @if (match.WonOnPenalties is not null)
        {
          <div class="text-muted" style="font-size:.68rem">pen.</div>
        }
      }
      else
      {
        <span class="badge bg-secondary opacity-50 px-2">vs</span>
      }
    </div>

    <!-- Jugador visitante -->
    <div class="flex-fill text-start">
      <span class="small fw-semibold">@match.AwayPlayerName</span>
      @if (!string.IsNullOrEmpty(match.AwayTeamName))
      {
        <div class="text-muted" style="font-size:.72rem">@match.AwayTeamName</div>
      }
    </div>

  </div>
</div>
```

**Reglas de tipografía Bootstrap para toda la app:**

| Elemento | Clases Bootstrap 5 |
|----------|-------------------|
| Título de página | `h2 fw-bold` + `font-family: var(--font-display)` |
| Título de sección | `h5 fw-semibold mb-3` |
| Título de card | `h6 fw-semibold mb-1` |
| KPI / número hero | `display-5 fw-bold` + `font-display` |
| Score de partido | `fw-bold fs-5` + `font-display` |
| Label de campo | `form-label form-label-sm fw-semibold` |
| Metadato / caption | `text-muted small` |
| Badge de estado | `badge` + color semántico |
| Texto de tabla | `small` (heredado del `table-sm`) |

**Reglas de spacing Bootstrap para separar secciones:**

```html
<!-- Entre secciones de una misma página -->
<div class="mb-4"><!-- sección 1 --></div>
<div class="mb-4"><!-- sección 2 --></div>

<!-- Entre título y contenido de una sección -->
<h5 class="fw-semibold mb-3">Standings</h5>

<!-- Entre cards en grid -->
<div class="row g-3"><!-- g-3 = 16px gap --></div>

<!-- Padding interno de cards -->
<div class="card-body p-3"><!-- p-3 = 16px --></div>
<!-- O para cards más generosas: -->
<div class="card-body p-4"><!-- p-4 = 24px --></div>

<!-- Separador visual entre sub-secciones -->
<hr class="my-3" style="border-color:var(--color-border)">
```

---

## DIMENSIÓN 4 — Feedback al usuario

### Auditoría

**Feedback #1 — Botones de acción sin spinner visual**
**Dónde:** Todos los botones de guardado en la app.
**Por qué importa:** El texto "Guardando..." solo es visible si el usuario lee el botón justo después de clickear. Un spinner es un signal visual más robusto que no requiere leer.

```html
<!-- Bootstrap 5 — patrón estándar para botones con loading -->
<!-- Usar en: SaveResult, ActivateSeason, CreateVault, CreateSeason, etc. -->
<button class="btn btn-primary btn-sm" @onclick="HandleSubmit"
        disabled="@isLoading">
  @if (isLoading)
  {
    <span class="spinner-border spinner-border-sm me-2"
          role="status" aria-hidden="true"></span>
    <span>Guardando...</span>
  }
  else
  {
    <span>Guardar resultado</span>
  }
</button>

<!-- Para botones destructivos -->
<button class="btn btn-danger btn-sm" @onclick="Delete" disabled="@isDeleting">
  @if (isDeleting)
  {
    <span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
    <span>Eliminando...</span>
  }
  else
  {
    <span>Eliminar</span>
  }
</button>
```

---

**Feedback #2 — Alert de resultado exitoso permanente en página**
**Dónde:** `LeagueView.razor` — el `alert alert-success` con el resultado y el cambio de ELO queda en pantalla indefinidamente.
**Por qué importa:** Un alert permanente que no se puede cerrar genera ruido visual. Después de 3 resultados en la misma jornada, la parte superior de la columna queda llena de alerts verdes.

El proyecto ya tiene un `ToastService` custom. El patrón correcto es redirigir el success al toast y eliminar el alert inline.

```razor
@* En el code-behind, reemplazar la asignación de successMessage por: *@

// Antes:
// successMessage = $"Resultado: {home.Name} {result.HomeScore}-{result.AwayScore} {away.Name}";

// Después (usando el ToastService existente):
ToastService.ShowSuccess(
  Lang.Get("common.resultRecorded"),
  $"{FormatEloChange(result.Home)} · {FormatEloChange(result.Away)}"
);
// Y eliminar el <div class="alert alert-success"> del markup
```

---

**Feedback #3 — Estados vacíos sin guía ni acción**
**Dónde:** `EmptyState.razor` — actualmente es un `<p class="empty-state">@Message</p>`.
**Por qué importa:** Una pantalla vacía sin ícono, sin título y sin CTA deja al usuario sin saber qué hacer. El zero state es una oportunidad de guiar al usuario a la siguiente acción.

```html
<!-- Bootstrap 5 — EmptyState component mejorado -->
<!-- Reemplaza el contenido actual de EmptyState.razor -->
<div class="text-center py-5 px-3">
  @if (Icon is not null)
  {
    <div class="mb-3 opacity-25">@Icon</div>
  }
  <p class="fw-semibold mb-1">@Title</p>
  <p class="text-muted small mb-0">@Message</p>
  @if (ActionLabel is not null && ActionHref is not null)
  {
    <a href="@ActionHref" class="btn btn-outline-primary btn-sm mt-3">@ActionLabel</a>
  }
  else if (ActionLabel is not null && OnAction.HasDelegate)
  {
    <button class="btn btn-outline-primary btn-sm mt-3" @onclick="OnAction">@ActionLabel</button>
  }
</div>
```

```csharp
// EmptyState.razor.cs — parámetros nuevos
[Parameter] public string? Title { get; set; }
[Parameter] public string Message { get; set; } = "";
[Parameter] public RenderFragment? Icon { get; set; }
[Parameter] public string? ActionLabel { get; set; }
[Parameter] public string? ActionHref { get; set; }
[Parameter] public EventCallback OnAction { get; set; }
```

Ejemplo de uso en Dashboard cuando no hay vaults:
```html
<EmptyState Title="Todavía no tenés ningún vault"
            Message="Creá un vault para empezar a registrar partidos con tus amigos.">
  <Icon>
    <svg width="48" height="48" ...><!-- ícono de grupo --></svg>
  </Icon>
  <ActionLabel>Crear primer vault</ActionLabel>
  <ActionHref>/vaults/create</ActionHref>
</EmptyState>
```

---

**Feedback #4 — Errores de formulario solo en alert global, no inline**
**Dónde:** `CreateVault.razor`, `CreateSeason.razor` — el error va a `alert alert-danger` arriba del formulario.
**Por qué importa:** Cuando un formulario falla, el usuario quiere saber exactamente qué campo está mal, no leer un mensaje genérico y volver a adivinar.

```html
<!-- Bootstrap 5 — validación inline con is-invalid -->
<!-- Patrón a aplicar en todos los formularios con EditForm -->
<div class="mb-3">
  <label class="form-label fw-semibold" for="vaultName">
    Nombre del vault
    <span class="text-danger ms-1" aria-hidden="true">*</span>
  </label>
  <InputText id="vaultName"
             class="form-control @(fieldHasError("Name") ? "is-invalid" : "")"
             @bind-Value="model.Name"
             placeholder="Ej: Los Pibes" />
  <ValidationMessage For="() => model.Name" class="invalid-feedback" />
</div>

<!-- Alert global solo para errores del servidor (no de validación de campos) -->
@if (!string.IsNullOrEmpty(serverError))
{
  <div class="alert alert-danger d-flex align-items-center gap-2 py-2 mt-3" role="alert">
    <svg width="16" height="16" fill="currentColor" viewBox="0 0 16 16" aria-hidden="true">
      <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14m0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16"/>
      <path d="M7.002 11a1 1 0 1 1 2 0 1 1 0 0 1-2 0M7.1 4.995a.905.905 0 1 1 1.8 0l-.35 3.507a.553.553 0 0 1-1.1 0z"/>
    </svg>
    <span class="small">@serverError</span>
  </div>
}
```

**Regla general de feedback para toda la app:**
1. **Acciones exitosas** → siempre mediante `ToastService` (ya existe). Nunca alerts permanentes inline.
2. **Errores de campo** → `is-invalid` + `<div class="invalid-feedback">` en el campo específico.
3. **Errores de servidor** → `alert alert-danger` con ícono, solo para errores no mapeables a un campo.
4. **Loading de botón** → siempre `spinner-border-sm` + `disabled`. Nunca solo cambio de texto.
5. **Zero states** → `EmptyState` con `Title` + `Message` + CTA siempre que sea posible.
6. **Acciones destructivas** → Bootstrap Modal de confirmación, nunca inline.

---

## Plan de acción priorizado

| # | Mejora | Dimensión | Esfuerzo | Impacto |
|---|--------|-----------|----------|---------|
| ~~1~~ | ~~⚡ Spinner en todos los botones de guardado~~ ✅ | ~~Feedback~~ | ~~S~~ | ~~Alto~~ |
| 2 | ⚡ Labels visibles en MatchResultForm (reemplazar solo-placeholder) | Formularios | S | Alto |
| ~~3~~ | ~~⚡ Eliminar `alert-success` permanente; redirigir al ToastService~~ ✅ | ~~Feedback~~ | ~~S~~ | ~~Alto~~ |
| 4 | ⚡ `is-invalid` + `invalid-feedback` inline en todos los EditForm | Formularios | S | Alto |
| ~~5~~ | ~~⚡ Score como dato primario en match cards (`fs-5 fw-bold font-display`)~~ ✅ | ~~Jerarquía~~ | ~~S~~ | ~~Alto~~ |
| ~~6~~ | ~~⚡ ELO como número hero en PlayerProfile (`display-5 fw-bold`)~~ ✅ | ~~Jerarquía~~ | ~~S~~ | ~~Alto~~ |
| ~~7~~ | ~~⚡ Botón "Activar temporada" elevado al top de SeasonDetail Draft~~ ✅ | ~~Navegación~~ | ~~S~~ | ~~Alto~~ |
| 8 | Breadcrumbs en LeagueView / CupView / SeasonDetail | Navegación | M | Alto |
| 9 | EmptyState con Icon + Title + CTA button | Feedback | M | Medio |
| 10 | Modal de confirmación para Delete Season / Delete Vault | Formularios | M | Alto |
| 11 | Stepper de configuración de temporada (Draft flow) | Navegación | M | Medio |
| 12 | Add Match form responsive (col Bootstrap, sin widths hardcodeados) | Formularios | M | Medio |
| 13 | Jerarquía de vault cards en Dashboard (h5 nombre, badge estado) — ⚠️ parcial: nombre `fw-bold` + badge jugadores + CTA con flecha ✅; badge "Temporada activa" pendiente (requiere `HasActiveSeason` en `VaultResponse`) | Jerarquía | M | Medio |
| ~~14~~ | ~~`--radius-md` definido + adoptado en record cards~~ ✅ | ~~CSS tokens~~ | ~~S~~ | ~~Medio~~ |
| ~~15~~ | ~~Base font-size de 15px → 16px~~ ✅ | ~~CSS tokens~~ | ~~S~~ | ~~Medio~~ |
| ~~16~~ | ~~Section labels nav: opacity 0.5 → 0.65~~ ✅ | ~~Accesibilidad~~ | ~~S~~ | ~~Medio~~ |
| ~~17~~ | ~~`card-glass` temeable via `--color-surface-rgb`~~ ✅ | ~~CSS tokens~~ | ~~S~~ | ~~Bajo~~ |
| 18 | Bootstrap vars (`--bs-success`) → tokens propios (`--color-success`) — pendiente hasta implementar `PlayerCompare` | CSS tokens | S | Bajo |
| ~~19~~ | ~~`match-card--played` sin opacity global; solo `.match-meta` atenuado~~ ✅ | ~~CSS tokens~~ | ~~S~~ | ~~Medio~~ |
| 20 | Spacing tokens `--space-*` adoptados como estándar en nuevos componentes | CSS tokens | L | Bajo |

> ⚡ = Aplicable en < 1 hora cambiando clases Bootstrap o snippets de Razor. Sin nueva arquitectura.
