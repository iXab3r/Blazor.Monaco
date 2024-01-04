using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public readonly record struct Position
{
    /// <summary>
    /// Line number (starts at 1).
    /// </summary>
    [JsonPropertyName("lineNumber")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int LineNumber { get; init; }

    /// <summary>
    /// Column (the first character in a line is between column 1 and column 2).
    /// </summary>
    [JsonPropertyName("column")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Column { get; init; }
}