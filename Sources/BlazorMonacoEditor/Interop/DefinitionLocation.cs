using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Represents a single "go to definition" target in Monaco.
/// </summary>
public sealed record DefinitionLocation
{
    /// <summary>
    /// Target model URI serialized as a string.
    /// </summary>
    [JsonPropertyName("uri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Uri { get; init; }

    /// <summary>
    /// Target range in the destination model.
    /// </summary>
    [JsonPropertyName("range")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public MonacoRange? Range { get; init; }
}
