using System;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;

namespace BlazorMonacoEditor.Services;

public interface IMonacoInterop
{
    ValueTask<IAsyncDisposable> RegisterCompletionProvider(ICompletionProvider completionProvider);
    
    ValueTask<IAsyncDisposable> RegisterCodeActionProvider(ICodeActionProvider codeActionProvider);

    ValueTask SetModelMarkers(Uri modelUri, string markersOwner, MarkerData[] markers);

    ValueTask SetModelContent(Uri modelUri, string newContent);

    ValueTask SetModelDecorations(MonacoEditorId editorId, ModelDeltaDecoration[] decorations);

    ValueTask ExecuteEdits(MonacoEditorId editorId, string editSource, IdentifiedSingleEditOperation[] operations);
}