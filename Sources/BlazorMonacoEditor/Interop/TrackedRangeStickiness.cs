namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Describes the behavior of decorations when typing/editing near their edges.
/// </summary>
public enum TrackedRangeStickiness
{
    /// <summary>
    /// The decoration range always grows when typing at its edges.
    /// </summary>
    AlwaysGrowsWhenTypingAtEdges = 0,

    /// <summary>
    /// The decoration range never grows when typing at its edges.
    /// </summary>
    NeverGrowsWhenTypingAtEdges = 1,

    /// <summary>
    /// The decoration range grows only when typing before it.
    /// </summary>
    GrowsOnlyWhenTypingBefore = 2,

    /// <summary>
    /// The decoration range grows only when typing after it.
    /// </summary>
    GrowsOnlyWhenTypingAfter = 3
}