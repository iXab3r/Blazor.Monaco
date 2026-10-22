# Core architecture

## Interop lifetime

[MonacoInterop.ts](wwwroot/ts/MonacoInterop.ts) reuses a browser-window instance through `window.__eyeAurasMonacoInterop` and exposes it as `window.monacoInterop`. It manages editor and text-model contexts. The C# [MonacoInterop](Services/MonacoInterop.cs) service is registered with scoped lifetime by [AddMonaco](ServiceCollectionExtensions.cs); it is not a process-wide singleton.

Authored TypeScript lives in `wwwroot/ts/`. Webpack emits the browser bundles into `wwwroot/js/`; the project build orchestrates npm and webpack.

## Model identity and synchronization

[MonacoModelUri](Scaffolding/MonacoModelUri.cs) creates an `inmemory` URI using the editor ID, normalized path, model ID, and role. The same logical file can therefore have distinct editor/role contexts. Preserve this isolation instead of treating a filename or Monaco-generated model ID as the application's unique identity.

[TextSyncCoordinator](Scaffolding/TextSyncCoordinator.cs) coordinates model text changes. Keep edit coordinate bases and change provenance intact; see [the regression tests](../BlazorMonacoEditor.Tests/TextSyncCoordinatorFixture.cs) and [model identity tests](../BlazorMonacoEditor.Tests/MonacoModelUriFixture.cs).

## Language providers

Monaco language-provider registration applies to a language and can receive requests from multiple models/editors. The interop forwards the model URI to the .NET provider. Registration returns a disposable anchor; preserve registration/facade ownership and disposal ordering. Language-wide registration does not imply that every .NET provider must have singleton lifetime.
