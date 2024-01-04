using System;
using System.Threading.Tasks;

namespace BlazorMonacoEditor.Services;

public interface IMonacoInterop
{
    ValueTask<IAsyncDisposable> RegisterCompletionProvider(string languageId, ICompletionProvider completionProvider);
}