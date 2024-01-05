using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;

namespace BlazorMonacoEditor.Services;

public interface ICompletionProvider
{
    Task<string[]> GetTriggerCharacters();

    Task<string> GetLanguage();
    
    Task<CompletionList?> ProvideCompletionItems(MonacoUri modelUri, CompletionContext completionContext, Position caretPosition, int caretOffset);
}