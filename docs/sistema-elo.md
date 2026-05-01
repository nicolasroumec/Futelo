# Sistema ELO - Futelo

> Basado en `Planificación.pdf`

## Concepto base

- Todos los jugadores empiezan con **1500 puntos ELO**
- Por cada partido se ganan/pierden puntos según resultado y diferencia de ELO entre rivales
- Factor K determina cuánto puede subir/bajar un jugador por partido

### Fórmula

```
Puntos ganados = K * (Resultado - Expectativa)

Expectativa = 1 / (1 + 10^((ELO_rival - ELO_propio) / 400))
```

## Factor K por competencia

| Competencia | K base |
|-------------|--------|
| League | 32 |
| Cup | 24 |
| SuperCup | 16 |

## Ajuste por diferencia de goles

| Diferencia | Multiplicador |
|------------|--------------|
| 1 gol | K × 1.0 (normal) |
| 2 goles | K × 1.2 |
| 3+ goles | K × 1.5 |

## Penales en Cup

- El partido se computa como **empate** para el cálculo ELO (ninguno ganó en el juego)
- El ajuste por diferencia de goles no aplica (resultado = empate)
- El **bonus de clasificación se otorga igual** al jugador que avanza de ronda

## Formato del bracket de Cup

El bracket se arma según la posición final en League. Los mejor ubicados tienen ventaja (menos partidos o rivales más débiles).

### 4 jugadores
```
SF: 1 vs 4
SF: 2 vs 3
Final
```

### 5 jugadores
```
Ronda previa: 4 vs 5
SF: 1 vs ganador(4v5)   ← 1ro espera al ganador de la ronda previa
SF: 2 vs 3              ← directos a semis
Final
```

### 6 jugadores
```
Ronda previa: 4 vs 5
Ronda previa: 3 vs 6
SF: 1 vs ganador(4v5)
SF: 2 vs ganador(3v6)
Final
```

### 8 jugadores
```
QF: 1v8, 2v7, 3v6, 4v5
SF y Final
```

> Para otros números de jugadores, los peor ubicados en League juegan rondas previas y los mejor ubicados entran directo a semis.

## Bonus Cup por clasificación

| Ronda | Bonus al que avanza |
|-------|-------------------|
| Ronda previa / Cuartos | +8 pts |
| Semifinal | +12 pts |
| Final (campeón) | +16 pts |

> El bonus aplica sin importar si el pase fue por victoria normal o por penales.
> Los jugadores que entran con **bye** a una ronda no cobran el bonus de la ronda que se saltean.

## Rankings

- **Por temporada:** se resetean al inicio de cada temporada
- **Histórico:** acumula todas las temporadas, mantiene memoria permanente
- `EloHistory` guarda `RankBefore` y `RankAfter` para mostrar cambios de posición

### Visualización al cargar un resultado

Al guardar el resultado de un partido, la UI muestra inmediatamente:

```
✓ Resultado cargado

  Martín   1505 → 1523  (+18)  ↑ al puesto 1
  Vos      1495 → 1477  (-18)  ↓ al puesto 2
```

La página de ranking siempre refleja el estado actual (sin push/WebSockets).  
Cada fila muestra el cambio del último partido jugado (▲ / ▼ / —).

### Historial ELO (perfil de jugador)

- Línea de tiempo partido a partido
- ELO histórico y ELO de temporada visualizados por separado
- Para cada entrada: rival, competencia, resultado, ELO antes/después, cambio de posición

## Adicionales (ideas futuras)

- Predictor de resultados basado en ELO y partidos anteriores
- Decaimiento de puntos por inactividad

## Ejemplo

**Jugadores del mismo nivel (1500 vs 1500):**
- Expectativa: 50% para cada uno
- Si A gana: `32 * (1 - 0.5) = +16 pts` para A, `-16 pts` para B

**Favorito claro (1800 vs 1300):**
- Expectativa del 1800: 94%
- Si el favorito gana: `32 * (1 - 0.94) = +1.92 pts` (gana poco)
- Si el underdog gana: `32 * (1 - 0.06) = +29.76 pts` (gran recompensa)
