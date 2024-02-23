namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Specifies the trigger type for code actions.
/// </summary>
public enum CodeActionTriggerType
{
    /// <summary>
    /// Code actions were invoked manually by the user.
    /// </summary>
    Invoke = 1,

    /// <summary>
    /// Code actions were triggered automatically.
    /// </summary>
    Auto = 2
}