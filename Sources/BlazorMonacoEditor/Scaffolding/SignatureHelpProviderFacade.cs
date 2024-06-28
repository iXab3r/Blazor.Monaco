using System;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding;

internal sealed class SignatureHelpProviderFacade : IAsyncDisposable, ISignatureHelpProvider
{
    private readonly ISignatureHelpProvider signatureHelpProvider;

    public SignatureHelpProviderFacade(ISignatureHelpProvider signatureHelpProvider)
    {
        this.signatureHelpProvider = signatureHelpProvider;
        ObjectReference = DotNetObjectReference.Create(this);
    }
    
    public DotNetObjectReference<SignatureHelpProviderFacade> ObjectReference { get; }

    [JSInvokable]
    public Task<string[]> GetTriggerCharacters()
    {
        return signatureHelpProvider.GetTriggerCharacters();
    }

    [JSInvokable]
    public Task<string[]> GetRetriggerCharacters()
    {
        return signatureHelpProvider.GetRetriggerCharacters();
    }

    [JSInvokable]
    public Task<string> GetLanguage()
    {
        return signatureHelpProvider.GetLanguage();
    }

    [JSInvokable]
    public async Task<SignatureHelp?> ProvideSignatureHelp(MonacoUri modelUri, SignatureHelpContext signatureHelpContext, MonacoPosition position)
    {
        var result = await signatureHelpProvider.ProvideSignatureHelp(modelUri, signatureHelpContext, position);
        return result;
    }

    public async ValueTask DisposeAsync()
    {
        ObjectReference.DisposeJsSafe();
    }
}