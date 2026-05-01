# Modelo de datos - Futelo

## Entidades de catálogo (globales, CRUD libre)

### VideoGame
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | PK |
| Name | string | Ej: "FIFA 23", "PES 2013", "eFootball 2025" |

### Team
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | PK |
| Name | string | Ej: "Real Madrid", "Boca Juniors" |

---

## Entidades principales

### AppUser (extiende IdentityUser)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| DisplayName | string | Nombre visible en la app |
| EloRating | int | ELO histórico acumulado (arranca en 1500) |

### Vault (Bóveda)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | PK |
| Name | string | Ej: "Los pibes" |
| OwnerId | string | FK → AppUser |
| Players | List\<VaultPlayer\> | Miembros del vault |
| Seasons | List\<Season\> | Temporadas del vault |
| Invitations | List\<VaultInvitation\> | Invitaciones pendientes |

### VaultPlayer (many-to-many Vault ↔ AppUser)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| VaultId | int | FK → Vault |
| PlayerId | string | FK → AppUser |
| JoinedAt | DateTime | Fecha en que se unió al vault |

### VaultInvitation
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | PK |
| VaultId | int | FK → Vault |
| Email | string | Destinatario |
| Token | string | Token único para aceptar |
| Status | enum | Pending, Accepted, Expired |
| CreatedAt | DateTime | |
| ExpiresAt | DateTime | |

### Season (Temporada)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | PK |
| VaultId | int | FK → Vault |
| Name | string | Ej: "Temporada 2025" |
| Year | int | Año |
| Status | enum | Draft, Active, Finished |
| Liga | Liga? | null si no se juega |
| Copa | Copa? | null si no se juega |
| Supercopa | Supercopa? | null si no se juega |
| Players | List\<SeasonPlayer\> | Subconjunto de jugadores del vault que participan |

### SeasonPlayer (subconjunto de VaultPlayer para la temporada)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| SeasonId | int | FK → Season |
| PlayerId | string | FK → AppUser (debe ser VaultPlayer del vault) |
| SeasonElo | int | ELO del jugador en esta temporada (arranca en 1500, se resetea) |

### Liga
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | PK |
| SeasonId | int | FK → Season |
| IsHomeAndAway | bool | Si se juega ida y vuelta |
| Status | enum | NotStarted, Active, Finished |
| Players | List\<LigaPlayer\> | Subconjunto de SeasonPlayer que juegan la Liga |
| Matches | List\<Match\> | Todos los partidos |

> El fixture se genera automáticamente con round-robin al crear la Liga.
> Se puede regenerar (re-sortear) mientras el Status sea `NotStarted` (ningún partido jugado aún).
> Una vez cargado el primer resultado, el fixture queda bloqueado.

### LigaPlayer (subconjunto de SeasonPlayer para la Liga)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| LigaId | int | FK → Liga |
| PlayerId | string | FK → AppUser |
| LigaPosition | int? | Posición final (para seed de Copa) |

### Copa
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | PK |
| SeasonId | int | FK → Season |
| IsHomeAndAway | bool | Si cada ronda es ida y vuelta |
| BracketMode | enum | Seeded, Free |
| Status | enum | NotStarted, Active, Finished |
| Players | List\<CopaPlayer\> | Subconjunto de SeasonPlayer que juegan la Copa |
| Rounds | List\<CopaRound\> | Rondas del torneo |

> **Seeded**: los cruces se arman automáticamente según la posición final en Liga.
> **Free**: el organizador arma los cruces manualmente antes de iniciar la Copa.

### CopaPlayer (subconjunto de SeasonPlayer para la Copa)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| CopaId | int | FK → Copa |
| PlayerId | string | FK → AppUser |

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

> Los participantes de la Supercopa se determinan automáticamente al finalizar Liga y Copa.

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
| VideoGameId | int? | FK → VideoGame (null si no se especifica) |
| HomeTeamId | int? | FK → Team — equipo elegido por el local |
| AwayTeamId | int? | FK → Team — equipo elegido por el visitante |
| LigaId | int? | FK → Liga (nullable) |
| CopaRoundId | int? | FK → CopaRound (nullable) |
| SupercopaId | int? | FK → Supercopa (nullable) |

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
| RankBefore | int | Posición en el ranking antes del partido |
| RankAfter | int | Posición en el ranking después del partido |
| IsSeasonElo | bool | Si es el ELO de temporada o el histórico |
| CreatedAt | DateTime | |

---

## Jerarquía de la aplicación

```
AppUser
  └── puede pertenecer a múltiples Vaults

Vault
  ├── VaultPlayer     (miembros permanentes del grupo)
  ├── VaultInvitation (para sumar nuevos miembros)
  └── Season
        ├── SeasonPlayer  (subconjunto de VaultPlayer)
        ├── Liga
        │     ├── LigaPlayer  (subconjunto de SeasonPlayer)
        │     └── Match[]
        ├── Copa
        │     ├── CopaPlayer  (subconjunto de SeasonPlayer)
        │     ├── CopaRound[]
        │     └── Match[]
        └── Supercopa
              └── Match[] (1 o 2, jugadores auto-determinados)
```

---

## Reglas de negocio

### Liga
- W=3pts, D=1pt, L=0pts
- Desempate: diferencia de goles → goles a favor → head-to-head
- Fixture generado automáticamente con round-robin al crear
- Re-sorteo permitido mientras Status = `NotStarted`

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

> 7 jugadores: pendiente de definir (TBD).

**Modo Seeded** (default si hay Liga en la temporada):
- Los cruces se arman automáticamente según `LigaPosition` en `LigaPlayer`
- Bracket generado automáticamente al finalizar la Liga

**Modo Free**:
- El organizador asigna manualmente los cruces de cada ronda antes de iniciar
- No requiere Liga previa
- Si no hay Liga en la temporada, este es el único modo disponible

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
