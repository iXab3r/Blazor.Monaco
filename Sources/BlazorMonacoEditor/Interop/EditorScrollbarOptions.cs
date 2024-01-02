using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record EditorScrollbarOptions
{
    /// <summary>
    /// The size of arrows (if displayed). Defaults to 11.
    /// **NOTE**: This option cannot be updated using `updateOptions()`
    /// </summary>
    [JsonPropertyName("arrowSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? ArrowSize { get; init; }

    /// <summary>
    /// Render vertical scrollbar. Defaults to 'auto'. 'auto' | 'visible' | 'hidden'
    /// </summary>
    [JsonPropertyName("vertical")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Vertical { get; init; }

    /// <summary>
    /// Render horizontal scrollbar. Defaults to 'auto'. 'auto' | 'visible' | 'hidden'
    /// </summary>
    [JsonPropertyName("horizontal")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Horizontal { get; init; }

    /// <summary>
    /// Cast horizontal and vertical shadows when the content is scrolled. Defaults to true.
    /// **NOTE**: This option cannot be updated using `updateOptions()`
    /// </summary>
    [JsonPropertyName("useShadows")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? UseShadows { get; init; }

    /// <summary>
    /// Render arrows at the top and bottom of the vertical scrollbar. Defaults to false.
    /// **NOTE**: This option cannot be updated using `updateOptions()`
    /// </summary>
    [JsonPropertyName("verticalHasArrows")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? VerticalHasArrows { get; init; }

    /// <summary>
    /// Render arrows at the left and right of the horizontal scrollbar. Defaults to false.
    /// **NOTE**: This option cannot be updated using `updateOptions()`
    /// </summary>
    [JsonPropertyName("horizontalHasArrows")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? HorizontalHasArrows { get; init; }

    /// <summary>
    /// Listen to mouse wheel events and react to them by scrolling. Defaults to true.
    /// </summary>
    [JsonPropertyName("handleMouseWheel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? HandleMouseWheel { get; init; }

    /// <summary>
    /// Always consume mouse wheel events (always call preventDefault() and stopPropagation() on the browser events). Defaults to true.
    /// **NOTE**: This option cannot be updated using `updateOptions()`
    /// </summary>
    [JsonPropertyName("alwaysConsumeMouseWheel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? AlwaysConsumeMouseWheel { get; init; }

    /// <summary>
    /// Height in pixels for the horizontal scrollbar. Defaults to 10 (px).
    /// </summary>
    [JsonPropertyName("horizontalScrollbarSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? HorizontalScrollbarSize { get; init; }

    /// <summary>
    /// Width in pixels for the vertical scrollbar. Defaults to 10 (px).
    /// </summary>
    [JsonPropertyName("verticalScrollbarSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? VerticalScrollbarSize { get; init; }

    /// <summary>
    /// Width in pixels for the vertical slider. Defaults to `verticalScrollbarSize`.
    /// **NOTE**: This option cannot be updated using `updateOptions()`
    /// </summary>
    [JsonPropertyName("verticalSliderSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? VerticalSliderSize { get; init; }

    /// <summary>
    /// Height in pixels for the horizontal slider. Defaults to `horizontalScrollbarSize`.
    /// **NOTE**: This option cannot be updated using `updateOptions()`
    /// </summary>
    [JsonPropertyName("horizontalSliderSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? HorizontalSliderSize { get; init; }

    /// <summary>
    /// Scroll gutter clicks move by page vs jump to position. Defaults to false.
    /// </summary>
    [JsonPropertyName("scrollByPage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ScrollByPage { get; init; }

    /// <summary>
    /// When set, the horizontal scrollbar will not increase content height. Defaults to false.
    /// </summary>
    [JsonPropertyName("ignoreHorizontalScrollbarInContentHeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? IgnoreHorizontalScrollbarInContentHeight { get; init; }
}