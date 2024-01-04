using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record MarkdownString
{
    /// <summary>
    /// The markdown string value.
    /// </summary>
    [JsonPropertyName("value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Value { get; init; }

    /// <summary>
    /// Indicates whether this markdown string is trusted. Can be a boolean or MarkdownStringTrustedOptions.
    /// </summary>
    [JsonPropertyName("isTrusted")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? IsTrusted { get; init; }

    /// <summary>
    /// Indicates whether theme icons are supported in the markdown string.
    /// </summary>
    [JsonPropertyName("supportThemeIcons")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? SupportThemeIcons { get; init; }

    /// <summary>
    /// Indicates whether HTML is supported in the markdown string.
    /// </summary>
    [JsonPropertyName("supportHtml")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? SupportHtml { get; init; }

    /// <summary>
    /// The base URI for the markdown string.
    /// </summary>
    [JsonPropertyName("baseUri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public MonacoUri? BaseUri { get; init; }

    /// <summary>
    /// A dictionary mapping href strings to URI components.
    /// </summary>
    [JsonPropertyName("uris")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, MonacoUri>? Uris { get; init; }
}