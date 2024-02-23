using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record ModelDecorationOverviewRulerOptions : DecorationOptions
{
    /// <summary>
    /// The position in the overview ruler.
    /// </summary>
    [JsonPropertyName("position")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OverviewRulerLane? Position { get; init; }
}