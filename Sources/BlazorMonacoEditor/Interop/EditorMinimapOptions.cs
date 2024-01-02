using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record EditorMinimapOptions
{
    /// <summary>
    /// Enable the rendering of the minimap. Defaults to true.
    /// </summary>
    [JsonPropertyName("enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Enabled { get; init; }

    /// <summary>
    /// Control the rendering of minimap.
    /// </summary>
    [JsonPropertyName("autohide")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Autohide { get; init; }

    /// <summary>
    /// Control the side of the minimap in editor. Defaults to 'right'.
    /// 'right' | 'left'
    /// </summary>
    [JsonPropertyName("side")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Side { get; init; }

    /// <summary>
    /// Control the minimap rendering mode. Defaults to 'actual'.
    /// 'proportional' | 'fill' | 'fit'
    /// </summary>
    [JsonPropertyName("size")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Size { get; init; }

    /// <summary>
    /// Control the rendering of the minimap slider. Defaults to 'mouseover'.
    /// 'always' | 'mouseover'
    /// </summary>
    [JsonPropertyName("showSlider")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ShowSlider { get; init; }

    /// <summary>
    /// Render the actual text on a line (as opposed to color blocks). Defaults to true.
    /// </summary>
    [JsonPropertyName("renderCharacters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? RenderCharacters { get; init; }

    /// <summary>
    /// Limit the width of the minimap to render at most a certain number of columns. Defaults to 120.
    /// </summary>
    [JsonPropertyName("maxColumn")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? MaxColumn { get; init; }

    /// <summary>
    /// Relative size of the font in the minimap. Defaults to 1.
    /// </summary>
    [JsonPropertyName("scale")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public double? Scale { get; init; }
}