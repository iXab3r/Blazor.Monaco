namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Specifies the kind of trigger that caused the signature help to be initiated.
/// </summary>
public enum SignatureHelpTriggerKind
{
    /// <summary>
    /// Signature help was invoked manually.
    /// </summary>
    Invoke = 1,
    
    /// <summary>
    /// Signature help was triggered by a trigger character.
    /// </summary>
    TriggerCharacter = 2,
    
    /// <summary>
    /// Signature help was triggered by a content change.
    /// </summary>
    ContentChange = 3
}