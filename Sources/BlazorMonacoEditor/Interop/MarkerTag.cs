namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Defines tags that can be attached to markers.
/// </summary>
public enum MarkerTag
{
    /// <summary>
    /// Indicates that the code is unnecessary and could be removed.
    /// </summary>
    Unnecessary = 1,

    /// <summary>
    /// Indicates that the code is deprecated and should be avoided in new code.
    /// </summary>
    Deprecated = 2
}