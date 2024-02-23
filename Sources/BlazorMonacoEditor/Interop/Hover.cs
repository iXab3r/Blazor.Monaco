using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Represents additional information for a symbol or word in the Monaco editor, rendered in a tooltip-like widget.
/// </summary>
public class Hover
{
    /// <summary>
    /// Gets or sets the contents of this hover, which provide additional information.
    /// </summary>
    /// <value>
    /// A list of markdown strings that represents the contents of the hover.
    /// </value>
    [JsonPropertyName("contents")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<MarkdownString>? Contents { get; set; }

    /// <summary>
    /// Gets or sets the range to which this hover applies. If not specified, the editor will use the range at the current position or the current position itself.
    /// </summary>
    /// <value>
    /// The range of code to which this hover information applies, or <c>null</c> if the hover applies to the current position.
    /// </value>
    [JsonPropertyName("range")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public MonacoRange? Range { get; set; }
    
    public static readonly Hover Empty = new()
    {
        Contents = ArraySegment<MarkdownString>.Empty
    };
}