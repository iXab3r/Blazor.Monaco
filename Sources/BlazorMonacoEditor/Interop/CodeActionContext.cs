using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Contains additional diagnostic information about the context in which a code action is run.
/// </summary>
public sealed record CodeActionContext
{
    /// <summary>
    /// An array of diagnostics.
    /// </summary>
    [JsonPropertyName("markers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<MarkerData>? Markers { get; set; }

    /// <summary>
    /// Requested kind of actions to return. This can be used to specify a specific type of code action to return
    /// if the client is only interested in certain types of actions.
    /// </summary>
    [JsonPropertyName("only")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Only { get; set; }

    /// <summary>
    /// The reason why code actions were requested, such as whether they were triggered automatically or invoked manually.
    /// </summary>
    [JsonPropertyName("trigger")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public CodeActionTriggerType? Trigger { get; set; }
}