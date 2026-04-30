# Modelo de datos - Futelo

## Entidades principales

### AppUser (extiende IdentityUser)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| DisplayName | string | Nombre visible en la app |
| EloRating | int | ELO histórico acumulado (arranca en 1500) |

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
| RoundNumber | int | 1=Final, 2=Semis, 4=Cuartos/Previa... |
| Name | string | "Final", "Semifinal", "Ronda Previa" |
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
| WonOnPenalties | string? | FK → AppUser — ganador por penales (null si no hubo) |
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
| SeasonElo | int | ELO del jugador en esta temporada (arranca en 1500, se resetea) |
| LigaPosition | int? | Posición final en Liga (para seed de Copa) |

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

### EloHistory (historial de cambios ELO)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | PK |
| PlayerId | string | FK → AppUser |
| MatchId | int | FK → Match |
| SeasonId | int | FK → Season |
| EloBefore | int | ELO antes del partido |
| EloAfter | int | ELO después del partido |
| EloChange | int | Diferencia (puede ser negativa) |
| IsSeasonElo | bool | Si es el ELO de temporada o el histórico |
| CreatedAt | DateTime | |

---

## Reglas de negocio

### Liga
- W=3pts, D=1pt, L=0pts
- Desempate: diferencia de goles → goles a favor → head-to-head
- Fixture generado automáticamente con round-robin al iniciar

### Copa — Bracket por cantidad de jugadores

**4 jugadores:**
```
SF: 1 vs 4  |  SF: 2 vs 3  →  Final
```

**5 jugadores:**
```
Ronda previa: 4 vs 5
SF: 1 vs ganador(4v5)  |  SF: 2 vs 3  →  Final
```

**6 jugadores:**
```
Ronda previa: 4 vs 5  |  Ronda previa: 3 vs 6
SF: 1 vs ganador(4v5)  |  SF: 2 vs ganador(3v6)  →  Final
```

**8 jugadores:**
```
QF: 1v8, 2v7, 3v6, 4v5  →  SF  →  Final
```

- El seed se determina por la posición final en Liga (`LigaPosition` en `SeasonPlayer`)
- Bracket generado automáticamente al finalizar la Liga
- Si no hay Liga en la temporada, el seed es aleatorio

### Copa — Penales
- Resultado del partido = **empate** para el cálculo ELO
- El ganador por penales se guarda en `WonOnPenalties`
- El bonus de clasificación se otorga igual al que avanza

### Supercopa
- Participantes: campeón Liga vs campeón Copa
- Si el mismo jugador ganó ambos, juega el subcampeón de Copa
- 1 o 2 partidos según `IsHomeAndAway`

### ELO
- **ELO histórico** (`AppUser.EloRating`): acumula todas las temporadas, nunca se resetea
- **ELO de temporada** (`SeasonPlayer.SeasonElo`): arranca en 1500 al inicio de cada temporada
- Cada partido actualiza ambos ELOs simultáneamente
- Todo cambio queda registrado en `EloHistory`

| Competencia | K base |
|-------------|--------|
| Liga | 32 |
| Copa | 24 |
| Supercopa | 16 |

| Diferencia de goles | Multiplicador |
|---------------------|--------------|
| 1 gol | × 1.0 |
| 2 goles | × 1.2 |
| 3+ goles | × 1.5 |

| Ronda Copa | Bonus al que avanza |
|------------|-------------------|
| Ronda previa / QF | +8 pts |
| Semifinal | +12 pts |
| Final | +16 pts |

> Los jugadores con bye no cobran bonus por la ronda que se saltean.
