using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record InlayHintList
{
    [JsonPropertyName("hints")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<InlayHint>? Hints { get; init; }

    public static readonly InlayHintList Empty = new()
    {
        Hints = ArraySegment<InlayHint>.Empty
    };
}
