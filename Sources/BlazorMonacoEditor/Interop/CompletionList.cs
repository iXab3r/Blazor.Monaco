using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record CompletionList
{
    /// <summary>
    /// Array of completion items.
    /// </summary>
    [JsonPropertyName("suggestions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<CompletionItem>? Suggestions { get; set; }

    /// <summary>
    /// Optional flag to indicate that the list is not complete.
    /// When set to true, further typing should trigger the re-computation of suggestions.
    /// </summary>
    [JsonPropertyName("incomplete")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Incomplete { get; set; }
}