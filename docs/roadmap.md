# Roadmap de sesiones - Futelo

## Sesión 1 — Base del proyecto ✅
- Crear solución con Server, Client y Shared
- Instalar paquetes NuGet
- Crear documentación base
- Subir repo inicial a GitHub

## Sesión 2 — Dominio y base de datos
- Modelos EF Core (Season, Liga, Copa, Supercopa, Match, etc.)
- AppDbContext con configuraciones Fluent API
- Primera migración y seed de datos de prueba
- Verificar que la DB se crea correctamente

## Sesión 3 — Autenticación
- ASP.NET Core Identity configurado
- Endpoints: POST /auth/register, POST /auth/login
- Generación y validación de JWT
- Servicio de auth en el Client (localStorage para token)
- Páginas: Login.razor, Register.razor

## Sesión 4 — Temporadas
- CRUD de temporadas (crear, ver, editar)
- Agregar/quitar jugadores
- Sistema de invitaciones por email
- Aceptar invitación con token
- Páginas: Dashboard.razor, SeasonDetail.razor, CreateSeason.razor

## Sesión 5 — Liga
- Generación automática de fixture (round-robin)
- Tabla de posiciones
- Cargar resultados de partidos
- Páginas: Liga/LigaView.razor

## Sesión 6 — Copa
- Generación del bracket de eliminación directa
- Vista del bracket (árbol visual)
- Manejo de ida y vuelta (aggregate)
- Avance automático al siguiente ronda
- Páginas: Copa/CopaView.razor

## Sesión 7 — Supercopa
- Lógica para determinar los participantes
- Partido(s) simple(s)
- Página: Supercopa/SupercopaView.razor

## Sesión 8 — Pulido y despliegue
- Responsive mobile
- Validaciones y manejo de errores
- Opciones de hosting (Railway, Fly.io, Azure)
