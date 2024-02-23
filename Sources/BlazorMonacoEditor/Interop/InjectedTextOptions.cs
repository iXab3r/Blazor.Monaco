using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record InjectedTextOptions
{
    /// <summary>
    /// Sets the text to inject. Must be a single line.
    /// </summary>
    [JsonPropertyName("content")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Content { get; init; }

    /// <summary>
    /// If set, the decoration will be rendered inline with the text with this CSS class name.
    /// </summary>
    [JsonPropertyName("inlineClassName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? InlineClassName { get; init; }

    /// <summary>
    /// If there is an `inlineClassName` which affects letter spacing.
    /// </summary>
    [JsonPropertyName("inlineClassNameAffectsLetterSpacing")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? InlineClassNameAffectsLetterSpacing { get; init; }

    /// <summary>
    /// This field allows attaching data to this injected text.
    /// </summary>
    [JsonPropertyName("attachedData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? AttachedData { get; init; }

    /// <summary>
    /// Configures cursor stops around injected text.
    /// </summary>
    [JsonPropertyName("cursorStops")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InjectedTextCursorStops? CursorStops { get; init; }
}