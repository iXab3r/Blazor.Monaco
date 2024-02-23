using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record ThemeColor
{
    /// <summary>
    /// The identifier for the theme color.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Id { get; init; }
}