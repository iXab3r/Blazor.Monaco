using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;

namespace BlazorMonacoEditor.Services;

public interface ICodeActionProvider
{
    Task<string> GetLanguage();
    
    Task<CodeActionList?> ProviderCodeActions(MonacoUri modelUri, MonacoRange range, CodeActionContext codeActionContext);
    
    Task<CodeAction?> ResolveCodeAction(CodeAction codeAction);

    Task ApplyCodeAction(string[] codeActionArguments);
}