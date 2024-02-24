using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding;

internal sealed class MonacoDisposable : IAsyncDisposable
{
    public MonacoDisposable(IJSObjectReference jsObjectReference)
    {
        JSObjectReference = jsObjectReference;
    }
    
    public IJSObjectReference JSObjectReference { get; }

    public async ValueTask DisposeAsync()
    {
        await JSObjectReference.InvokeVoidAsync("dispose");
    }
}