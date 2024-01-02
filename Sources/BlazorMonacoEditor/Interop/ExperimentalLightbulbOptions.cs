using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record ExperimentalLightbulbOptions
{
    /// <summary>
    /// Highlight AI code actions with AI icon.
    /// off | onCode | on
    /// </summary>
    [JsonPropertyName("showAiIcon")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ShowAiIcon { get; set; }
}