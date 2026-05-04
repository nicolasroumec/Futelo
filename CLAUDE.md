# Futelo – Project Guidelines

## Code conventions

- All code in English: classes, methods, variables, file names.
- All Blazor components use the **code-behind pattern**: C# logic goes in a `.razor.cs` partial class file. Never use `@code {}` blocks inside `.razor` files.

## Branch naming

Format: `{number}-{feature-name}` (e.g. `2-authentication`)

## Commits

Commit after each logical chunk, not one big commit at the end.
