using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding;

internal sealed class MonacoModel : IAsyncDisposable
{
    public MonacoModel()
    {
        ObjectReference = DotNetObjectReference.Create(this);
    }

    public DotNetObjectReference<MonacoModel> ObjectReference { get; }
    
    public async ValueTask DisposeAsync()
    {
    }
}