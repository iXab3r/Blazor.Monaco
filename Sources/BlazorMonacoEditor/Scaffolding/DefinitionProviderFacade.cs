using System;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding;

internal sealed class DefinitionProviderFacade : IAsyncDisposable, IDefinitionProvider
{
    private readonly IDefinitionProvider provider;
    private readonly ILogger log;

    public DefinitionProviderFacade(
        IDefinitionProvider provider,
        ILoggerFactory logFactory)
    {
        this.provider = provider;
        log = logFactory.CreateLogger<DefinitionProviderFacade>();
        ObjectReference = DotNetObjectReference.Create(this);
    }

    public DotNetObjectReference<DefinitionProviderFacade> ObjectReference { get; }

    [JSInvokable]
    public async Task<string> GetLanguage()
    {
        try
        {
            return await provider.GetLanguage();
        }
        catch (Exception e)
        {
            log.LogError(e, "Failed to get language from {DefinitionProvider}", provider);
            return string.Empty;
        }
    }

    [JSInvokable]
    public async Task<DefinitionLocation?> ProvideDefinition(MonacoUri modelUri, MonacoPosition position)
    {
        try
        {
            return await provider.ProvideDefinition(modelUri, position);
        }
        catch (Exception e)
        {
            log.LogError(e, "Failed to provide definition from {DefinitionProvider}, modelUri: {MonacoUri}, position: {Position}", provider, modelUri, position);
            return null;
        }
    }

    public ValueTask DisposeAsync()
    {
        ObjectReference.DisposeJsSafe();
        return ValueTask.CompletedTask;
    }
}
