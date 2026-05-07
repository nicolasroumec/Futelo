# Roadmap de sesiones - Futelo

## Patrón de capas (Server)

Cada feature del servidor sigue esta estructura. Leer antes de implementar cualquier sesión.

```
Controller  →  Service  →  Repository  →  FuteloContext
```

| Capa | Hace | No hace |
|------|------|---------|
| **Controller** | Recibe HTTP, valida modelo, llama al service, devuelve respuesta | Lógica de negocio, acceso a DB |
| **Service** | Lógica de negocio, orquesta repositorios, lanza excepciones de dominio | Saber de HTTP, tocar FuteloContext directo |
| **Repository** | Queries a la DB, usa `FindByCondition`/`FindAll` internamente, expone métodos con nombre semántico | Lógica de negocio, `IQueryable` público |

**Reglas:**
- Siempre crear la interfaz antes que la implementación (`IXxxService`, `IXxxRepository`)
- Registrar todo en `Program.cs` via inyección de dependencias (`AddScoped`)
- Los repositorios concretos heredan de `BaseRepository<T>` e implementan su interfaz
- **Excepción:** para Auth no hay repositorio custom — `UserManager<AppUser>` de Identity ya cumple ese rol

---

## Sesión 1 — Base del proyecto ✅
- Crear solución con Server, Client y Shared
- Instalar paquetes NuGet
- Crear documentación base
- Subir repo inicial a GitHub

## Sesión 2 — Dominio y base de datos ✅
- Modelos EF Core (Season, League, Cup, SuperCup, Match, VideoGame, Team, etc.)
- FuteloContext con configuraciones Fluent API
- BaseRepository<T> como base para todos los repositorios
- Primera migración

## Sesión 3 — Autenticación ✅

### Server
- `IAuthService` + `AuthService` (register, login, generación de JWT)
- `AuthController` → POST `/api/auth/register`, POST `/api/auth/login`
- Configurar JWT en `Program.cs` (`AddAuthentication`, `AddJwtBearer`)
- Sin repositorio custom: usar `UserManager<AppUser>` dentro del service

### Client
- `IAuthService` + `AuthService` (HTTP calls, guardar/leer token en localStorage)
- `Login.razor`, `Register.razor`
- Registrar `AuthService` en `Program.cs` del Client

## Sesión 4 — Vault + Catálogos ✅

### Server
- `IVaultRepository` + `VaultRepository : BaseRepository<Vault>`
- `IVaultService` + `VaultService`
- `VaultController` → GET/POST/PUT/DELETE `/api/vaults`
- `IInvitationRepository` + `InvitationRepository`
- `IInvitationService` + `InvitationService`
- Endpoints de invitación: POST `/api/vaults/{id}/invite`, POST `/api/invitations/{token}/accept`
- `ITeamRepository` + `TeamRepository`
- `ITeamService` + `TeamService`
- `TeamController` → GET/POST/PUT/DELETE `/api/teams`
- `IVideoGameRepository` + `VideoGameRepository`
- `IVideoGameService` + `VideoGameService`
- `VideoGameController` → GET/POST/PUT/DELETE `/api/videogames`

### Client
- `VaultService`, `TeamService`, `VideoGameService`
- `Dashboard.razor`, `VaultDetail.razor`, `CreateVault.razor`, `Teams.razor`, `Games.razor`
- CRUD completo de Teams y VideoGames desde el front

## Sesión 5 — Roles en Vault + Invitaciones (client)

### Modelo
- Agregar enum `VaultRole` (Admin, Editor, Viewer)
- Agregar campo `Role` a `VaultPlayer`
- Migración EF Core

### Server
- Actualizar `VaultService` e `InvitationService` para respetar roles
- Al invitar, se puede especificar el rol que tendrá el invitado
- Endpoints que modifican el vault solo accesibles para Admin/Editor según corresponda

### Client
- `InvitationService` (HTTP calls para invitar y aceptar)
- Formulario de invitación dentro del detalle del vault (email + rol)
- Página `AcceptInvitation.razor` → ruta `/invitations/{token}/accept`
- Mostrar rol de cada miembro en el detalle del vault

## Sesión 6 — Temporadas ✅

### Server
- `ISeasonRepository` + `SeasonRepository`
- `ISeasonService` + `SeasonService`
- `SeasonController` → GET `/api/seasons?vaultId=`, GET `/api/seasons/{id}`, POST `/api/seasons`, PUT `/api/seasons/{id}/configure`, PUT `/api/seasons/{id}/activate`
- Lógica: crear temporada, asignar jugadores, habilitar League/Cup/SuperCup, activar temporada (Draft → Active)

### Client
- `ISeasonService` + `SeasonService`
- `CreateSeason.razor`, `SeasonDetail.razor` (configure + Start Season)
- `VaultDetail.razor` actualizado: lista de seasons + botón New Season

## Sesión 7 — League ✅

### Server
- `ILeagueRepository` + `LeagueRepository`
- `ILeagueService` + `LeagueService`
  - Generación de fixture round-robin (soporta impar con "fecha libre")
  - Cálculo de tabla de posiciones
  - Registro de resultado + cálculo de ELO (K=32, multiplicador por diferencia de goles)
- `LeagueController`

### Client
- `LeagueService`
- `League/LeagueView.razor` (fixture + tabla)

## Sesión 8 — Cup

### Server
- `ICupRepository` + `CupRepository`
- `ICupService` + `CupService`
  - Generación de bracket (4, 5, 6, 8 jugadores)
  - Manejo de ida y vuelta + penales
  - Avance automático de ronda
  - ELO (K=24, bonos por ronda)
  - Al terminar: `Cup.ChampionId` + `CupPlayer.CupPosition` (para seed de SuperCup)
- `CupController`

### Client
- `CupService`
- `Cup/CupView.razor` (árbol de bracket)

> **Nota:** agregar `ChampionId` al modelo `Cup` + migración, igual que se hizo en League (sesión 7).

## Sesión 9 — SuperCup

### Server
- `ISuperCupRepository` + `SuperCupRepository`
- `ISuperCupService` + `SuperCupService`
  - Determinar participantes desde `League.ChampionId` y `Cup.ChampionId`
  - Si el mismo jugador ganó ambos → usar subcampeón de Cup (`CupPlayer.CupPosition = 2`)
  - Registro de partido(s), ELO (K=16)
  - Al terminar: `SuperCup.ChampionId`
- `SuperCupController`

### Client
- `SuperCupService`
- `SuperCup/SuperCupView.razor`

> **Nota:** agregar `ChampionId` al modelo `SuperCup` + migración.

## Sesión 10 — Perfiles y estadísticas

### Server
- `IStatsRepository` + `StatsRepository` (queries de solo lectura, no hereda BaseRepository con writes)
- `IStatsService` + `StatsService`
  - Stats de jugador: partidos, W/D/L, goles, top equipos, top juegos
  - Head-to-head entre dos jugadores
  - Perfil de equipo
  - Ranking general del vault (por `SeasonElo` activo y por `EloRating` histórico)
  - **Palmarés**: historial de campeones por temporada (League/Cup/SuperCup) usando `ChampionId` de cada torneo
- `StatsController`

### Client
- `StatsService`
- `Player/PlayerProfile.razor`, `Player/HeadToHead.razor`, `Teams/TeamProfile.razor`
- `Ranking.razor` — ranking de temporada + histórico
- `Palmares.razor` — trofeos ganados por cada jugador a lo largo de todas las temporadas

## Sesión 11 — Pulido y despliegue
- Responsive mobile
- Manejo global de errores (middleware en Server, error boundaries en Client)
- Opciones de hosting (Railway, Fly.io, Azure)
