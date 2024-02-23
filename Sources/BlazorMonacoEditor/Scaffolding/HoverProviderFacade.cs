using System;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding;

internal sealed class HoverProviderFacade : IAsyncDisposable, IHoverProvider
{
    private readonly IHoverProvider provider;

    public HoverProviderFacade(IHoverProvider provider)
    {
        this.provider = provider;
        ObjectReference = DotNetObjectReference.Create(this);
    }
    
    public DotNetObjectReference<HoverProviderFacade> ObjectReference { get; }

    [JSInvokable]
    public Task<string> GetLanguage()
    {
        return provider.GetLanguage();
    }

    [JSInvokable]
    public async Task<Hover?> ProvideHover(MonacoUri modelUri, MonacoPosition position)
    {
        var result = await provider.ProvideHover(modelUri, position);
        return result;
    }

    public async ValueTask DisposeAsync()
    {
        ObjectReference.Dispose();
    }
}