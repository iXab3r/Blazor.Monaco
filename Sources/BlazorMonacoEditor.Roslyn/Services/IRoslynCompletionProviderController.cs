using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BlazorMonacoEditor.Roslyn.Services;

public interface IRoslynCompletionProviderController
{
    IRoslynCompletionProvider CompletionProvider { get; }
    
    Task Register();
}