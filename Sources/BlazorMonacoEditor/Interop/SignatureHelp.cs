using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

/// <summary>
/// Signature help represents the signature of something callable. There can be multiple signatures but only one active and only one active parameter.
/// </summary>
public sealed record SignatureHelp
{
    /// <summary>
    /// One or more signatures.
    /// </summary>
    [JsonPropertyName("signatures")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<SignatureInformation>? Signatures { get; set; } 
    
    /// <summary>
    /// The active signature.
    /// </summary>
    [JsonPropertyName("activeSignature")]
    public int ActiveSignature { get; set; }
    
    /// <summary>
    /// The active parameter of the active signature.
    /// </summary>
    [JsonPropertyName("activeParameter")]
    public int ActiveParameter { get; set; }

    public static readonly SignatureHelp Empty = new()
    {
        Signatures = new List<SignatureInformation>()
    };
}