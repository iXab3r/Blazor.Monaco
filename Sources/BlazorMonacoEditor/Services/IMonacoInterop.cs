using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Scaffolding;

namespace BlazorMonacoEditor.Services;

public interface IMonacoInterop
{
    ValueTask<IAsyncDisposable> RegisterCompletionProvider(ICompletionProvider completionProvider);
    
    ValueTask<IAsyncDisposable> RegisterCodeActionProvider(ICodeActionProvider codeActionProvider);
    
    ValueTask<IAsyncDisposable> RegisterHoverProvider(IHoverProvider hoverProvider);

    ValueTask<IAsyncDisposable> RegisterSignatureHelpProvider(ISignatureHelpProvider signatureHelpProvider);

    ValueTask SetModelMarkers(Uri modelUri, string markersOwner, IReadOnlyList<MarkerData> markers);

    ValueTask SetModelContent(Uri modelUri, string newContent);

    ValueTask SetModelDecorations(MonacoEditorId editorId, IReadOnlyList<ModelDeltaDecoration> decorations);

    ValueTask ExecuteEdits(MonacoEditorId editorId, string editSource, IReadOnlyList<IdentifiedSingleEditOperation> operations);

    ValueTask Focus(MonacoEditorId editorId);
    
    ValueTask FocusAtPosition(MonacoEditorId editorId, int line, int column);

    ValueTask RunEditorAction(MonacoEditorId editorId, string actionId);
    
    ValueTask<MonacoRange> GetSelection(MonacoEditorId editorId);
    
    ValueTask SetSelection(MonacoEditorId editorId, MonacoRange range);
    
    ValueTask RevealRangeInCenter(MonacoEditorId editorId, MonacoRange range);

    ValueTask InvokeVoidAsync(string methodName, params object[] args);
    
    ValueTask<T> InvokeAsync<T>(string methodName, params object[] args);
}