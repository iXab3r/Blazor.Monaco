using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record ModelDecorationMinimapOptions : DecorationOptions
{
    /// <summary>
    /// The position in the minimap.
    /// </summary>
    [JsonPropertyName("position")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public MinimapPosition? Position { get; init; }
}