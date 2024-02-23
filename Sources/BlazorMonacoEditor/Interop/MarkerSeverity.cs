namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Defines the severity levels for markers.
/// </summary>
public enum MarkerSeverity
{
    /// <summary>
    /// Hint - suggests a potential improvement or alternative.
    /// </summary>
    Hint = 1,

    /// <summary>
    /// Information - provides informational messages or feedback.
    /// </summary>
    Info = 2,

    /// <summary>
    /// Warning - indicates a potential issue that could lead to problems.
    /// </summary>
    Warning = 4,

    /// <summary>
    /// Error - signifies a problem that will prevent code from functioning correctly.
    /// </summary>
    Error = 8
}