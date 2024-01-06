using System;
using System.Threading.Tasks;
using BlazorMonacoEditor.Services;

namespace BlazorMonacoEditor.Roslyn.Services;

internal sealed class RoslynCompletionProviderController : IRoslynCompletionProviderController, IAsyncDisposable
{
    private readonly IMonacoInterop monacoInterop;
    private IAsyncDisposable? registrationAnchor;

    public RoslynCompletionProviderController(IMonacoInterop monacoInterop, IRoslynCompletionProvider roslynCompletionProvider)
    {
        this.monacoInterop = monacoInterop;
        this.CompletionProvider = roslynCompletionProvider;
    }

    public IRoslynCompletionProvider CompletionProvider { get; }

    public async Task Register()
    {
        if (registrationAnchor != null)
        {
            return;
        }

        registrationAnchor = await monacoInterop.RegisterCompletionProvider(CompletionProvider);
    }

    public async ValueTask DisposeAsync()
    {
        var anchor = registrationAnchor;
        if (anchor != null)
        {
            await anchor.DisposeAsync();
        }
    }
}