namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Specifies the behavior of cursor stops around injected text within the Monaco Editor.
/// </summary>
public enum InjectedTextCursorStops
{
    /// <summary>
    /// The cursor stops on both sides of the injected text.
    /// </summary>
    Both = 0,

    /// <summary>
    /// The cursor stops only on the right side of the injected text.
    /// </summary>
    Right = 1,

    /// <summary>
    /// The cursor stops only on the left side of the injected text.
    /// </summary>
    Left = 2,

    /// <summary>
    /// The cursor does not stop on either side of the injected text, allowing the cursor
    /// to move over the injected text without stopping.
    /// </summary>
    None = 3
}