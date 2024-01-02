using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record SmartSelectOptions
{
    /// <summary>
    /// Determines if leading and trailing whitespace should be selected.
    /// </summary>
    [JsonPropertyName("selectLeadingAndTrailingWhitespace")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? SelectLeadingAndTrailingWhitespace { get; set; }

    /// <summary>
    /// Determines if subwords should be selected.
    /// </summary>
    [JsonPropertyName("selectSubwords")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? SelectSubwords { get; set; }
}