using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record InlayHint
{
    [JsonPropertyName("position")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public MonacoPosition Position { get; init; }

    [JsonPropertyName("label")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Label { get; init; }

    [JsonPropertyName("kind")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InlayHintKind? Kind { get; init; }

    [JsonPropertyName("tooltip")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MarkdownString? Tooltip { get; init; }

    [JsonPropertyName("paddingLeft")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? PaddingLeft { get; init; }

    [JsonPropertyName("paddingRight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? PaddingRight { get; init; }
}
