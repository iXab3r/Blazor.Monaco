using System;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding;

internal sealed class InlayHintsProviderFacade : IAsyncDisposable, IInlayHintsProvider
{
    private readonly IInlayHintsProvider provider;
    private readonly ILogger log;

    public InlayHintsProviderFacade(
        IInlayHintsProvider provider,
        ILoggerFactory logFactory)
    {
        this.provider = provider;
        log = logFactory.CreateLogger<InlayHintsProviderFacade>();
        ObjectReference = DotNetObjectReference.Create(this);
    }

    public DotNetObjectReference<InlayHintsProviderFacade> ObjectReference { get; }

    [JSInvokable]
    public async Task<string> GetLanguage()
    {
        try
        {
            return await provider.GetLanguage();
        }
        catch (Exception e)
        {
            log.LogError(e, "Failed to get language from {InlayHintsProvider}", provider);
            return string.Empty;
        }
    }

    [JSInvokable]
    public async Task<InlayHintList?> ProvideInlayHints(MonacoUri modelUri, MonacoRange range)
    {
        try
        {
            return await provider.ProvideInlayHints(modelUri, range);
        }
        catch (Exception e)
        {
            log.LogError(e, "Failed to provide inlay hints from {InlayHintsProvider}, modelUri: {MonacoUri}, range: {Range}", provider, modelUri, range);
            return InlayHintList.Empty;
        }
    }

    public ValueTask DisposeAsync()
    {
        ObjectReference.DisposeJsSafe();
        return ValueTask.CompletedTask;
    }
}
