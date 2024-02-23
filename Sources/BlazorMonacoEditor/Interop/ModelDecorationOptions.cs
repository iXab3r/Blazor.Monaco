using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Options for a model decoration in Monaco Editor.
/// </summary>
public sealed record ModelDecorationOptions
{
    /// <summary>
    /// Customize the growing behavior of the decoration when typing at the edges of the decoration.
    /// </summary>
    [JsonPropertyName("stickiness")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public TrackedRangeStickiness? Stickiness { get; init; } 

    /// <summary>
    /// CSS class name describing the decoration.
    /// </summary>
    [JsonPropertyName("className")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ClassName { get; init; }

    /// <summary>
    /// Indicates whether the decoration should span across the entire line when it continues onto the next line.
    /// </summary>
    [JsonPropertyName("shouldFillLineOnLineBreak")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShouldFillLineOnLineBreak { get; init; }

    /// <summary>
    /// CSS class name for the decoration block.
    /// </summary>
    [JsonPropertyName("blockClassName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? BlockClassName { get; init; }

    /// <summary>
    /// Indicates if this block should be rendered after the last line.
    /// </summary>
    [JsonPropertyName("blockIsAfterEnd")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? BlockIsAfterEnd { get; init; }

    /// <summary>
    /// Indicates if the decoration block does not collapse.
    /// </summary>
    [JsonPropertyName("blockDoesNotCollapse")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? BlockDoesNotCollapse { get; init; }

    /// <summary>
    /// Padding for the decoration block.
    /// </summary>
    [JsonPropertyName("blockPadding")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<double>? BlockPadding { get; init; }

    /// <summary>
    /// Message to be rendered when hovering over the glyph margin decoration.
    /// </summary>
    [JsonPropertyName("glyphMarginHoverMessage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? GlyphMarginHoverMessage { get; init; }

    /// <summary>
    /// Array of MarkdownString to render as the decoration message.
    /// </summary>
    [JsonPropertyName("hoverMessage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? HoverMessage { get; init; }

    /// <summary>
    /// Should the decoration expand to encompass a whole line.
    /// </summary>
    [JsonPropertyName("isWholeLine")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? IsWholeLine { get; init; }

    /// <summary>
    /// Always render the decoration (even when the range it encompasses is collapsed).
    /// </summary>
    [JsonPropertyName("showIfCollapsed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowIfCollapsed { get; init; }

    /// <summary>
    /// Specifies the stack order of a decoration.
    /// </summary>
    [JsonPropertyName("zIndex")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? ZIndex { get; init; }

    /// <summary>
    /// If set, render this decoration in the overview ruler.
    /// </summary>
    [JsonPropertyName("overviewRuler")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ModelDecorationOverviewRulerOptions? OverviewRuler { get; init; }

    /// <summary>
    /// If set, render this decoration in the minimap.
    /// </summary>
    [JsonPropertyName("minimap")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ModelDecorationMinimapOptions? Minimap { get; init; }

    /// <summary>
    /// If set, the decoration will be rendered in the glyph margin with this CSS class name.
    /// </summary>
    [JsonPropertyName("glyphMarginClassName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? GlyphMarginClassName { get; init; }

    /// <summary>
    /// If set and the decoration has glyphMarginClassName set, render this decoration
    /// with the specified options in the glyph margin.
    /// </summary>
    [JsonPropertyName("glyphMargin")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ModelDecorationGlyphMarginOptions? GlyphMargin { get; init; }

    /// <summary>
    /// If set, the decoration will be rendered in the lines decorations with this CSS class name.
    /// </summary>
    [JsonPropertyName("linesDecorationsClassName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? LinesDecorationsClassName { get; init; }

    /// <summary>
    /// If set, the decoration will be rendered in the lines decorations with this CSS class name, but only for the first line in case of line wrapping.
    /// </summary>
    [JsonPropertyName("firstLineDecorationClassName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? FirstLineDecorationClassName { get; init; }

    /// <summary>
    /// If set, the decoration will be rendered in the margin (covering its full width) with this CSS class name.
    /// </summary>
    [JsonPropertyName("marginClassName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MarginClassName { get; init; }

    /// <summary>
    /// If set, the decoration will be rendered inline with the text with this CSS class name.
    /// </summary>
    [JsonPropertyName("inlineClassName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? InlineClassName { get; init; }

    /// <summary>
    /// If there is an inlineClassName which affects letter spacing.
    /// </summary>
    [JsonPropertyName("inlineClassNameAffectsLetterSpacing")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? InlineClassNameAffectsLetterSpacing { get; init; }

    /// <summary>
    /// If set, the decoration will be rendered before the text with this CSS class name.
    /// </summary>
    [JsonPropertyName("beforeContentClassName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? BeforeContentClassName { get; init; }

    /// <summary>
    /// If set, the decoration will be rendered after the text with this CSS class name.
    /// </summary>
    [JsonPropertyName("afterContentClassName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AfterContentClassName { get; init; }

    /// <summary>
    /// If set, text will be injected in the view after the range.
    /// </summary>
    [JsonPropertyName("after")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public InjectedTextOptions? After { get; init; }

    /// <summary>
    /// If set, text will be injected in the view before the range.
    /// </summary>
    [JsonPropertyName("before")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public InjectedTextOptions? Before { get; init; }
}