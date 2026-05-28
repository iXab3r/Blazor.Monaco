using System.Collections.Generic;

namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Captures the Monaco runtime objects currently known to the scoped interop instance.
/// </summary>
public sealed record MonacoRuntimeSnapshot
{
    /// <summary>
    /// Active standalone editors keyed by the EyeAuras editor identifier.
    /// </summary>
    public IReadOnlyList<MonacoRuntimeEditorSnapshot> Editors { get; init; } = new List<MonacoRuntimeEditorSnapshot>();

    /// <summary>
    /// Active diff editors keyed by the EyeAuras editor identifier.
    /// </summary>
    public IReadOnlyList<MonacoRuntimeEditorSnapshot> DiffEditors { get; init; } = new List<MonacoRuntimeEditorSnapshot>();

    /// <summary>
    /// Text models tracked by full Monaco URI.
    /// </summary>
    public IReadOnlyList<MonacoRuntimeModelSnapshot> Models { get; init; } = new List<MonacoRuntimeModelSnapshot>();

    /// <summary>
    /// Global action identifiers currently retained by the JavaScript fallback map.
    /// </summary>
    public IReadOnlyList<string> EditorActionIds { get; init; } = new List<string>();
}

/// <summary>
/// Describes an editor and the model URI it is displaying, when available.
/// </summary>
public sealed record MonacoRuntimeEditorSnapshot
{
    /// <summary>
    /// EyeAuras-side editor identifier.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Full Monaco model URI displayed by the editor, or null when no model is attached.
    /// </summary>
    public string? ModelUri { get; init; }
}

/// <summary>
/// Describes a Monaco text model tracked by the scoped interop instance.
/// </summary>
public sealed record MonacoRuntimeModelSnapshot
{
    /// <summary>
    /// Full Monaco model URI.
    /// </summary>
    public string Uri { get; init; } = string.Empty;

    /// <summary>
    /// Monaco language identifier currently assigned to the model.
    /// </summary>
    public string LanguageId { get; init; } = string.Empty;

    /// <summary>
    /// Monaco model version identifier.
    /// </summary>
    public int VersionId { get; init; }
}
