using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record EditorHoverOptions
{
    /// <summary>
    /// Enable the hover. Defaults to true.
    /// </summary>
    [JsonPropertyName("enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Delay for showing the hover. Defaults to 300.
    /// </summary>
    [JsonPropertyName("delay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? Delay { get; set; }

    /// <summary>
    /// Is the hover sticky such that it can be clicked and its contents selected? Defaults to true.
    /// </summary>
    [JsonPropertyName("sticky")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Sticky { get; set; }

    /// <summary>
    /// Controls how long the hover is visible after you hovered out of it. Requires sticky setting to be true.
    /// </summary>
    [JsonPropertyName("hidingDelay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? HidingDelay { get; set; }

    /// <summary>
    /// Should the hover be shown above the line if possible? Defaults to false.
    /// </summary>
    [JsonPropertyName("above")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Above { get; set; }
}