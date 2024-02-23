using System;
using System.Threading.Tasks;
using BlazorMonacoEditor.Services;
using Microsoft.CodeAnalysis;

namespace BlazorMonacoEditor.Roslyn.Services;

public interface IRoslynCompletionProvider : ICompletionProvider, IAsyncDisposable
{
    IDisposable AddWorkspace(Workspace workspace);

    Task Register();
}