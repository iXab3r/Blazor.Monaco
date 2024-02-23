using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Represents the context in which signature help was triggered.
/// </summary>
public sealed record SignatureHelpContext
{
    /// <summary>
    /// Specifies the kind of trigger that caused the signature help to be initiated.
    /// </summary>
    [JsonPropertyName("triggerKind")]
    public SignatureHelpTriggerKind TriggerKind { get; set; }

    /// <summary>
    /// The character that triggered the signature help, if any.
    /// </summary>
    /// <remarks>
    /// This property is optional and may not be present if the signature help was not triggered by a character (e.g., manually invoked).
    /// </remarks>
    [JsonPropertyName("triggerCharacter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? TriggerCharacter { get; set; }

    /// <summary>
    /// Indicates whether the signature help request is a re-trigger rather than an initial trigger.
    /// </summary>
    [JsonPropertyName("isRetrigger")]
    public bool IsRetrigger { get; set; }

    /// <summary>
    /// The active signature help, if any, at the time the signature help was triggered.
    /// </summary>
    /// <remarks>
    /// This property is optional and provides information about the existing signature help, allowing for more contextual responses.
    /// </remarks>
    [JsonPropertyName("activeSignatureHelp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public SignatureHelp? ActiveSignatureHelp { get; set; }
}