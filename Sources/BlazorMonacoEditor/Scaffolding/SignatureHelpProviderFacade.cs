using System;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding;

internal sealed class SignatureHelpProviderFacade : IAsyncDisposable, ISignatureHelpProvider
{
    private readonly ISignatureHelpProvider signatureHelpProvider;

    public SignatureHelpProviderFacade(ISignatureHelpProvider signatureHelpProvider, ILogger log)
    {
        Log = log;
        this.signatureHelpProvider = signatureHelpProvider;
        ObjectReference = DotNetObjectReference.Create(this);
    }

    public ILogger Log { get; }

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
        try
        {
            var result = await signatureHelpProvider.ProvideSignatureHelp(modelUri, signatureHelpContext, position);
            return result;
        }
        catch (Exception e)
        {
            Log.LogError(e, "Failed to get signature help for model {0} @ {1}", modelUri, position);
            return null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        ObjectReference.DisposeJsSafe();
    }
}