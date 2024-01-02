using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed class RulerOption
{
    /// <summary>
    /// The column where the ruler will be rendered.
    /// </summary>
    [JsonPropertyName("column")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Column { get; init; }

    /// <summary>
    /// The color of the ruler. Can be null.
    /// </summary>
    [JsonPropertyName("color")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Color { get; init; }
}

public enum RenderLineNumbersType
{
    Off = 0,
    On = 1,
    Relative = 2,
    Interval = 3,
    Custom = 4
}