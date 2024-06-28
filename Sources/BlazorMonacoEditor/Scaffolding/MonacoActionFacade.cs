using System;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding;

public sealed record MonacoActionArgs
{
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
        await Anchor.DisposeJsSafeAsync();
        ObjectReference.DisposeJsSafe();
    }
}