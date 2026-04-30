# Arquitectura - Futelo

## Stack

| Capa | Tecnología |
|------|-----------|
| Frontend | Blazor 10 WebAssembly |
| Backend | ASP.NET Core 10 Web API |
| Base de datos | SQLite (via Entity Framework Core 10) |
| Auth | ASP.NET Core Identity + JWT |
| UI | Bootstrap 5 (incluido en el template) |
| Almacenamiento cliente | JS Interop directo (localStorage) |

## Estructura de proyectos

```
Futelo.slnx
├── Futelo.Server/       # ASP.NET Core 10 API
│   ├── Controllers/
│   ├── Data/            # DbContext
│   ├── Models/          # Entidades EF Core
│   ├── Services/        # Lógica de negocio
│   └── DTOs/            # (si no van en Shared)
│
├── Futelo.Client/       # Blazor 10 WebAssembly
│   ├── Pages/
│   ├── Layout/
│   ├── Services/        # HTTP clients, auth
│   └── wwwroot/
│
└── Futelo.Shared/       # Compartido entre Server y Client
    └── DTOs/            # Request/Response models
```

## Paquetes NuGet instalados

### Futelo.Server
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 10.0.7
- `Microsoft.EntityFrameworkCore.Sqlite` 10.0.7
- `Microsoft.EntityFrameworkCore.Design` 10.0.7
- `Microsoft.AspNetCore.Authentication.JwtBearer` 10.0.7
- `Microsoft.IdentityModel.Tokens` 8.x

### Futelo.Client
- Sin paquetes extra (Bootstrap viene incluido en el template)
- localStorage via `IJSRuntime` directo

## Comunicación

```
Blazor WASM  ──HTTP/JSON──►  ASP.NET Core API  ──EF Core──►  SQLite
    │                               │
    └── localStorage (JWT token)    └── ASP.NET Core Identity
```
