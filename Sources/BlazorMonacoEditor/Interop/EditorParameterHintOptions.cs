using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public class EditorParameterHintOptions
{
    /// <summary>
    /// Enable parameter hints. Defaults to true.
    /// </summary>
    [JsonPropertyName("enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Enable cycling of parameter hints. Defaults to false.
    /// </summary>
    [JsonPropertyName("cycle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Cycle { get; set; }
}