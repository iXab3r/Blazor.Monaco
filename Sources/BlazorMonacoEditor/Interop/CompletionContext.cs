using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public readonly record struct CompletionContext
{
    /// <summary>
    /// How the completion was triggered.
    /// </summary>
    [JsonPropertyName("triggerKind")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public CompletionTriggerKind TriggerKind { get; init; }

    /// <summary>
    /// Character that triggered the completion item provider.
    /// `null` if provider was not triggered by a character.
    /// </summary>
    [JsonPropertyName("triggerCharacter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? TriggerCharacter { get; init; }
}