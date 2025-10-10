using System;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding;

internal sealed class CompletionProviderFacade : IAsyncDisposable, ICompletionProvider
{
    private readonly ICompletionProvider completionProvider;
    private readonly ILogger log;

    public CompletionProviderFacade(
        ICompletionProvider completionProvider,
        ILoggerFactory logFactory)
    {
        this.completionProvider = completionProvider;
        ObjectReference = DotNetObjectReference.Create(this);
        log = logFactory.CreateLogger<CompletionProviderFacade>();
    }

    public DotNetObjectReference<CompletionProviderFacade> ObjectReference { get; }

    [JSInvokable]
    public async Task<string[]> GetTriggerCharacters()
    {
        try
        {
            return await completionProvider.GetTriggerCharacters();
        }
        catch (Exception e)
        {
            log.LogError(e, "Failed to get language from {CompletionProvider}", completionProvider);
            return Array.Empty<string>();
        }
    }

    [JSInvokable]
    public async Task<string> GetLanguage()
    {
        try
        {
            return await completionProvider.GetLanguage();
        }
        catch (Exception e)
        {
            log.LogError(e, "Failed to get language from {CompletionProvider}", completionProvider);
            return string.Empty;
        }
    }

    [JSInvokable]
    public async Task<CompletionList?> ProvideCompletionItems(MonacoUri modelUri, CompletionContext completionContext, MonacoPosition position, int caretOffset)
    {
        try
        {
            var result = await completionProvider.ProvideCompletionItems(modelUri, completionContext, position, caretOffset);
            return result;
        }
        catch (Exception e)
        {
            log.LogError(e, "Failed to get completion list from {CompletionProvider}, modelUri: {MonacoUri}, caretOffset: {CaretOffset}, position: {MonacoPosition}", completionProvider, modelUri, caretOffset, position);
            return CompletionList.Empty;
        }
    }

    [JSInvokable]
    public async Task<CompletionItem> ResolveCompletionItem(CompletionItem item)
    {
        try
        {
            return await completionProvider.ResolveCompletionItem(item);
        }
        catch (Exception e)
        {
            log.LogError(e, "Failed to resolve completion item list from {CompletionProvider}", completionProvider);
            return item;
        }
    }

    public async ValueTask DisposeAsync()
    {
        ObjectReference.DisposeJsSafe();
    }
}