using System;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding;

internal static class ExceptionExtensions
{
    public static bool IsJSException(this Exception exception)
    {
        return exception switch
        {
            AggregateException {InnerExceptions: [JSDisconnectedException or JSException]} => true,
            JSDisconnectedException or JSException => true,
            _ => false
        };
    }
}