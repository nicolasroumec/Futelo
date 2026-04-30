# Modelo de datos - Futelo

## Entidades principales

### AppUser (extiende IdentityUser)
- `DisplayName` — nombre visible en la app

### Season (Temporada)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | PK |
| Name | string | Ej: "Temporada 2025" |
| Year | int | Año |
| Status | enum | Draft, Active, Finished |
| OwnerId | string | FK → AppUser |
| Liga | Liga? | null si no se juega |
| Copa | Copa? | null si no se juega |
| Supercopa | Supercopa? | null si no se juega |
| Players | List\<SeasonPlayer\> | Jugadores de la temporada |
| Invitations | List\<SeasonInvitation\> | Invitaciones pendientes |

### Liga
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | PK |
| SeasonId | int | FK → Season |
| IsHomeAndAway | bool | Si se juega ida y vuelta |
| Status | enum | NotStarted, Active, Finished |
| Matches | List\<Match\> | Todos los partidos |

### Copa
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | PK |
| SeasonId | int | FK → Season |
| IsHomeAndAway | bool | Si cada ronda es ida y vuelta |
| Status | enum | NotStarted, Active, Finished |
| Rounds | List\<CopaRound\> | Rondas del torneo |

### CopaRound
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | PK |
| CopaId | int | FK → Copa |
| RoundNumber | int | 1=Final, 2=Semis, 4=Cuartos... |
| Name | string | "Final", "Semifinal", etc. |
| Matches | List\<Match\> | Partidos de la ronda |

### Supercopa
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | PK |
| SeasonId | int | FK → Season |
| IsHomeAndAway | bool | Si se juega ida y vuelta |
| Player1Id | string? | FK → AppUser (campeón Liga) |
| Player2Id | string? | FK → AppUser (campeón Copa) |
| Status | enum | NotStarted, Active, Finished |
| Matches | List\<Match\> | 1 o 2 partidos |

### Match (Partido)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | PK |
| HomePlayerId | string | FK → AppUser |
| AwayPlayerId | string | FK → AppUser |
| HomeScore | int? | null = no jugado aún |
| AwayScore | int? | null = no jugado aún |
| Status | enum | Pending, Played |
| Leg | int | 1 o 2 (para ida y vuelta) |
| PlayedAt | DateTime? | Fecha del partido |
| LigaId | int? | FK → Liga (nullable) |
| CopaRoundId | int? | FK → CopaRound (nullable) |
| SupercopaId | int? | FK → Supercopa (nullable) |

### SeasonPlayer (many-to-many Season ↔ AppUser)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| SeasonId | int | FK → Season |
| PlayerId | string | FK → AppUser |
| Role | enum | Owner, Player |

### SeasonInvitation
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | PK |
| SeasonId | int | FK → Season |
| Email | string | Destinatario |
| Token | string | Token único para aceptar |
| Status | enum | Pending, Accepted, Expired |
| CreatedAt | DateTime | |
| ExpiresAt | DateTime | |

## Reglas de negocio

- **Liga:** W=3pts, D=1pt, L=0pts. Desempate: diferencia de goles, luego goles a favor.
- **Copa:** ganador por aggregate si IsHomeAndAway. En empate de aggregate, gana el visitante del partido de ida (away goals).
- **Supercopa:** si el mismo jugador ganó Liga y Copa, juega el subcampeón de Copa.
- **Fixture de Liga:** se genera automáticamente con round-robin al iniciar la competencia.
- **Bracket de Copa:** se genera automáticamente (con byes si el número no es potencia de 2).
