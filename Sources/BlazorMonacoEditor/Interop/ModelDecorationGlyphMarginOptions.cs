using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Options for rendering a model decoration in the glyph margin of the editor.
/// </summary>
public sealed record ModelDecorationGlyphMarginOptions
{
    /// <summary>
    /// The position in the glyph margin.
    /// </summary>
    [JsonPropertyName("position")]
    public GlyphMarginLane Position { get; init; }
}