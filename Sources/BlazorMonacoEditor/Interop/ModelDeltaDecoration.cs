using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record ModelDeltaDecoration
{
    /// <summary>
    /// Range that this decoration covers.
    /// </summary>
    [JsonPropertyName("range")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public MonacoRange? Range { get; init; }

    /// <summary>
    /// Options associated with this decoration.
    /// </summary>
    [JsonPropertyName("options")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ModelDecorationOptions? Options { get; init; }
}