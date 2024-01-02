using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public class GotoLocationOptions
{
    /// <summary>
    /// Controls behavior when multiple target locations exist. Possible values include 'peek', 'gotoAndPeek', and 'goto'.
    /// </summary>
    [JsonPropertyName("multiple")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Multiple { get; set; }

    /// <summary>
    /// Controls behavior when multiple target locations for definitions exist. Possible values include 'peek', 'gotoAndPeek', and 'goto'.
    /// </summary>
    [JsonPropertyName("multipleDefinitions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MultipleDefinitions { get; set; }

    /// <summary>
    /// Controls behavior when multiple target locations for type definitions exist. Possible values include 'peek', 'gotoAndPeek', and 'goto'.
    /// </summary>
    [JsonPropertyName("multipleTypeDefinitions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MultipleTypeDefinitions { get; set; }

    /// <summary>
    /// Controls behavior when multiple target locations for declarations exist. Possible values include 'peek', 'gotoAndPeek', and 'goto'.
    /// </summary>
    [JsonPropertyName("multipleDeclarations")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MultipleDeclarations { get; set; }

    /// <summary>
    /// Controls behavior when multiple target locations for implementations exist. Possible values include 'peek', 'gotoAndPeek', and 'goto'.
    /// </summary>
    [JsonPropertyName("multipleImplementations")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MultipleImplementations { get; set; }

    /// <summary>
    /// Controls behavior when multiple target locations for references exist. Possible values include 'peek', 'gotoAndPeek', and 'goto'.
    /// </summary>
    [JsonPropertyName("multipleReferences")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MultipleReferences { get; set; }

    /// <summary>
    /// Command to use as an alternative to going to a definition.
    /// </summary>
    [JsonPropertyName("alternativeDefinitionCommand")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AlternativeDefinitionCommand { get; set; }

    /// <summary>
    /// Command to use as an alternative to going to a type definition.
    /// </summary>
    [JsonPropertyName("alternativeTypeDefinitionCommand")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AlternativeTypeDefinitionCommand { get; set; }

    /// <summary>
    /// Command to use as an alternative to going to a declaration.
    /// </summary>
    [JsonPropertyName("alternativeDeclarationCommand")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AlternativeDeclarationCommand { get; set; }

    /// <summary>
    /// Command to use as an alternative to going to an implementation.
    /// </summary>
    [JsonPropertyName("alternativeImplementationCommand")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AlternativeImplementationCommand { get; set; }

    /// <summary>
    /// Command to use as an alternative to going to a reference.
    /// </summary>
    [JsonPropertyName("alternativeReferenceCommand")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AlternativeReferenceCommand { get; set; }
}