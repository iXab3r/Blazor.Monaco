using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;

namespace BlazorMonacoEditor.Services;

public interface ISignatureHelpProvider
{
    Task<string> GetLanguage();
    
    Task<string[]> GetTriggerCharacters();

    Task<string[]> GetRetriggerCharacters();

    Task<SignatureHelp?> ProvideSignatureHelp(MonacoUri modelUri, SignatureHelpContext signatureHelpContext, MonacoPosition position);
}