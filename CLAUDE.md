# Futelo – Project Guidelines

See `docs/domain.md` for the domain model and `docs/features.md` for feature status.

## Stack

Blazor 10 WASM (`Futelo.Client`) + ASP.NET Core 10 (`Futelo.Server`) + PostgreSQL via EF Core (Npgsql). Shared DTOs/enums live in `Futelo.Shared`. In production the Server also hosts the Client WASM as static assets (single-origin).

## Architecture layers (Server)

`Controller` → `IService` / `Service` → `IRepository` / `Repository` → EF Core `DbContext`

All services use primary-constructor DI. Never inject `ILogger` unless logging is actually needed.

## Code conventions

- All code in English: classes, methods, variables, file names.
- Blazor components use the **code-behind pattern**: logic in `.razor.cs` partial class. Never use `@code {}` blocks.
- `CanEdit` on competition responses = current user is vault owner.
- `DateOnly?` for `<input type="date">` bindings in Blazor 10 (not `string` or `DateTime?`).

## Branch / commits

- Branch format: `{number}-{feature-name}` (e.g. `15-features`)
- Commit after each logical chunk. The user commits manually — only suggest the message.
