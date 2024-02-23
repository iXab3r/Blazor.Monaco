using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Represents a list of code actions 
/// </summary>
public class CodeActionList
{
    /// <summary>
    /// Gets the list of code actions.
    /// </summary>
    [JsonPropertyName("actions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<CodeAction>? Actions { get; set; }
    
    public static readonly CodeActionList Empty = new()
    {
        Actions = ArraySegment<CodeAction>.Empty
    };
}