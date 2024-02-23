using System;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record MonacoCommand
{
    /// <summary>
    /// The identifier of the command.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Id { get; set; }

    /// <summary>
    /// The title of the command.
    /// </summary>
    [JsonPropertyName("title")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Title { get; set; }

    /// <summary>
    /// Tooltip for the command, if any.
    /// </summary>
    [JsonPropertyName("tooltip")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Tooltip { get; set; }

    /// <summary>
    /// Arguments for the command, if any.
    /// </summary>
    [JsonPropertyName("arguments")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public object[]? Arguments { get; set; }
}