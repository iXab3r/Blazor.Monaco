namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Specifies the position in the minimap to render the decoration.
/// </summary>
public enum MinimapPosition
{
    /// <summary>
    /// Render the decoration inline within the minimap.
    /// </summary>
    Inline = 1,

    /// <summary>
    /// Render the decoration in the gutter area of the minimap.
    /// </summary>
    Gutter = 2
}