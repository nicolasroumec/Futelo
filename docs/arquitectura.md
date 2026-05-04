# Arquitectura - Futelo

## Stack

| Capa | Tecnología |
|------|-----------|
| Frontend | Blazor 10 WebAssembly |
| Backend | ASP.NET Core 10 Web API |
| Base de datos | SQL Server (via Entity Framework Core 10) |
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
Blazor WASM  ──HTTP/JSON──►  ASP.NET Core API  ──EF Core──►  SQL Server
    │                               │
    └── localStorage (JWT token)    └── ASP.NET Core Identity
```

---

## Claims del JWT

El token generado en `AuthService.GenerateJwt` contiene:

| Claim | Valor |
|-------|-------|
| `sub` (`ClaimTypes.NameIdentifier`) | `AppUser.Id` (GUID string) |
| `email` | `AppUser.Email` |
| `unique_name` | `AppUser.UserName` |
| `displayName` | `AppUser.DisplayName` |

En los controllers, el userId se extrae así:

```csharp
private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
```

---

## BaseRepository — API

`BaseRepository<T>` expone estos métodos protegidos:

```csharp
// Sin includes (solo tracking off)
FindAll() → IQueryable<T>

// Con includes (usar cuando se necesitan nav properties)
FindAll(Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes) → IQueryable<T>

// Filtro por condición (sin includes)
FindByCondition(Expression<Func<T, bool>> expression) → IQueryable<T>

// CRUD (no llaman SaveChanges — el repositorio concreto lo hace)
Create(T entity)
Update(T entity)   // → EntityState.Modified en todos los campos escalares
Delete(T entity)

// Persistir
SaveChangesAsync() → Task
```

Cuando se necesitan includes con ThenInclude, usar `Context.Set<T>()` directamente (el overload de `FindAll` con includes no soporta bien el chaining de ThenInclude):

```csharp
await Context.Set<Vault>()
    .Include(v => v.Owner)
    .Include(v => v.Players).ThenInclude(p => p.Player)
    .AsNoTrackingWithIdentityResolution()
    .FirstOrDefaultAsync(v => v.Id == id);
```

---

## Convención de excepciones en Controllers

Los services lanzan excepciones tipadas; los controllers las mapean a HTTP:

| Excepción | HTTP | Cuándo usarla |
|-----------|------|---------------|
| `KeyNotFoundException` | 404 Not Found | Recurso no existe (o no pertenece al usuario) |
| `InvalidOperationException` | 400 Bad Request | Regla de negocio violada |
| `UnauthorizedAccessException` | 403 Forbidden | Usuario autenticado pero sin permiso |

> Para "no encontrado" y "no eres miembro", se devuelve 404 en ambos casos para no revelar la existencia del recurso.

---

## Cascade delete en FuteloContext

`DeleteBehavior.Restrict` está aplicado **solo en las FKs hacia `AppUser`** (para impedir borrar un usuario que tiene registros asociados). La relación Vault → VaultPlayer y similares usa el default de EF Core (Cascade para FKs requeridas), por lo que borrar un Vault elimina en cascada sus VaultPlayers e Invitations.
