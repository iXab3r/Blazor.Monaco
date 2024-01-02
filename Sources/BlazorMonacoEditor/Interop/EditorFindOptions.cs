using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record EditorFindOptions
{
    /// <summary>
    /// Controls whether the cursor should move to find matches while typing.
    /// </summary>
    [JsonPropertyName("cursorMoveOnType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CursorMoveOnType { get; init; }

    /// <summary>
    /// Controls if we seed search string in the Find Widget with editor selection.
    /// Can be 'never', 'always', or 'selection'.
    /// </summary>
    [JsonPropertyName("seedSearchStringFromSelection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SeedSearchStringFromSelection { get; init; }

    /// <summary>
    /// Controls if Find in Selection flag is turned on in the editor.
    /// Can be 'never', 'always', or 'multiline'.
    /// </summary>
    [JsonPropertyName("autoFindInSelection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AutoFindInSelection { get; init; }

    /// <summary>
    /// Add extra space on top of the find widget.
    /// </summary>
    [JsonPropertyName("addExtraSpaceOnTop")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? AddExtraSpaceOnTop { get; init; }

    /// <summary>
    /// Controls whether the search result and diff result automatically restarts from the beginning (or the end) 
    /// when no further matches can be found.
    /// </summary>
    [JsonPropertyName("loop")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Loop { get; init; }
}