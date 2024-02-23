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
    public Task<string[]> GetTriggerCharacters()
    {
        return completionProvider.GetTriggerCharacters();
    }

    [JSInvokable]
    public Task<string> GetLanguage()
    {
        return completionProvider.GetLanguage();
    }

    [JSInvokable]
    public async Task<CompletionList?> ProvideCompletionItems(MonacoUri modelUri, CompletionContext completionContext, MonacoPosition position, int caretOffset)
    {
        var result = await completionProvider.ProvideCompletionItems(modelUri, completionContext, position, caretOffset);
        return result;
    }

    [JSInvokable]
    public async Task<CompletionItem> ResolveCompletionItem(CompletionItem item)
    {
        return await completionProvider.ResolveCompletionItem(item);
    }

    public async ValueTask DisposeAsync()
    {
        ObjectReference.Dispose();
    }
}