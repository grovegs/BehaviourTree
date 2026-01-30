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
- netstandard2.1 includes Span\<T\>/ReadOnlySpan\<T\> natively
- For netstandard2.1: use official NuGet packages only when needed (e.g., `System.Threading.Channels`, `Microsoft.Bcl.TimeProvider`)
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

## Unity/Godot Engine Code Rules

- Never throw C# exceptions (ArgumentNullException, ArgumentException, etc.) in engine implementations
- Use engine-specific error handling instead:
  - Unity: `Debug.LogError()`, `Debug.LogWarning()`, `Debug.Assert()`
  - Godot: `GD.PushError()`, `GD.PushWarning()`, `GD.Assert()`
- Check parameters for null and provide fallback values with engine logging
- Example Unity: `if (settings == null) { Debug.LogError("Settings cannot be null"); settings = CreateInstance<Settings>(); }`
- Example Godot: `if (settings == null) { GD.PushError("Settings cannot be null"); settings = new(); }`

## Unity/Godot Configuration Patterns

**Unity Settings:**

- Use ScriptableObject for project-wide settings (stored in Assets/Settings/)
- Use EditorBuildSettings.AddConfigObject() for runtime access
- Settings provider in Editor/ folder using UI Toolkit (PropertyField, VisualElement)
- No singletons - settings accessed via GetOrCreate()
- No Resources.Load() - EditorBuildSettings.TryGetConfigObject() for runtime
- Factory accepts settings instance for DI-friendly architecture

**Unity C# Configuration:**

- Unity projects must include `csc.rsp` file in `Assets/` directory to enable C# 10 features:

  ```text
  -langversion:10
  -nullable:enable
  ```

- This enables nullable reference types and C# 10 language features (pattern matching, global usings, etc.)
- Required for compatibility with the core library that uses modern C# features
- The `csc.rsp` file is in the Unity project, not the package root (Unity compiles the project, not individual packages)

**Godot Settings:**

- Use Resource for project-wide settings (stored at res://addons/)
- Resource path registered in ProjectSettings for runtime access
- [GlobalClass] partial class inheriting from Resource
- No singletons - settings accessed via GetOrCreate()
- ResourceLoader.Load() for runtime (path from ProjectSettings)
- Factory accepts settings instance for DI-friendly architecture
- Optional: Reference same resource via [Export] in nodes for DI
