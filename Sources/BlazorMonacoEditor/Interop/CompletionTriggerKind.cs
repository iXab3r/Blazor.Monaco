namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Enum for CompletionTriggerKind, defining how a suggest provider was triggered.
/// </summary>
public enum CompletionTriggerKind
{
    /// <summary>
    /// Completion was invoked manually.
    /// </summary>
    Invoke = 0,

    /// <summary>
    /// Completion was triggered by a character.
    /// </summary>
    TriggerCharacter = 1,

    /// <summary>
    /// Completion was re-triggered as the current completion list is incomplete.
    /// </summary>
    TriggerForIncompleteCompletions = 2
}