# Arquitectura - Futelo

## Stack

| Capa | Tecnología |
|------|-----------|
| Frontend | Blazor 10 WebAssembly |
| Backend | ASP.NET Core 10 Web API |
| Base de datos | PostgreSQL (via Entity Framework Core 10) |
| Auth | ASP.NET Core Identity + JWT (7 días, sin refresh tokens) |
| UI | Bootstrap 5 |
| Almacenamiento cliente | localStorage via IJSRuntime |

## Estructura de proyectos

```
Futelo.slnx
├── Futelo.Server/
│   ├── Controllers/         # Reciben requests HTTP, llaman al servicio, devuelven respuestas
│   ├── Data/                # AppDbContext (EF Core)
│   ├── Models/              # Entidades de dominio (EF Core)
│   ├── Repositories/        # Acceso a datos (wrappean DbContext)
│   └── Services/            # Lógica de negocio (usan Repositories)
│
├── Futelo.Client/
│   ├── Pages/               # Componentes Razor (una página = un archivo)
│   ├── Layout/              # Shell visual (NavMenu, MainLayout)
│   ├── Services/            # Lógica cliente (HTTP calls, auth, estado)
│   └── wwwroot/             # Archivos estáticos
│
└── Futelo.Shared/
    └── DTOs/                # Request/Response objects compartidos entre Server y Client
        └── Auth/            # RegisterRequest, LoginRequest, AuthResponse
```

## Capas del Server

La lógica del servidor sigue tres capas bien separadas:

```
Controller  →  Service  →  Repository  →  DbContext / UserManager
```

| Capa | Responsabilidad |
|------|----------------|
| **Controller** | Recibe el HTTP request, valida el modelo, llama al service, devuelve el HTTP response |
| **Service** | Contiene la lógica de negocio. No sabe nada de HTTP ni de EF Core directamente |
| **Repository** | Encapsula las queries a la base de datos. El Service no toca DbContext directo |

- Los Services e interfaces de Repositories se registran en `Program.cs` via inyección de dependencias.
- Para Auth, `UserManager<AppUser>` de Identity ya actúa como repositorio — no se crea uno extra.

## Convenciones de nombres

| Elemento | Convención | Ejemplo |
|----------|-----------|---------|
| Interfaces | `I` + nombre | `IAuthService`, `IVaultRepository` |
| Implementaciones | nombre sin prefijo | `AuthService`, `VaultRepository` |
| DTOs entrantes | `XxxRequest` | `RegisterRequest` |
| DTOs salientes | `XxxResponse` | `AuthResponse` |
| Archivos/clases | PascalCase en inglés | `AuthController.cs` |

## Comunicación

```
Blazor WASM  ──HTTP/JSON──►  ASP.NET Core API  ──EF Core──►  PostgreSQL
    │                               │
    └── localStorage (JWT token)    └── ASP.NET Core Identity
```
