using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;

namespace BlazorMonacoEditor.Services;

public interface ICompletionProvider
{
    Task<CompletionList> ProvideCompletionItems(MonacoUri modelUri, Position caretPosition, int caretOffset);
}