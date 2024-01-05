using System;
using BlazorMonacoEditor.Services;
using Microsoft.CodeAnalysis;

namespace BlazorMonacoEditor.Roslyn.Services;

public interface IRoslynCompletionProvider : ICompletionProvider
{
    IDisposable AddWorkspace(Workspace workspace);
}