namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Represents the vertical lanes in the overview ruler of the editor.
/// </summary>
public enum OverviewRulerLane
{
    /// <summary>
    /// The left lane of the overview ruler.
    /// </summary>
    Left = 1,

    /// <summary>
    /// The center lane of the overview ruler.
    /// </summary>
    Center = 2,

    /// <summary>
    /// The right lane of the overview ruler.
    /// </summary>
    Right = 4,

    /// <summary>
    /// Occupies the full width of the overview ruler.
    /// </summary>
    Full = 7
}