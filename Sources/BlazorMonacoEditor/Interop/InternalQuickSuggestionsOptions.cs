using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record InternalQuickSuggestionsOptions
{
    /// <summary>
    /// Quick suggestions configuration for other contexts.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public QuickSuggestionsValue Other { get; set; }

    /// <summary>
    /// Quick suggestions configuration for comments.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public QuickSuggestionsValue Comments { get; set; }

    /// <summary>
    /// Quick suggestions configuration for strings.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public QuickSuggestionsValue Strings { get; set; }
}