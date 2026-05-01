# Roadmap de sesiones - Futelo

## Sesión 1 — Base del proyecto ✅
- Crear solución con Server, Client y Shared
- Instalar paquetes NuGet
- Crear documentación base
- Subir repo inicial a GitHub

## Sesión 2 — Dominio y base de datos
- Modelos EF Core (Season, League, Cup, SuperCup, Match, VideoGame, Team, etc.)
- AppDbContext con configuraciones Fluent API
- Primera migración y seed de datos de prueba
- Verificar que la DB se crea correctamente

## Sesión 3 — Autenticación
- ASP.NET Core Identity configurado
- Endpoints: POST /auth/register, POST /auth/login
- Generación y validación de JWT
- Servicio de auth en el Client (localStorage para token)
- Páginas: Login.razor, Register.razor

## Sesión 4 — Vault + Catálogos
- CRUD de vaults (crear, ver, editar)
- Invitar jugadores al vault por email (VaultInvitation)
- CRUD de videojuegos (VideoGame)
- CRUD de equipos (Team)
- Páginas: Dashboard.razor, VaultDetail.razor, CreateVault.razor, Games.razor, Teams.razor

## Sesión 5 — Temporadas
- Crear temporada dentro de un vault (elegir jugadores del vault)
- Configurar League, Cup, SuperCup (on/off + elegir jugadores por torneo)
- Páginas: SeasonDetail.razor, CreateSeason.razor

## Sesión 6 — League
- Generación automática de fixture (round-robin, soporta número impar con "libre")
- Re-sorteo del fixture mientras no haya resultados cargados
- Tabla de posiciones
- Cargar resultados: equipo local, equipo visitante, videojuego, goles
- Cálculo y visualización de ELO al cargar resultado (antes → después, cambio de posición)
- Páginas: League/LeagueView.razor

## Sesión 7 — Cup
- Generación del bracket (4, 5, 6, 8 jugadores)
- Vista del bracket (árbol visual)
- Manejo de ida y vuelta (aggregate) y penales
- Avance automático a la siguiente ronda
- Páginas: Cup/CupView.razor

## Sesión 8 — SuperCup
- Lógica para determinar los participantes
- Partido(s) simple(s) con registro de equipo y videojuego
- Página: SuperCup/SuperCupView.razor

## Sesión 9 — Perfiles

### Perfil de jugador (visible por todos)
- Stats generales: partidos / ganados / empatados / perdidos / goles
- Top 3 equipos más usados (con record G/E/P por equipo)
- Top 3 videojuegos más jugados
- ELO actual + historial partido a partido (línea de tiempo, cambios de posición)
- Lista completa de partidos jugados (fecha, rival, competencia, resultado, equipo, juego)
- Sección head-to-head: lista de rivales con record acumulado contra cada uno

### Vista head-to-head (cualquier jugador A vs jugador B, visible por todos)
- Record total: X victorias - Y empates - Z derrotas
- Goles totales de cada lado
- Lista de todos los partidos entre ambos (fecha, competencia, resultado, equipos, juego)
- Evolución del ELO de ambos a lo largo de sus enfrentamientos

### Perfil de equipo (visible por todos)
- Jugadores que lo usaron y cuántas veces
- Record global con ese equipo (G/E/P), goles a favor y en contra
- Videojuegos en que fue usado

### Ranking general
- Tabla con todos los jugadores del vault, ELO actual y cambio del último partido

### Páginas
- Player/PlayerProfile.razor
- Player/HeadToHead.razor
- Teams/TeamProfile.razor
- Ranking.razor

## Sesión 10 — Pulido y despliegue
- Responsive mobile
- Validaciones y manejo de errores
- Opciones de hosting (Railway, Fly.io, Azure)
