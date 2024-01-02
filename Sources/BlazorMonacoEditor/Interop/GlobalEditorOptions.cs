using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

internal sealed record GlobalEditorOptions
{
    /// <summary>
    /// The number of spaces a tab is equal to.
    /// This setting is overridden based on the file contents when `detectIndentation` is on.
    /// Defaults to 4.
    /// </summary>
    [JsonPropertyName("tabSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? TabSize { get; init; }

    /// <summary>
    /// Insert spaces when pressing `Tab`.
    /// This setting is overridden based on the file contents when `detectIndentation` is on.
    /// Defaults to true.
    /// </summary>
    [JsonPropertyName("insertSpaces")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? InsertSpaces { get; init; }

    /// <summary>
    /// Controls whether `tabSize` and `insertSpaces` will be automatically detected 
    /// when a file is opened based on the file contents.
    /// Defaults to true.
    /// </summary>
    [JsonPropertyName("detectIndentation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? DetectIndentation { get; init; }

    /// <summary>
    /// Remove trailing auto inserted whitespace.
    /// Defaults to true.
    /// </summary>
    [JsonPropertyName("trimAutoWhitespace")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? TrimAutoWhitespace { get; init; }

    /// <summary>
    /// Special handling for large files to disable certain memory intensive features.
    /// Defaults to true.
    /// </summary>
    [JsonPropertyName("largeFileOptimizations")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? LargeFileOptimizations { get; init; }

    /// <summary>
    /// Controls whether completions should be computed based on words in the document.
    /// Can be 'off', 'currentDocument', 'matchingDocuments', or 'allDocuments'.
    /// Defaults to 'currentDocument'.
    /// </summary>
    [JsonPropertyName("wordBasedSuggestions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? WordBasedSuggestions { get; init; }

    /// <summary>
    /// Controls whether word based completions should be included from opened documents 
    /// of the same language or any language.
    /// </summary>
    [JsonPropertyName("wordBasedSuggestionsOnlySameLanguage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? WordBasedSuggestionsOnlySameLanguage { get; init; }

    /// <summary>
    /// Controls whether the semanticHighlighting is shown for the languages that support it.
    /// Can be true, false, or 'configuredByTheme'.
    /// Defaults to 'configuredByTheme'.
    /// </summary>
    [JsonPropertyName("semanticHighlighting.enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SemanticHighlightingEnabled { get; init; }

    /// <summary>
    /// Keep peek editors open even when double-clicking their content or when hitting `Escape`.
    /// Defaults to false.
    /// </summary>
    [JsonPropertyName("stablePeek")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? StablePeek { get; init; }

    /// <summary>
    /// Lines above this length will not be tokenized for performance reasons.
    /// Defaults to 20000.
    /// </summary>
    [JsonPropertyName("maxTokenizationLineLength")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? MaxTokenizationLineLength { get; init; }

    /// <summary>
    /// Theme to be used for rendering.
    /// The current out-of-the-box available themes are: 'vs' (default), 'vs-dark', 'hc-black', 'hc-light'.
    /// You can create custom themes via `monaco.editor.defineTheme`.
    /// To switch a theme, use `monaco.editor.setTheme`.
    /// **NOTE**: The theme might be overwritten if the OS is in high contrast mode,
    /// unless `autoDetectHighContrast` is set to false.
    /// </summary>
    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Theme { get; init; }

    /// <summary>
    /// If enabled, will automatically change to high contrast theme if the OS is using a high contrast theme.
    /// Defaults to true.
    /// </summary>
    [JsonPropertyName("autoDetectHighContrast")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? AutoDetectHighContrast { get; init; }
}