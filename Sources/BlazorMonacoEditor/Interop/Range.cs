using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

internal readonly record struct Range
{
    /// <summary>
    /// Line number on which the range starts (starts at 1).
    /// </summary>
    [JsonPropertyName("startLineNumber")]
    public int StartLineNumber { get; init; }

    /// <summary>
    /// Column on which the range starts in line `startLineNumber` (starts at 1).
    /// </summary>
    [JsonPropertyName("startColumn")]
    public int StartColumn { get; init; }

    /// <summary>
    /// Line number on which the range ends.
    /// </summary>
    [JsonPropertyName("endLineNumber")]
    public int EndLineNumber { get; init; }

    /// <summary>
    /// Column on which the range ends in line `endLineNumber`.
    /// </summary>
    [JsonPropertyName("endColumn")]
    public int EndColumn { get; init; }
}