using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public class CompletionItemLabel
{
    /// <summary>
    /// The label of the completion item.
    /// </summary>
    [JsonPropertyName("label")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Label { get; set; }

    /// <summary>
    /// Detail information about the completion item.
    /// </summary>
    [JsonPropertyName("detail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Detail { get; set; }

    /// <summary>
    /// Description of the completion item.
    /// </summary>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }
}