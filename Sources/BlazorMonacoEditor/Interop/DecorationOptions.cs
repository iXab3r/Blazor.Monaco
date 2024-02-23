using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public record DecorationOptions
{
    /// <summary>
    /// CSS color to render. Can be in various formats (e.g., RGBA, color registry).
    /// </summary>
    [JsonPropertyName("color")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Color { get; init; }

    /// <summary>
    /// CSS color to render in dark themes.
    /// </summary>
    [JsonPropertyName("darkColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DarkColor { get; init; }
}