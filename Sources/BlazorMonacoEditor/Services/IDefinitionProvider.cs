using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;

namespace BlazorMonacoEditor.Services;

/// <summary>
/// Provides symbol definition locations for Monaco "go to definition" requests.
/// </summary>
public interface IDefinitionProvider
{
    /// <summary>
    /// Gets the language identifier handled by this provider.
    /// </summary>
    Task<string> GetLanguage();

    /// <summary>
    /// Resolves the definition location for the symbol at the requested position.
    /// </summary>
    /// <param name="modelUri">The Monaco text model URI where navigation was requested.</param>
    /// <param name="position">The cursor position (1-based line and column).</param>
    /// <returns>
    /// A <see cref="DefinitionLocation"/> for source navigation, or <c>null</c> when no navigable source location exists.
    /// </returns>
    Task<DefinitionLocation?> ProvideDefinition(MonacoUri modelUri, MonacoPosition position);
}
