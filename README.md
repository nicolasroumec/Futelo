# Futelo ⚽

App para trackear torneos de fútbol entre amigos. Cada temporada puede tener Liga, Copa y Supercopa.

## Stack

- **Frontend:** Blazor 10 WebAssembly
- **Backend:** ASP.NET Core 10 Web API
- **Base de datos:** SQLite + Entity Framework Core 10
- **Auth:** ASP.NET Core Identity + JWT
- **UI:** Bootstrap 5

## Estructura

```
Futelo.slnx
├── Futelo.Server/   # API + Identity + EF Core
├── Futelo.Client/   # Blazor WASM
└── Futelo.Shared/   # DTOs compartidos
```

## Setup local

### 1. Clonar el repo

```bash
git clone <url-del-repo>
cd Futelo
```

### 2. Configurar secrets

Crear `Futelo.Server/appsettings.Development.json` (no va al repo):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=futelo.db"
  },
  "Jwt": {
    "Key": "tu-clave-secreta-de-minimo-32-caracteres",
    "Issuer": "futelo",
    "Audience": "futelo-users",
    "ExpirationDays": 30
  }
}
```

### 3. Correr migraciones

```bash
cd Futelo.Server
dotnet ef database update
```

### 4. Ejecutar

```bash
# Terminal 1 - API
cd Futelo.Server
dotnet run

# Terminal 2 - Blazor WASM
cd Futelo.Client
dotnet run
```

## Docs

- [`docs/domain.md`](./docs/domain.md) — modelo de dominio, entidades, ciclo de vida, reglas de negocio
- [`docs/features.md`](./docs/features.md) — registro de features y mapa completo de endpoints
