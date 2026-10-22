# Roslyn architecture

[AddMonacoRoslyn](ServiceCollectionExtensions.cs) registers `IRoslynCompletionProvider` with scoped lifetime. [RoslynCompletionProvider](Services/RoslynCompletionProvider.cs) registers with Monaco interop and maps model URIs to Roslyn workspace/document entries for completion requests. Preserve registration ownership, document routing, and disposal behavior.

[RoslynTextModel](Scaffolding/RoslynTextModel.cs) associates a model with a `Workspace` and `DocumentId`, reads text from the current solution, and applies changes through the workspace. Browser editor/model identity belongs to the [core layer](../BlazorMonacoEditor/README.md), rather than being defined by Roslyn's display path alone.
