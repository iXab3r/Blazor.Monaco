using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;

namespace BlazorMonacoEditor.Scaffolding;

/// <summary>
/// Provides hover information for symbols or words in a specific language within the Monaco editor.
/// </summary>
public interface IHoverProvider
{
    /// <summary>
    /// Gets the language identifier for which this hover provider is applicable.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing the language identifier as a <see cref="string"/>.</returns>
    Task<string> GetLanguage();

    /// <summary>
    /// Provides hover information asynchronously for a given position and model URI in the Monaco editor.
    /// </summary>
    /// <param name="modelUri">The URI of the model for which hover information is being requested.</param>
    /// <param name="position">The position within the model for which hover information is being requested.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing the hover information as a <see cref="Hover"/> object, or <c>null</c> if no hover information is available at the given position.</returns>
    Task<Hover?> ProvideHover(MonacoUri modelUri, MonacoPosition position);
}