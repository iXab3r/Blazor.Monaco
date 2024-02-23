using System;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Provides additional information related to a marker.
/// </summary>
public sealed record RelatedInformation
{
    /// <summary>
    /// The resource (file, URI) where the information is related to.
    /// </summary>
    [JsonPropertyName("resource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public MonacoUri? Resource { get; set; }

    /// <summary>
    /// The message that provides more information.
    /// </summary>
    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Message { get; set; }

    /// <summary>
    /// The line number where the related information starts.
    /// </summary>
    [JsonPropertyName("startLineNumber")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? StartLineNumber { get; set; }

    /// <summary>
    /// The column number where the related information starts.
    /// </summary>
    [JsonPropertyName("startColumn")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? StartColumn { get; set; }

    /// <summary>
    /// The line number where the related information ends.
    /// </summary>
    [JsonPropertyName("endLineNumber")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? EndLineNumber { get; set; }

    /// <summary>
    /// The column number where the related information ends.
    /// </summary>
    [JsonPropertyName("endColumn")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? EndColumn { get; set; }
}