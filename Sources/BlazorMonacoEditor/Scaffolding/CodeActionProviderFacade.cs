using System;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding;

internal sealed class CodeActionProviderFacade : IAsyncDisposable, ICodeActionProvider
{
    private readonly ICodeActionProvider provider;

    public CodeActionProviderFacade(ICodeActionProvider provider)
    {
        this.provider = provider;
        ObjectReference = DotNetObjectReference.Create(this);
    }
    
    public DotNetObjectReference<CodeActionProviderFacade> ObjectReference { get; }

    [JSInvokable]
    public Task<string> GetLanguage()
    {
        return provider.GetLanguage();
    }

    [JSInvokable]
    public async Task<CodeActionList?> ProviderCodeActions(MonacoUri modelUri, MonacoRange range, CodeActionContext codeActionContext)
    {
        var result = await provider.ProviderCodeActions(modelUri, range, codeActionContext);
        return result;
    }

    [JSInvokable]
    public async Task<CodeAction?> ResolveCodeAction(CodeAction codeAction)
    {
        var result = await provider.ResolveCodeAction(codeAction);
        return result;
    }

    [JSInvokable]
    public async Task ApplyCodeAction(string[] codeActionArguments)
    {
        await provider.ApplyCodeAction(codeActionArguments);
    }

    public async ValueTask DisposeAsync()
    {
    }
}