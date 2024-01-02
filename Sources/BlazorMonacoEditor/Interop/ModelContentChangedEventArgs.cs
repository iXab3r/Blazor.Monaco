using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

internal sealed record ModelContentChangedEventArgs
{
    /// <summary>
    /// An array of content changes.
    /// </summary>
    [JsonPropertyName("changes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<ModelContentChange>? Changes { get; set; }

    /// <summary>
    /// The (new) end-of-line character.
    /// </summary>
    [JsonPropertyName("eol")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Eol { get; set; }

    /// <summary>
    /// The new version id the model has transitioned to.
    /// </summary>
    [JsonPropertyName("versionId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int VersionId { get; set; }

    /// <summary>
    /// Flag that indicates that this event was generated while undoing.
    /// </summary>
    [JsonPropertyName("isUndoing")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsUndoing { get; set; }

    /// <summary>
    /// Flag that indicates that this event was generated while redoing.
    /// </summary>
    [JsonPropertyName("isRedoing")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsRedoing { get; set; }

    /// <summary>
    /// Flag that indicates that all decorations were lost with this edit.
    /// The model has been reset to a new value.
    /// </summary>
    [JsonPropertyName("isFlush")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsFlush { get; set; }

    /// <summary>
    /// Flag that indicates that this event describes an eol change.
    /// </summary>
    [JsonPropertyName("isEolChange")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsEolChange { get; set; }
}