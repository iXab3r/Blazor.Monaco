using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public class CompletionItemLabel
{
    /// <summary>
    /// The label of the completion item.
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    /// <summary>
    /// Detail information about the completion item.
    /// </summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    /// <summary>
    /// Description of the completion item.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}