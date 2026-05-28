using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorMonacoEditor.Scaffolding;

internal sealed class MonacoProviderRegistration : IAsyncDisposable
{
    private readonly IAsyncDisposable providerFacade;
    private readonly IAsyncDisposable jsRegistration;
    private int isDisposed;

    public MonacoProviderRegistration(IAsyncDisposable providerFacade, IAsyncDisposable jsRegistration)
    {
        this.providerFacade = providerFacade ?? throw new ArgumentNullException(nameof(providerFacade));
        this.jsRegistration = jsRegistration ?? throw new ArgumentNullException(nameof(jsRegistration));
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref isDisposed, 1) != 0)
        {
            return;
        }

        try
        {
            await jsRegistration.DisposeJsSafeAsync();
        }
        finally
        {
            await providerFacade.DisposeJsSafeAsync();
        }
    }
}
