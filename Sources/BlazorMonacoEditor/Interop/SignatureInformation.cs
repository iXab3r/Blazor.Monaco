using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Represents the signature of something callable. A signature can have a label, a doc-comment, and a set of parameters.
/// </summary>
public sealed record SignatureInformation
{
    /// <summary>
    /// The label of this signature. Will be shown in the UI.
    /// </summary>
    [JsonPropertyName("label")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Label { get; set; }
    
    /// <summary>
    /// The human-readable doc-comment of this signature. Will be shown in the UI but can be omitted.
    /// </summary>
    /// <remarks>
    /// This can be a plain text string or a Markdown string.
    /// </remarks>
    [JsonPropertyName("documentation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public MarkdownString? Documentation { get; set; }
    
    /// <summary>
    /// The parameters of this signature.
    /// </summary>
    [JsonPropertyName("parameters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<ParameterInformation>? Parameters { get; set; }
    
    /// <summary>
    /// Index of the active parameter. If provided, this is used in place of `SignatureHelp.activeSignature`.
    /// </summary>
    [JsonPropertyName("activeParameter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int ActiveParameter { get; set; }
}