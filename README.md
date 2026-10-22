# Blazor.Monaco

Blazor integration for Monaco Editor with a Roslyn completion layer.

- [Core architecture](Sources/BlazorMonacoEditor/README.md): browser interop, editor/model identity, and providers.
- [Roslyn architecture](Sources/BlazorMonacoEditor.Roslyn/README.md): workspace/document routing.
- [Agent guidance](AGENTS.md): focused work and independent review.

## Build and validation

The solution is [Sources/BlazorMonacoEditor.sln](Sources/BlazorMonacoEditor.sln). Project files define the framework and dependency requirements; the core project invokes `npm install` and webpack (`build-dev` for Debug, `build-prod` for Release), so building also requires Node/npm.

From the repository root:

```text
dotnet build Sources/BlazorMonacoEditor.sln
dotnet test Sources/BlazorMonacoEditor.Tests/BlazorMonacoEditor.Tests.csproj
```

Run the test project explicitly: it is not included in the solution. It covers model identity, provider disposal, and text synchronization. Use the applicable playground or consuming host for browser/editor behavior; these commands do not by themselves prove interactive behavior. The core package's `npm test` script is an unimplemented placeholder.
