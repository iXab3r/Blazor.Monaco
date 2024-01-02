using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record EditorStickyScrollOptions
{
    /// <summary>
    /// Enable the sticky scroll.
    /// </summary>
    [JsonPropertyName("enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Enabled { get; init; }

    /// <summary>
    /// Maximum number of sticky lines to show.
    /// </summary>
    [JsonPropertyName("maxLineCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? MaxLineCount { get; init; }

    /// <summary>
    /// Model to choose for sticky scroll by default.
    /// Can be 'outlineModel', 'foldingProviderModel', or 'indentationModel'.
    /// </summary>
    [JsonPropertyName("defaultModel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? DefaultModel { get; init; }

    /// <summary>
    /// Define whether to scroll sticky scroll with editor horizontal scrollbar.
    /// </summary>
    [JsonPropertyName("scrollWithEditor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ScrollWithEditor { get; init; }
}