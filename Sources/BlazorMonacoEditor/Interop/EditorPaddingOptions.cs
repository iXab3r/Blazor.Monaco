using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public class EditorPaddingOptions
{
    /// <summary>
    /// Spacing between top edge of editor and first line.
    /// </summary>
    [JsonPropertyName("top")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? Top { get; set; }

    /// <summary>
    /// Spacing between bottom edge of editor and last line.
    /// </summary>
    [JsonPropertyName("bottom")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? Bottom { get; set; }
}