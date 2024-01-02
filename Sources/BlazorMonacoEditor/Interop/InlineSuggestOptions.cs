using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record InlineSuggestOptions
{
    /// <summary>
    /// Enable or disable the rendering of automatic inline completions.
    /// </summary>
    [JsonPropertyName("enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Configures the mode.
    /// Use `prefix` to only show ghost text if the text to replace is a prefix of the suggestion text.
    /// Use `subword` to only show ghost text if the replace text is a subword of the suggestion text.
    /// Use `subwordSmart` to only show ghost text if the replace text is a subword of the suggestion text,
    /// but the subword must start after the cursor position. Defaults to `prefix`.
    /// </summary>
    [JsonPropertyName("mode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Mode { get; set; }

    /// <summary>
    /// Configures when the toolbar is shown.
    /// </summary>
    [JsonPropertyName("showToolbar")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ShowToolbar { get; set; }

    /// <summary>
    /// Suppresses suggestions when the suggest widget is open.
    /// </summary>
    [JsonPropertyName("suppressSuggestions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? SuppressSuggestions { get; set; }

    /// <summary>
    /// Does not clear active inline suggestions when the editor loses focus.
    /// </summary>
    [JsonPropertyName("keepOnBlur")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? KeepOnBlur { get; set; }
}