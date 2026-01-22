# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

GroveGames.BehaviourTree is a lightweight behaviour tree framework developed by Grove Games for .NET and Godot.
The library targets .NET 10.0 for modern features and netstandard2.1 for backward compatibility.

## Project Structure

The repository contains the following project types:

- **Core Library** (`src/GroveGames.BehaviourTree/`): The main .NET library targeting both `net10.0` and `netstandard2.1`
- **Godot Integration** (`src/GroveGames.BehaviourTree.Godot/`): Godot-specific implementations and plugin

## Development Commands

### Building

```bash
dotnet build                           # Build all projects
dotnet build -c Release               # Release build
```

### Testing

```bash
dotnet test                           # Run all tests
dotnet test tests/GroveGames.BehaviourTree.Tests/        # Core library tests
```

### Formatting

```bash
dotnet format                         # Format all code according to .editorconfig
dotnet format --verify-no-changes    # Check if code is properly formatted (CI/CD)
dotnet format whitespace             # Format whitespace only
dotnet format style                  # Apply code style fixes
```

### Packaging

```bash
dotnet pack -c Release                # Create NuGet packages
```

## Code Style & Formatting

The project uses automated formatting via GitHub Actions and comprehensive configuration files:

- **EditorConfig** (`.editorconfig`): Core C# coding standards, 4-space indentation, naming conventions (private fields: `_camelCase`, static: `s_camelCase`, interfaces: `IPascalCase`)
- **VS Code Settings** (`.vscode/settings.json`): C# formatting and save actions

### Target Frameworks & Features

- **Multi-targeting**: `net10.0` (with AOT support) and `netstandard2.1` (for Unity/Godot compatibility)
- **Nullable Reference Types**: Enabled across the project
- **AOT Compatibility**: The `net10.0` target includes AOT analyzers and trimming support
- **Polyfills via Extension Members**: Custom polyfills using C# 14 extension members in `Polyfills/` folder provide backward compatibility for netstandard2.1

## Testing Framework

- **Test Framework**: xUnit v3
- **Test Configuration**: Uses `xunit.runner.json` for xUnit configuration

### Testing Conventions

- **No mocking frameworks**: Do not use Moq or other mocking libraries
- **Private test classes**: Each test file contains its own private test doubles as nested classes
- **Self-contained tests**: Each test file should be self-contained with all required test implementations

Example pattern:
```csharp
public class ServiceTests
{
    [Fact]
    public void Method_ShouldDoSomething()
    {
        var testDependency = new TestDependency();
        var service = new Service(testDependency);
        // ...
    }

    private sealed class TestDependency : IDependency
    {
        public int CallCount { get; private set; }

        public void DoSomething()
        {
            CallCount++;
        }
    }
}
```

## Build Configuration

Key build configurations:

- **Multi-targeting**: Projects support both modern .NET and legacy .NET Standard
- **AOT Support**: Native AOT compilation enabled for `net10.0` target
- **Documentation**: XML documentation generation enabled for all projects
- **Package Properties**: Centralized in `Directory.Build.props`

## SDK Version

The project targets .NET 10.0 SDK (see `global.json`). When working with this codebase, ensure you have .NET 10.0 SDK installed.

## GitHub Workflows

The project uses reusable workflows from `grovegs/workflows`:

- **Tests** (`tests.yml`): Runs on pushes/PRs to main/develop branches
- **Format** (`format.yml`): Validates code formatting
- **Release** (`release.yml`): Manual workflow for creating releases and publishing packages

## Development Sandbox

The `sandbox/` directory contains sample applications for testing and development:

- **ConsoleApplication**: Basic .NET console app for testing core functionality
- **DotnetBenchmark**: Performance benchmarking application
- **GodotApplication**: Godot project with the addon for testing Godot integration

### Godot Sandbox Setup

The Godot sandbox project uses a symlink to reference the Godot addon locally:

- `addons/GroveGames.BehaviourTree` → symlink to `src/GroveGames.BehaviourTree.Godot/addons/GroveGames.BehaviourTree`

## Key Dependencies

- **Microsoft.SourceLink.GitHub**: For source linking in packages

Note: Custom polyfill extensions in the `System` namespace provide backward compatibility for netstandard2.1.
