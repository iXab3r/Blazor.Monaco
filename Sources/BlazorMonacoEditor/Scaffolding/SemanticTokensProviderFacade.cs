using System;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding;

internal sealed class SemanticTokensProviderFacade : IAsyncDisposable, ISemanticTokensProvider
{
    private readonly ISemanticTokensProvider provider;
    private readonly ILogger log;

    public SemanticTokensProviderFacade(
        ISemanticTokensProvider provider,
        ILoggerFactory logFactory)
    {
        this.provider = provider;
        log = logFactory.CreateLogger<SemanticTokensProviderFacade>();
        ObjectReference = DotNetObjectReference.Create(this);
    }

    public DotNetObjectReference<SemanticTokensProviderFacade> ObjectReference { get; }

    [JSInvokable]
    public async Task<string> GetLanguage()
    {
        try
        {
            return await provider.GetLanguage();
        }
        catch (Exception e)
        {
            log.LogError(e, "Failed to get language from {SemanticTokensProvider}", provider);
            return string.Empty;
        }
    }

    [JSInvokable]
    public async Task<SemanticTokensLegend> GetSemanticTokensLegend()
    {
        try
        {
            return await provider.GetSemanticTokensLegend();
        }
        catch (Exception e)
        {
            log.LogError(e, "Failed to get semantic tokens legend from {SemanticTokensProvider}", provider);
            return SemanticTokensLegend.Empty;
        }
    }

    [JSInvokable]
    public async Task<SemanticTokens?> ProvideDocumentSemanticTokens(MonacoUri modelUri, string? lastResultId)
    {
        try
        {
            return await provider.ProvideDocumentSemanticTokens(modelUri, lastResultId);
        }
        catch (Exception e)
        {
            log.LogError(e, "Failed to provide semantic tokens from {SemanticTokensProvider}, modelUri: {MonacoUri}", provider, modelUri);
            return SemanticTokens.Empty;
        }
    }

    [JSInvokable]
    public async Task ReleaseDocumentSemanticTokens(string? resultId)
    {
        try
        {
            await provider.ReleaseDocumentSemanticTokens(resultId);
        }
        catch (Exception e)
        {
            log.LogError(e, "Failed to release semantic tokens from {SemanticTokensProvider}, resultId: {ResultId}", provider, resultId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        ObjectReference.DisposeJsSafe();
    }
}
