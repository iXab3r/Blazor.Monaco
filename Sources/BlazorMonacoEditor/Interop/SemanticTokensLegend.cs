using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record SemanticTokensLegend
{
    [JsonPropertyName("tokenTypes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? TokenTypes { get; init; }

    [JsonPropertyName("tokenModifiers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? TokenModifiers { get; init; }

    public static readonly SemanticTokensLegend Empty = new()
    {
        TokenTypes = ArraySegment<string>.Empty,
        TokenModifiers = ArraySegment<string>.Empty
    };
}
