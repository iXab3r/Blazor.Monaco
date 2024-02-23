using System;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Scaffolding;

namespace BlazorMonacoEditor.Services;

public interface IMonacoInterop
{
    ValueTask<IAsyncDisposable> RegisterCompletionProvider(ICompletionProvider completionProvider);
    
    ValueTask<IAsyncDisposable> RegisterCodeActionProvider(ICodeActionProvider codeActionProvider);
    
    ValueTask<IAsyncDisposable> RegisterHoverProvider(IHoverProvider hoverProvider);

    ValueTask SetModelMarkers(Uri modelUri, string markersOwner, MarkerData[] markers);

    ValueTask SetModelContent(Uri modelUri, string newContent);

    ValueTask SetModelDecorations(MonacoEditorId editorId, ModelDeltaDecoration[] decorations);

    ValueTask ExecuteEdits(MonacoEditorId editorId, string editSource, IdentifiedSingleEditOperation[] operations);
}