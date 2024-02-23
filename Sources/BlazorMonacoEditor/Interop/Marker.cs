using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;



/// <summary>
/// Represents a marker in the Monaco editor, which could be an error, warning, info, or hint in the code.
/// </summary>
public sealed record Marker
{
    /// <summary>
    /// The owner of the marker. Typically the identifier of the extension or system that generated the marker.
    /// </summary>
    [JsonPropertyName("owner")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Owner { get; set; }

    /// <summary>
    /// The resource (file, URI) the marker is associated with.
    /// </summary>
    [JsonPropertyName("resource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public MonacoUri? Resource { get; set; }

    /// <summary>
    /// The severity of the marker.
    /// </summary>
    [JsonPropertyName("severity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public MarkerSeverity? Severity { get; set; }

    /// <summary>
    /// An optional code for the marker that could be a string or an object with a value and a target URI.
    /// </summary>
    [JsonPropertyName("code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public object? Code { get; set; }

    /// <summary>
    /// The message describing the marker.
    /// </summary>
    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Message { get; set; }

    /// <summary>
    /// An optional source of the marker, indicating where it originated from.
    /// </summary>
    [JsonPropertyName("source")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Source { get; set; }

    /// <summary>
    /// The start line number of the marker.
    /// </summary>
    [JsonPropertyName("startLineNumber")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? StartLineNumber { get; set; }

    /// <summary>
    /// The start column of the marker.
    /// </summary>
    [JsonPropertyName("startColumn")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? StartColumn { get; set; }

    /// <summary>
    /// The end line number of the marker.
    /// </summary>
    [JsonPropertyName("endLineNumber")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? EndLineNumber { get; set; }

    /// <summary>
    /// The end column of the marker.
    /// </summary>
    [JsonPropertyName("endColumn")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? EndColumn { get; set; }

    /// <summary>
    /// An optional model version ID the marker is associated with.
    /// </summary>
    [JsonPropertyName("modelVersionId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? ModelVersionId { get; set; }

    /// <summary>
    /// An optional list of related information items for the marker.
    /// </summary>
    [JsonPropertyName("relatedInformation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<RelatedInformation>? RelatedInformation { get; set; }

    /// <summary>
    /// An optional list of tags associated with the marker.
    /// </summary>
    [JsonPropertyName("tags")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<MarkerTag>? Tags { get; set; }
}