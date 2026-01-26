# CLAUDE.md

## Commands

```bash
dotnet build
dotnet format
dotnet test
dotnet pack -c Release
```

## Code Rules

- Use initializers only in constructor bodies, not field declarations (except for static constants and simple static readonly values)
- Use zero-allocation APIs (Span\<T\>, ReadOnlySpan\<T\>, stackalloc, etc.) when available
- net10.0 supports most optimizations natively
- For netstandard2.1: use official NuGet packages (e.g., `System.Memory` for Span\<T\>, `Microsoft.Bcl.HashCode`)
- When no official package exists, write polyfills in `Polyfills/` using C# 14 extension members
- Follow `.editorconfig` naming: `_camelCase` for private fields, `s_camelCase` for static, `IPascalCase` for interfaces
- Seal implementation classes unless designed for inheritance
- Interface-first design for public APIs
- Use readonly for fields that don't change after construction
- Use ArgumentNullException.ThrowIfNull(), ArgumentOutOfRangeException.ThrowIfGreaterThan(), and ObjectDisposedException.ThrowIf() for validation
- For thread-safe classes: use volatile int _disposed with Interlocked for disposal
- For single-threaded classes: use bool _disposed
- Create separate concurrent implementations when thread safety is needed (e.g., ObjectPool vs ConcurrentObjectPool)
- Only use volatile and Interlocked when concurrent access is required

## Performance Rules

- Avoid LINQ in hot paths (use for loops instead)
- Prefer structs for small data types (< 16 bytes)
- Use ArrayPool\<T\> for temporary buffers
- Use stackalloc for small allocations (< 1KB)
- Use FrozenDictionary/FrozenSet for read-only lookups after initialization
- Use ImmutableArray for immutable collections
- Use List\<T\> for mutable collections
- Prefer ValueTask over Task when result is often synchronous

## Testing Rules

- Use xUnit v3
- No mocking frameworks (no Moq)
- Create private test doubles as nested classes within test files
- Name test doubles with "Test" prefix (e.g., TestLogProcessor)
- Make test doubles private sealed nested classes
- Test file names mirror source file names with "Tests" suffix
- Each test file must be self-contained
- Use AAA pattern (Arrange-Act-Assert)
- Test method naming: MethodName_Condition_Expected

## Architecture

- Core library targets: `net10.0` (AOT enabled) and `netstandard2.1`
- Polyfills in `Polyfills/` folder for netstandard2.1 compatibility
- Unity package extends core via `netstandard2.1` dependency
- Godot addon extends core via `net10.0` dependency
