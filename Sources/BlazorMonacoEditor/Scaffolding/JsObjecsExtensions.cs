using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding;

internal  static class JsObjectsExtensions
{
    public static void DisposeJsSafe<T>(this DotNetObjectReference<T> reference) where T : class
    {
        try
        {
            reference.Dispose();
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException)
        {
            // During disposal ignore such errors because there is a chance that browser context is already disposed
        }
    }
    
    public static void DisposeJsSafe(this IDisposable disposable)
    {
        try
        {
            disposable.Dispose();
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException)
        {
            // During disposal ignore such errors because there is a chance that browser context is already disposed
        }
    }
    
    public static async ValueTask DisposeJsSafeAsync(this IAsyncDisposable disposable)
    {
        try
        {
            await disposable.DisposeAsync();
        }
        catch (Exception e) when (e is JSException or JSDisconnectedException)
        {
            // During disposal ignore such errors because there is a chance that browser context is already disposed
        }
    }
}