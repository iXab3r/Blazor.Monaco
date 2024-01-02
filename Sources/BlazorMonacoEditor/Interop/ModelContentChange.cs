using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

internal readonly record struct ModelContentChange
{
    /// <summary>
    /// The range that got replaced.
    /// </summary>
    [JsonPropertyName("range")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Range Range { get; init; }

    /// <summary>
    /// The offset of the range that got replaced.
    /// </summary>
    [JsonPropertyName("rangeOffset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int RangeOffset { get; init; }

    /// <summary>
    /// The length of the range that got replaced.
    /// </summary>
    [JsonPropertyName("rangeLength")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int RangeLength { get; init; }

    /// <summary>
    /// The new text for the range.
    /// </summary>
    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Text { get; init; }
}