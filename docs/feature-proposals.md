# Futelo — Análisis y Propuesta de Features

> Generado: 2026-05-22

---

## PASO 1 — Estado actual

### Stack tecnológico


| Capa         | Tecnología                                        |
| ------------ | -------------------------------------------------- |
| Frontend     | Blazor 10 WASM, Bootstrap 5, SVG icons             |
| Backend      | ASP.NET Core 10 Web API                            |
| Database     | SQLite (dev) / SQL Server (prod) via EF Core 10    |
| Auth         | ASP.NET Core Identity + JWT (7 días, sin refresh) |
| PWA          | Service Worker + Manifest                          |
| i18n         | Servicio custom ES/EN                              |
| Arquitectura | Controller → Service → Repository → DbContext   |

### Funcionalidades existentes


| Feature                | Estado | Descripción                                                                                 |
| ---------------------- | ------ | -------------------------------------------------------------------------------------------- |
| Autenticación         | ✅     | Register/Login con JWT                                                                       |
| Vaults                 | ✅     | Grupos de amigos, invitación por token de email                                             |
| Temporadas             | ✅     | Lifecycle Draft → Active → Finished                                                        |
| Liga                   | ✅     | Round-robin, fixture auto + manual, standings con tiebreakers                                |
| Copa                   | ✅     | Bracket eliminación, seeding modes, gol visitante, penales                                  |
| SuperCopa              | ✅     | 1v1 entre campeones, penales, H2H                                                            |
| Sistema ELO            | ✅     | Global + por temporada, K variable por competición, multiplicadores por diferencia de goles |
| Rankings               | ✅     | ELO global, ELO temporada, Palmarés, Goleadores, Records, H2H                               |
| Perfil jugador         | ✅     | Historial ELO, partidos recientes, forma                                                     |
| Comparación jugadores | ✅     | Side-by-side stats                                                                           |
| Catálogos             | ✅     | VideoGames y Teams (CRUD)                                                                    |
| PWA                    | ✅     | Offline indicator, install prompt, service worker                                            |
| Temas                  | ✅     | Dark/Light, persistido en localStorage                                                       |
| i18n                   | ✅     | ES/EN                                                                                        |

### Flujo típico del usuario

1. **Registro** → entra al dashboard (lista de Vaults vacía si es nuevo)
2. **Crea un Vault** → comparte invitación por email a amigos → amigos aceptan
3. **Crea una Temporada** → configura Liga/Copa/SuperCopa, asigna jugadores y equipos
4. **Activa la temporada** → se generan fixtures/brackets automáticamente
5. **Registra resultados** → los ELO se actualizan automáticamente
6. **Consulta stats** → rankings, records, H2H durante la temporada
7. **Finaliza la temporada** → SuperCopa determina campeón final
8. **Ciclo nuevo** → nueva temporada, ELO global acumula

### Puntos de fricción

1. **Invitaciones por email**: `VaultInvitation` usa tokens, pero requiere que el email del invitado ya esté registrado. Coordinar esto fuera de la app (WhatsApp) es fricción real.
2. **Dashboard vacío**: El dashboard solo lista Vaults. Un usuario que no entra en días no tiene contexto de "¿qué pasó?" o "¿qué falta?".
3. **Sin notificaciones**: No hay mecanismo de push ni in-app para saber cuándo alguien registró un resultado o cuándo hay un partido pendiente.
4. **Partidos sin contexto social**: Los resultados se registran pero no hay forma de comentar o reaccionar, perdiéndose la "historia" de los partidos entre amigos.
5. **Competición única por tipo**: Solo una Liga, una Copa y una SuperCopa por temporada. No hay partidos amistosos fuera de competición.
6. **Sin estado real-time**: Blazor WASM fetcha datos al cargar. Si un amigo registra un resultado mientras estás en la página, no lo ves hasta que recargas.

### Brechas evidentes

- Sin partidos amistosos / fuera de competición que afecten ELO global
- Sin predicciones previas a los partidos (engagement pre-match)
- Sin resumen visual al final de temporada (exportable/compartible)
- Sin sistema de logros o badges para gamificación
- Sin vista calendario de partidos programados
- Sin formato de competición híbrido (fase de grupos + eliminatoria)
- Sin draft de equipos al inicio de temporada

---

## PASO 2 — Propuesta de nuevas funcionalidades

---

### ~~F1 — Activity Feed en Dashboard~~ ✅

**Problema que resuelve:** El dashboard actual muestra solo una lista de Vaults. Un usuario que vuelve después de un día no sabe qué pasó: ¿quién ganó? ¿hay partidos pendientes? ¿cambió el ranking? La app no engancha en la visita diaria.

**Cómo funciona:** Reemplaza (o complementa) el dashboard con un feed cronológico de eventos recientes del Vault: resultado registrado, nueva temporada activada, ELO que subió/bajó, jugador que alcanzó un récord, próximo partido programado. Los eventos se generan cuando se escriben resultados (ya existe el `PlayedAt` timestamp y `EloHistory`).

**Impacto esperado:** Aumenta la frecuencia de visita diaria. Los usuarios entran "a ver qué pasó", no solo a cargar resultados. Aumenta el sentido de comunidad dentro del Vault.

**Complejidad:** Media. Los datos ya existen (`EloHistory`, `Match.PlayedAt`, `Match.Status`). Se necesita un endpoint `/api/vaults/{id}/feed` que agregue los últimos N eventos ordenados por fecha, y un componente de feed en el cliente.

**Quick win:** No (~1.5-2 sprints).

---

### F2 — Partidos Amistosos (Friendlies)

**Problema que resuelve:** A veces los amigos quieren jugar un partido que no pertenece a ninguna competición, pero sí quieren que afecte el ELO global y quede en el historial. Actualmente eso es imposible: todos los partidos están atados a Liga, Copa o SuperCopa.

**Cómo funciona:** Nueva sección en el Vault ("Amistosos") donde el dueño puede registrar un partido 1v1 sin temporada activa. El partido afecta solo el ELO global (K reducido, ej. K=16). Aparece en el historial de ambos jugadores y en el ranking general.

**Impacto esperado:** Extiende el uso de la app fuera de las temporadas formales. Los vaults siguen activos en el off-season. Incrementa la cantidad de datos ELO y mejora la precisión del ranking.

**Complejidad:** Media. La entidad `Match` ya tiene FK discriminadas; se necesita un nuevo FK `FriendlyId` o una entidad `Friendly` similar a `SuperCup` pero más simple. El `EloCalculator` ya soporta K customizable.

**Quick win:** No (~1.5 sprints).

---

### F3 — Predicciones de Partidos

**Problema que resuelve:** Entre jornada y jornada no hay nada que hacer en la app. Los jugadores no tienen motivo para entrar antes de registrar un resultado. La predicción crea engagement pre-partido y añade una capa de competencia secundaria.

**Cómo funciona:** Antes de que un partido tenga resultado, los jugadores del Vault pueden predecir quién gana (o el marcador exacto). Al registrarse el resultado, se computan los puntos de predicción. Una tabla de "mejores pronosticadores" aparece en la sección de stats del Vault. Predicciones se bloquean automáticamente cuando se registra el resultado.

**Impacto esperado:** Aumenta las visitas durante la semana (no solo cuando se juega). Añade competencia secundaria que engancha incluso a jugadores eliminados en copa.

**Complejidad:** Media. Necesita nueva entidad `Prediction` (matchId, userId, predictedWinnerId, isCorrect, points). Endpoint para crear predicción y leer resultados. UI integrada en la vista de partido pendiente.

**Quick win:** No (~2 sprints incluyendo UI y lógica de puntos).

---

### F4 — Invitación por Link

**Problema que resuelve:** El sistema actual de invitación requiere introducir el email del invitado, que debe estar registrado previamente. En la práctica, los usuarios coordinan por WhatsApp quién tiene cuenta y quién no, lo cual es fricción innecesaria y un obstáculo de onboarding.

**Cómo funciona:** El dueño del Vault genera un link de invitación (`/invite/join?token=xyz`) con expiración configurable (24h/7 días). Cualquiera con el link puede registrarse y entrar al Vault en un solo flujo. Si ya tiene cuenta, simplemente se une. El link es compartible por WhatsApp/Telegram directamente.

**Impacto esperado:** Reduce drásticamente la fricción de onboarding. Es la diferencia entre "invita a tus amigos" (que funciona) vs. "invita a tus amigos que ya se registraron" (que frustra). Feature crítica para crecimiento orgánico.

**Complejidad:** Baja-Media. La entidad `VaultInvitation` ya existe con token; se necesita un flow que no requiera email previo. El endpoint `/invite/accept` ya existe. Principal cambio: generar tokens sin email asociado + página de join para usuarios no registrados.

**Quick win:** Sí (~1 sprint).

---

### F5 — Resumen Visual de Temporada (Season Recap)

**Problema que resuelve:** Cuando termina una temporada no hay un momento de celebración o "recap". La app simplemente muestra la temporada como "Finished". Los jugadores pierden la narrativa completa de lo que pasó y no tienen nada para compartir con orgullo.

**Cómo funciona:** Al marcar una temporada como Finished, se genera automáticamente una página de resumen con: campeón de liga/copa/supercopa, top goleador, mayor subida de ELO, mejores y peores resultados, partidos clave, racha más larga. La página tiene un diseño visual atractivo y un botón "Compartir resumen" que copia un link público (read-only, sin auth requerida).

**Impacto esperado:** Crea un momento memorable al cierre de cada temporada. El link compartible funciona como marketing orgánico — los usuarios lo mandan al grupo de WhatsApp. Aumenta la sensación de logro y el deseo de iniciar la próxima temporada.

**Complejidad:** Media. Los datos ya existen (EloHistory, Match, palmares, etc.). Se necesita un endpoint de agregación `/api/seasons/{id}/recap` + diseño de la página + sistema de links públicos (bypass al `[Authorize]` para el recap).

**Quick win:** No (~2 sprints incluyendo diseño visual y link público).

---

### F6 — Logros y Badges

**Problema que resuelve:** El sistema de ELO y estadísticas es rico pero invisible. Un jugador que hace 10 goles en un partido, o que gana 5 seguidos, no recibe ningún reconocimiento especial. La gamificación aumenta el engagement y el orgullo personal.

**Cómo funciona:** Conjunto de logros desbloqueables por acciones: "Primer título", "Hat-trick de títulos", "Imbatible" (temporada sin perder), "Goleador" (X goles en una temporada), "Remontada épica" (perder 3+ ELO y recuperarlo), "Campeón invicto". Los badges aparecen en el perfil del jugador. Se desbloquean en background al registrar resultados.

**Impacto esperado:** Aumenta la revisión frecuente del perfil propio y de rivales. Crea conversación ("¿viste que J consiguió el badge X?"). Añade metas personales más allá de ganar el torneo.

**Complejidad:** Media. Necesita un motor de reglas (`AchievementEngine`) que evalúa condiciones post-resultado, entidad `PlayerAchievement`, y UI de badges en `PlayerProfile`. Las condiciones usan datos ya disponibles (EloHistory, Match, standings).

**Quick win:** No (~2-3 sprints para motor + 10+ badges + UI).

---

### F7 — Vista Calendario de Partidos

**Problema que resuelve:** El campo `Match.ScheduledDate` existe y los usuarios pueden setearlo, pero no hay ninguna vista que muestre los partidos programados de forma cronológica. Para coordinar cuándo jugar, los amigos vuelven a WhatsApp.

**Cómo funciona:** Nueva pestaña "Calendario" en la temporada que muestra todos los partidos con fecha programada en una vista tipo lista mensual. Los partidos sin fecha van a "Sin programar". El dueño puede editar fechas directamente desde esta vista. Filtros por jugador.

**Impacto esperado:** La app se convierte en la fuente de verdad para cuándo se juega, reduciendo la dependencia de WhatsApp para coordinación. Incrementa el uso de las fechas programadas (actualmente subutilizadas según el código).

**Complejidad:** Baja. Los datos ya existen. Es principalmente un endpoint de consulta y un componente de UI que agrupa partidos por fecha. No requiere nuevos modelos de datos.

**Quick win:** Sí (~0.5-1 sprint).

---

### F8 — Draft de Equipos

**Problema que resuelve:** Actualmente el dueño del Vault asigna equipos a cada jugador manualmente antes de activar la temporada. Esto es una decisión unilateral que puede generar roces. Un draft interactivo distribuye el proceso y añade emoción al inicio de la temporada.

**Cómo funciona:** El dueño activa el modo draft. El sistema ordena los turnos (aleatorio o por ELO inverso para dar ventaja al más débil). Cada jugador, en su turno, elige un equipo de la lista disponible. El draft puede ser asíncrono (cada uno tiene X horas para elegir) o sincrónico. El dueño puede saltar turnos y asignar manualmente si alguien no responde.

**Impacto esperado:** Convierte el setup de temporada en un evento social. Los jugadores participan activamente en la preparación. El "momento del draft" se convierte en parte de la experiencia.

**Complejidad:** Alta. Requiere estado de draft compartido entre usuarios, lógica de turno, timeouts, y UI interactiva. Si es asíncrono, es más simple pero menos emocionante.

**Quick win:** No (~3-4 sprints para versión asíncrona completa).

---

### F9 — Formato Fase de Grupos + Eliminatoria

**Problema que resuelve:** La Liga es round-robin puro y la Copa es eliminatoria pura. Para grupos más grandes o quienes quieren una estructura tipo Champions League, no hay opción intermedia.

**Cómo funciona:** Nueva configuración de competición: "Torneo" con fase de grupos (N grupos de M jugadores, round-robin dentro del grupo) + fase knockout (top K de cada grupo avanzan). Los primeros de cada grupo son sembrados en la eliminatoria. Los standings y ELO aplican por fase.

**Impacto esperado:** Desbloquea vaults más grandes (8+ jugadores) con una experiencia más rica. Los jugadores tienen dos fases para ganar puntos y ELO, lo que reduce la aleatoriedad de un bracket puro.

**Complejidad:** Alta. Requiere nuevas entidades (`Group`, `GroupPlayer`, `GroupMatch`), lógica de asignación de grupos, clasificación cruzada, y combinación con el bracket existente de Cup.

**Quick win:** No (~4-5 sprints).

---

### F10 — Comentarios y Reacciones en Partidos

**Problema que resuelve:** Los amigos quieren "decir algo" sobre un resultado. El 7-0 de ayer, la remontada increíble, el gol decisivo en penales. Actualmente no hay ninguna capa social; los resultados son datos fríos sin narrativa.

**Cómo funciona:** En la vista de un partido jugado, los jugadores del Vault pueden dejar un emoji reaction (👑🔥😤😂) o un comentario de texto corto (máx. 200 chars). Los comentarios aparecen en el historial del partido. En el activity feed (F1) aparecen los comentarios recientes.

**Impacto esperado:** Añade la "historia" a cada partido que actualmente se pierde. Mantiene conversaciones dentro de la app en lugar de WhatsApp. Complementa el activity feed (F1) y hace los perfiles de jugador más ricos.

**Complejidad:** Baja-Media. Nueva entidad `MatchComment` (matchId, userId, content, emoji, createdAt). Endpoints CRUD básicos. UI embebida en la vista de partido. Solo los miembros del Vault pueden comentar.

**Quick win:** Sí (~1 sprint para versión básica).

---

### F11 — Notificaciones In-App

**Problema que resuelve:** Los usuarios no saben cuándo ocurren eventos relevantes a menos que entren activamente a la app. Resultado registrado, invitación aceptada, partido programado próximo, nuevo récord alcanzado — todo es silencioso actualmente.

**Cómo funciona:** Un sistema de notificaciones in-app (bell icon en el navbar) con mensajes como "Nicolás ganó 3-1 a Juan", "Tu próximo partido es mañana", "Te superaron en el ranking". Las notificaciones se generan en el servidor al registrar eventos y se leen al entrar a la app. Opcionalmente, web push notifications para el PWA.

**Impacto esperado:** Aumenta la frecuencia de visita al crear urgencia y contexto. Es el puente entre "la app que uso cuando me acuerdo" y "la app que me llama".

**Complejidad:** Media (in-app) / Alta (push). In-app: nueva entidad `Notification`, generación en services, endpoint de lectura, badge UI. Push: requiere service worker push, VAPID keys, suscripciones. El PWA ya tiene service worker, lo que facilita el push.

**Quick win:** No (in-app ~2 sprints, push ~1 sprint adicional).

---

### ~~F12 — Historial ELO con Gráfico Multi-Temporada~~ ✅

**Problema que resuelve:** `EloHistory` ya existe y el perfil del jugador muestra un gráfico de ELO por temporada. Pero no hay forma de ver la evolución del ELO global a lo largo de todas las temporadas en una sola vista.

**Cómo funciona:** En el perfil del jugador, un gráfico de línea muestra el ELO global a través del tiempo — partido por partido — con anotaciones de las temporadas. Se puede filtrar por competición (Liga/Copa). El eje X es fecha real (no número de partidos).

**Impacto esperado:** Da valor a los datos históricos que ya se están acumulando. Los jugadores "veteranos" tienen incentivo para mirar hacia atrás y comparar épocas. Refuerza el ELO global como métrica significativa.

**Complejidad:** Baja. Los datos ya existen en `EloHistory` con timestamps. Se necesita un endpoint que retorne el histórico global completo de un jugador + un gráfico de línea (puede reusar la librería ya usada en `PlayerProfile`).

**Quick win:** Sí (~0.5-1 sprint).

---

## PASO 3 — Matriz de priorización


| Feature                              | Valor para el usuario (1-5) | Esfuerzo técnico (1-5) | Prioridad sugerida |
| ------------------------------------ | :-------------------------: | :---------------------: | :----------------: |
| F4 — Invitación por Link           |              5              |            2            |      **Alto**      |
| ~~F1 — Activity Feed~~  ✅           |              5              |            3            |      **Alto**      |
| F10 — Comentarios y Reacciones      |              4              |            2            |      **Alto**      |
| F7 — Vista Calendario               |              3              |            1            |      **Alto**      |
| ~~F12 — Historial ELO Multi-Temporada~~ ✅ |         3              |            1            |      **Alto**      |
| F5 — Resumen Visual de Temporada    |              5              |            3            |     **Medio**     |
| F2 — Partidos Amistosos             |              4              |            3            |     **Medio**     |
| F3 — Predicciones de Partidos       |              4              |            3            |     **Medio**     |
| F11 — Notificaciones In-App         |              4              |            4            |     **Medio**     |
| F6 — Logros y Badges                |              3              |            4            |     **Medio**     |
| F8 — Draft de Equipos               |              3              |            4            |      **Bajo**      |
| F9 — Fase de Grupos + Eliminatoria  |              4              |            5            |      **Bajo**      |

---

## PASO 4 — Recomendación Top 3

---

### 🥇 #1 — F4: Invitación por Link

**Por qué esta sobre las demás:** Es la única feature que desbloquea el crecimiento. Hoy el único camino para sumar un amigo a un Vault es que ese amigo ya esté registrado Y que el dueño sepa su email exacto. En la práctica, esto significa que los Vaults tienen los mismos 3-4 personas que se conocen, sin crecer. Un link compartible por WhatsApp en dos clics elimina esa barrera completamente.

**Qué refuerza en el código que es necesaria:** La entidad `VaultInvitation` tiene los campos `Email` y `Token`, pero el código en `InvitationService` valida que el email invitado ya exista como usuario (`UserManager.FindByEmailAsync`). Si no existe, la invitación falla silenciosamente desde la perspectiva del nuevo usuario. El endpoint `/invite/accept` ya maneja el token — el cambio es mínimo.

**Cómo se integra sin romper nada:**

- Añadir un campo `IsLinkBased` (bool) a `VaultInvitation` y `MaxUses` (nullable int)
- El endpoint de accept verifica `IsLinkBased=true` y crea el usuario si no existe (redirect al registro con token pre-filled)
- El endpoint de generación de link en `VaultController` es nuevo pero no modifica los existentes
- El flow de email-token existente queda intacto

---

### ~~🥈 #2 — F1: Activity Feed en Dashboard~~ ✅

**Por qué esta sobre las demás:** El dashboard actual es una lista de Vaults — funcional pero que no crea hábito. La frecuencia de visita de un usuario depende de que "se acuerde" de entrar. Un feed de actividad reciente convierte el dashboard en el destino diario: "¿qué pasó hoy en mi Vault?". Los datos para construirlo ya existen completamente — `EloHistory.CreatedAt`, `Match.PlayedAt`, `Season.StartDate` — solo falta la capa de presentación.

**Qué refuerza en el código:** `EloHistory` registra `EloBefore`, `EloAfter`, `EloChange`, `RankBefore`, `RankAfter` con timestamp de cada partido. `Match` tiene `PlayedAt`. La query para "últimos 10 eventos del Vault ordenados por fecha" es una join entre Match y EloHistory — los índices ya están. El `VaultController` ya tiene `/recent-matches` que retorna los partidos recientes, lo cual es la mitad del feed.

**Cómo se integra sin romper nada:**

- Nuevo endpoint `/api/vaults/{id}/feed` que retorna `IEnumerable<FeedEventDto>` (tipo polimórfico: MatchResult | SeasonActivated | PlayerJoined | RecordBroken)
- El dashboard existente queda como fallback; el feed se añade como sección nueva
- El componente de feed es independiente; no modifica ninguna página existente

---

### 🥉 #3 — F5: Resumen Visual de Temporada

**Por qué esta sobre las demás:** Las temporadas terminan sin celebración. El flujo de "Finish Season" llama a un endpoint que actualiza el status y listo. Pero para el usuario, esa temporada puede haber durado 2-3 meses, con partidos épicos, remontadas y rivalidades. Un recap visual con diseño cuidado convierte ese momento en un evento, y el link público compartible es marketing orgánico gratuito.

**Qué refuerza en el código:** `StatsService` ya computa palmarés, goleadores, rachas (`VaultRecords`), y el endpoint de records ya incluye `TopScoringMatch`. Los datos del recap son literalmente un agregado de stats ya computadas. El `SeasonController` tiene `PUT /seasons/{id}/finish` — el trigger perfecto para generar el recap.

**Cómo se integra sin romper nada:**

- Nuevo endpoint `GET /api/seasons/{id}/recap` (read-only, sin `[Authorize]` para el link público — o con un token de acceso anónimo)
- Se ejecuta solo en temporadas con `Status = Finished`; no toca la lógica de finish existente
- Nueva página `/seasons/{id}/recap` en el cliente con diseño visual destacado
- Botón "Ver Recap" que aparece en `SeasonDetail` cuando `Status = Finished`

---

## Prerequisitos técnicos

Antes de cualquier feature nueva, hay dos deudas técnicas que pueden bloquear la estabilidad en producción:

~~**PT1 — Refresh Tokens:** El JWT expira en 7 días sin renovación automática. Un usuario que no entra por una semana se encuentra con un cierre de sesión inesperado. Con features como notificaciones (F11) o el feed (F1) que invitan a entrar más frecuentemente, este bug se hará más notorio, no menos. Estimado: ~1 sprint.~~ ✅

~~**PT2 — Migraciones EF Core:** El proyecto usa `EnsureCreated` en lugar de `dotnet ef migrations`. Cualquier cambio de schema (necesario para F2, F3, F6, etc.) en producción requeriría drops manuales. Migrar a un flujo de migrations formal antes de deployar es imprescindible. Estimado: ~0.5 sprints.~~ ✅
