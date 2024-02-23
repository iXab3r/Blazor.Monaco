using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Represents a parameter of a callable signature. A parameter can have a label and a doc-comment.
/// </summary>
public sealed record ParameterInformation
{
    /// <summary>
    /// The label of this signature. Will be shown in the UI.
    /// </summary>
    /// <remarks>
    /// The label can be a string or a tuple representing the start and end positions of the parameter in the signature label.
    /// For simplicity, it's treated as a string here. Consider a more complex type or separate properties if you need to support the tuple format.
    /// </remarks>
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    /// <summary>
    /// The human-readable doc-comment of this signature. Will be shown in the UI but can be omitted.
    /// </summary>
    /// <remarks>
    /// This can be a plain text string or a Markdown string.
    /// </remarks>
    [JsonPropertyName("documentation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MarkdownString? Documentation { get; set; }
}