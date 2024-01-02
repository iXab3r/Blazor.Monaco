using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record EditorCommentsOptions
{
    /// <summary>
    /// Insert a space after the line comment token and inside the block comments tokens. Defaults to true.
    /// </summary>
    [JsonPropertyName("insertSpace")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? InsertSpace { get; set; }

    /// <summary>
    /// Ignore empty lines when inserting line comments. Defaults to true.
    /// </summary>
    [JsonPropertyName("ignoreEmptyLines")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? IgnoreEmptyLines { get; set; }
}