using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;

namespace BlazorMonacoEditor.Services;

public interface ISemanticTokensProvider
{
    Task<string> GetLanguage();

    Task<SemanticTokensLegend> GetSemanticTokensLegend();

    Task<SemanticTokens?> ProvideDocumentSemanticTokens(MonacoUri modelUri, string? lastResultId);

    Task ReleaseDocumentSemanticTokens(string? resultId);
}
