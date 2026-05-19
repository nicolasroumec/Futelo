# Feature Ideas — Futelo

Ideas para implementar en el futuro, ordenadas por impacto estimado.

---

## Alto impacto

### Feed de actividad por Vault
Timeline por vault que muestre eventos recientes: resultados cargados, temporadas iniciadas, campeones coronados, nuevos miembros. Genera conversación y hace que los usuarios vuelvan a revisar la app a diario. Implementación: tabla `VaultEvent` con tipo, payload JSON y timestamp; endpoint paginado; sidebar o página dedicada en VaultDetail.

### Sistema de predicciones
Antes de cargar el resultado de un partido, los miembros del vault predicen ganador o marcador exacto. Tabla de puntos por acierto (ej: 3 pts ganador correcto, 5 pts marcador exacto). Crea engagement antes y después de cada partido. Requiere modelo `Prediction` vinculado a `Match` y `AppUser`, resolución automática al registrar resultado.

### Premios de temporada (Season Awards)
Al finalizar una temporada, generar automáticamente awards: MVP (mayor ELO final), Goleador, Mejor defensa (menos goles recibidos), Más mejorado (mayor delta de ELO), Más goleado. Mostrarlos en una página de cierre con badge visual. Todos los datos necesarios ya existen en el modelo.

### Notificaciones por email
Enviar emails para: invitación a vault, inicio de temporada, resultado de partido propio cargado. Integrar con SendGrid o similar. El sistema de invitaciones ya tiene token y email, solo falta el envío real.

---

## Impacto medio

### Sistema de logros / Achievements
Badges desbloqueables por hitos: primera victoria, 10 victorias seguidas, campeón de liga y copa en la misma temporada, haber derrotado al #1, etc. Extender `PlayerTitleEntry` a un sistema configurable con fecha de desbloqueo. Mostrar en el perfil del jugador.

### Programación de partidos (Match Scheduling)
Asignar fecha y hora a partidos pendientes y ver un calendario de próximos partidos por vault o por jugador. No requiere lógica compleja: agregar campo `ScheduledAt` a `Match` y un calendario visual en el cliente.

### Modo Draft de equipos
En lugar de asignar equipos manualmente por admin, implementar un draft snake donde los jugadores eligen en orden (determinado por ELO invertido, ranking anterior, o sorteo). Muy adecuado para el setup de temporada. Requiere un estado intermedio en Season (`Drafting`) y lógica de turnos.

### Comentarios en partidos
Los miembros del vault pueden dejar un comentario al cargar o ver un resultado. Sin threading, solo lista simple de comentarios por partido. Crea vida social alrededor de los resultados.

### Rivalidades detectadas automáticamente
Detectar pares de jugadores con alto volumen de partidos entre sí y resultado parejo. Mostrar en el perfil de cada jugador su "Rival histórico" con stats de esa rivalidad destacadas. El H2H ya existe; es agregar lógica de detección y una sección en el perfil.

### Comparación multi-jugador
Extender el H2H a un panel donde se seleccionan 3-4 jugadores y se ven sus stats lado a lado: ELO, win rate, goles, títulos, racha actual.

---

## Nice to have

### Exportar datos
Exportar tabla de posiciones, resultados y scorers a PDF o CSV para compartir fuera de la app (ej: WhatsApp del grupo).

### Refresh Token
El JWT actual expira a los 30 días sin renovación. Agregar refresh token para mayor seguridad y sesiones más largas sin forzar re-login.

### PWA (Progressive Web App)
Agregar `manifest.json` y service worker para instalar la app en el celular. Blazor WASM tiene soporte nativo para esto.

### Sala de espera de temporada (Season Preview)
Antes de activar una temporada, una página que muestre a los participantes, sus equipos asignados y la configuración de competiciones. Botón "Listo" por jugador para gamificar el inicio de temporada.

### Audit log de acciones
Registrar quién cargó cada resultado y cuándo. Útil para resolver disputas en el grupo. El archivo `docs/auditoria-frontend.md` ya existe como referencia.

---

## Orden sugerido para empezar

1. Feed de actividad — fácil de implementar, alto impacto social
2. Sistema de predicciones — gamificación directa sobre lo que ya hacen
3. Season Awards — cierre épico de temporada, todos los datos ya están
4. Notificaciones por email — el sistema de invitación queda completo
