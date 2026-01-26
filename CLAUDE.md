# CLAUDE.md

## Commands

```bash
dotnet build
dotnet format
dotnet test
dotnet pack -c Release
```

## Code Rules

- Use initializers only in constructor bodies, not field declarations
- Use official NuGet packages for backward compatibility (e.g., `System.Memory`, `Microsoft.Bcl.HashCode`)
- When no official package exists, write polyfills in `Polyfills/` using C# 14 extension members
- Follow `.editorconfig` naming: `_camelCase` for private fields, `s_camelCase` for static, `IPascalCase` for interfaces

## Testing Rules

- Use xUnit v3
- No mocking frameworks (no Moq)
- Create private test doubles as nested classes within test files
- Each test file must be self-contained

## Architecture

- Core library targets: `net10.0` (AOT enabled) and `netstandard2.1`
- Polyfills in `Polyfills/` folder for netstandard2.1 compatibility
- Godot addon extends core via `net10.0` dependency
