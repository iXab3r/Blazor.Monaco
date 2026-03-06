using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;

namespace BlazorMonacoEditor.Services;

public interface IInlayHintsProvider
{
    Task<string> GetLanguage();

    Task<InlayHintList?> ProvideInlayHints(MonacoUri modelUri, MonacoRange range);
}
