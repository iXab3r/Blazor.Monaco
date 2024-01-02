using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record QuickSuggestionsOptions
{
    /// <summary>
    /// Configuration for quick suggestions in other contexts. Can be boolean or QuickSuggestionsValue.
    /// </summary>
    [JsonPropertyName("other")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public object? Other { get; set; } // Can be bool or QuickSuggestionsValue

    /// <summary>
    /// Configuration for quick suggestions in comments. Can be boolean or QuickSuggestionsValue.
    /// </summary>
    [JsonPropertyName("comments")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public object? Comments { get; set; } // Can be bool or QuickSuggestionsValue

    /// <summary>
    /// Configuration for quick suggestions in strings. Can be boolean or QuickSuggestionsValue.
    /// </summary>
    [JsonPropertyName("strings")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public object? Strings { get; set; } // Can be bool or QuickSuggestionsValue
}