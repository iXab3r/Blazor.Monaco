using System;
using System.Threading;
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
    private int isDisposed;

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref isDisposed, 1) != 0)
        {
            return;
        }

        try
        {
            await JSObjectReference.InvokeVoidAsync("dispose");
        }
        finally
        {
            await JSObjectReference.DisposeAsync();
        }
    }
}
