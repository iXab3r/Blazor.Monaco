using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record EditorLightbulbOptions
{
    /// <summary>
    /// Enable the lightbulb code action. Defaults to true.
    /// </summary>
    [JsonPropertyName("enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Experimental options for the editor lightbulb.
    /// </summary>
    [JsonPropertyName("experimental")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ExperimentalLightbulbOptions? Experimental { get; set; }
}