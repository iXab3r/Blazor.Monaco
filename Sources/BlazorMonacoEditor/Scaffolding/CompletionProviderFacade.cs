using System;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding;

internal sealed class CompletionProviderFacade : IAsyncDisposable, ICompletionProvider
{
    private readonly ICompletionProvider completionProvider;

    public CompletionProviderFacade(ICompletionProvider completionProvider)
    {
        this.completionProvider = completionProvider;
        ObjectReference = DotNetObjectReference.Create(this);
    }
    
    public DotNetObjectReference<CompletionProviderFacade> ObjectReference { get; }

    [JSInvokable]
    public async Task<CompletionList> ProvideCompletionItems(MonacoUri modelUri, CompletionContext completionContext, Position position, int caretOffset)
    {
        Console.WriteLine($"Completion request: {modelUri.ToUri()}, position: {position}, caretOffset: {caretOffset}");
        var result = await completionProvider.ProvideCompletionItems(modelUri, completionContext, position, caretOffset);
        return result;
    }

    public async ValueTask DisposeAsync()
    {
        
    }
}