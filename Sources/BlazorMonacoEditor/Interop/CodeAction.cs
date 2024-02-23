using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Represents a code action.
/// </summary>
public sealed record CodeAction
{
    /// <summary>
    /// Gets or sets the title of the code action.
    /// </summary>
    [JsonPropertyName("title")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets an optional command associated with the code action.
    /// </summary>
    [JsonPropertyName("command")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public MonacoCommand? Command { get; set; }

    /// <summary>
    /// Gets or sets an optional array of diagnostics related to the code action.
    /// </summary>
    [JsonPropertyName("diagnostics")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<MarkerData>? Diagnostics { get; set; }

    /// <summary>
    /// Gets or sets an optional kind of the code action.
    /// </summary>
    [JsonPropertyName("kind")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Kind { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this action is a preferred fix.
    /// </summary>
    [JsonPropertyName("isPreferred")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? IsPreferred { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this action is AI-powered.
    /// </summary>
    [JsonPropertyName("isAI")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? IsAI { get; set; }

    /// <summary>
    /// Gets or sets an optional reason why the code action is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Disabled { get; set; }
}