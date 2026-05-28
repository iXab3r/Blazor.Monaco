using System;
using System.Threading;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding;

public sealed record MonacoActionArgs
{
    public MonacoEditorId EditorId { get; init; }
    
    public MonacoUri ModelUri { get; init; } 
    
    public MonacoPosition CaretPosition { get; init; } 
    
    public int CaretOffset { get; init; }
}

public sealed class MonacoActionFacade : IAsyncDisposable
{
    private MonacoActionFacade(ActionDescriptor descriptor, Func<MonacoActionArgs, Task> actionToExecute)
    {
        ObjectReference = DotNetObjectReference.Create(this);
        Descriptor = descriptor;
        ActionToExecute = actionToExecute;
    }
    
    public DotNetObjectReference<MonacoActionFacade> ObjectReference { get; }
    
    public ActionDescriptor Descriptor { get; init; }
    
    public Func<MonacoActionArgs, Task>  ActionToExecute { get; init; }
    
    private MonacoDisposable Anchor { get; set; }
    private int isDisposed;
    
    public static async Task<MonacoActionFacade> Create(IMonacoInterop interop, MonacoEditorId editorId, ActionDescriptor descriptor, Func<MonacoActionArgs, Task> actionToExecute)
    {
        var instance = new MonacoActionFacade(descriptor, actionToExecute);
        var anchor = await interop.InvokeAsync<IJSObjectReference>("addAction", editorId, descriptor, instance.ObjectReference);
        instance.Anchor = new MonacoDisposable(anchor);
        return instance;
    }
    
    public static async Task<MonacoActionFacade> Create(IMonacoInterop interop, ActionDescriptor descriptor, Func<MonacoActionArgs, Task> actionToExecute)
    {
        var instance = new MonacoActionFacade(descriptor, actionToExecute);
        var anchor = await interop.InvokeAsync<IJSObjectReference>("addEditorAction", descriptor, instance.ObjectReference);
        instance.Anchor = new MonacoDisposable(anchor);
        return instance;
    }

    [JSInvokable]
    public async Task HandleExecuted(MonacoActionArgs args)
    {
        await ActionToExecute.Invoke(args);
    }
    
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref isDisposed, 1) != 0)
        {
            return;
        }

        try
        {
            await Anchor.DisposeJsSafeAsync();
        }
        finally
        {
            ObjectReference.DisposeJsSafe();
        }
    }
}
