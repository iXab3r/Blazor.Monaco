using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public readonly record struct Range
{
    /// <summary>
    /// Line number on which the range starts (starts at 1).
    /// </summary>
    [JsonPropertyName("startLineNumber")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int StartLineNumber { get; init; }

    /// <summary>
    /// Column on which the range starts in line `startLineNumber` (starts at 1).
    /// </summary>
    [JsonPropertyName("startColumn")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int StartColumn { get; init; }

    /// <summary>
    /// Line number on which the range ends.
    /// </summary>
    [JsonPropertyName("endLineNumber")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int EndLineNumber { get; init; }

    /// <summary>
    /// Column on which the range ends in line `endLineNumber`.
    /// </summary>
    [JsonPropertyName("endColumn")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int EndColumn { get; init; }
}