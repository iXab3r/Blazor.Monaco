using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Description of an action contribution.
/// </summary>
public sealed record ActionDescriptor
{
    /// <summary>
    /// An unique identifier of the contributed action.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Id { get; set; }

    /// <summary>
    /// A label of the action that will be presented to the user.
    /// </summary>
    [JsonPropertyName("label")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Label { get; set; }

    /// <summary>
    /// Precondition rule.
    /// </summary>
    [JsonPropertyName("precondition")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Precondition { get; set; }

    /// <summary>
    /// An array of keybindings for the action.
    /// </summary>
    [JsonPropertyName("keybindings")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int[]? Keybindings { get; set; }

    /// <summary>
    /// The keybinding rule (condition on top of precondition).
    /// </summary>
    [JsonPropertyName("keybindingContext")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? KeybindingContext { get; set; }

    /// <summary>
    /// Control if the action should show up in the context menu and where.
    /// </summary>
    /// <remarks>
    /// The context menu of the editor has these defaults:
    /// - navigation: The navigation group comes first in all cases.
    /// - 1_modification: This group comes next and contains commands that modify your code.
    /// - 9_cutcopypaste: The last default group with the basic editing commands.
    /// Defaults to null (don't show in context menu).
    /// </remarks>
    [JsonPropertyName("contextMenuGroupId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ContextMenuGroupId { get; set; }

    /// <summary>
    /// Control the order in the context menu group.
    /// </summary>
    [JsonPropertyName("contextMenuOrder")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public double? ContextMenuOrder { get; set; }
}