using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record SingleEditOperation
{
    /// <summary>
    /// The range to replace. This can be empty to emulate a simple insert.
    /// </summary>
    [JsonPropertyName("range")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Range? Range { get; set; }

    /// <summary>
    /// The text to replace with. This can be null to emulate a simple delete.
    /// </summary>
    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Text { get; set; }

    /// <summary>
    /// This indicates that this operation has "insert" semantics.
    /// i.e., if `range` is collapsed, all markers at the position will be moved.
    /// </summary>
    [JsonPropertyName("forceMoveMarkers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ForceMoveMarkers { get; set; }
}