using System;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record MonacoUri
{
    /// <summary>
    /// The scheme of the URI, such as 'http' or 'https'.
    /// </summary>
    [JsonPropertyName("scheme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Scheme { get; init; } 

    /// <summary>
    /// The authority part of the URI, such as 'www.example.com'.
    /// Optional.
    /// </summary>
    [JsonPropertyName("authority")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Authority { get; init; }

    /// <summary>
    /// The path part of the URI.
    /// Optional.
    /// </summary>
    [JsonPropertyName("path")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Path { get; init; }

    /// <summary>
    /// The query part of the URI.
    /// Optional.
    /// </summary>
    [JsonPropertyName("query")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Query { get; init; }

    /// <summary>
    /// The fragment part of the URI.
    /// Optional.
    /// </summary>
    [JsonPropertyName("fragment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Fragment { get; init; }

    public Uri ToUri()
    {
        return new Uri($"{Scheme}://{Authority}{Path}");
    }
}