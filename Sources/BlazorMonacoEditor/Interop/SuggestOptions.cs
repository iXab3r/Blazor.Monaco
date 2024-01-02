using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record SuggestOptions
{
    /// <summary>
    /// Overwrite word ends on accept. Can be 'insert' or 'replace'. Default to false.
    /// </summary>
    [JsonPropertyName("insertMode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? InsertMode { get; set; }

    /// <summary>
    /// Enable graceful matching. Defaults to true.
    /// </summary>
    [JsonPropertyName("filterGraceful")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? FilterGraceful { get; set; }

    /// <summary>
    /// Prevent quick suggestions when a snippet is active. Defaults to true.
    /// </summary>
    [JsonPropertyName("snippetsPreventQuickSuggestions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? SnippetsPreventQuickSuggestions { get; set; }

    /// <summary>
    /// Favors words that appear close to the cursor.
    /// </summary>
    [JsonPropertyName("localityBonus")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? LocalityBonus { get; set; }

    /// <summary>
    /// Enable using global storage for remembering suggestions.
    /// </summary>
    [JsonPropertyName("shareSuggestSelections")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShareSuggestSelections { get; set; }

    /// <summary>
    /// Select suggestions when triggered via quick suggest or trigger characters.
    /// Can be 'always', 'never', 'whenTriggerCharacter', or 'whenQuickSuggestion'.
    /// </summary>
    [JsonPropertyName("selectionMode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SelectionMode { get; set; }

    /// <summary>
    /// Enable or disable icons in suggestions. Defaults to true.
    /// </summary>
    [JsonPropertyName("showIcons")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowIcons { get; set; }

    /// <summary>
    /// Enable or disable the suggest status bar.
    /// </summary>
    [JsonPropertyName("showStatusBar")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowStatusBar { get; set; }

    /// <summary>
    /// Enable or disable the rendering of the suggestion preview.
    /// </summary>
    [JsonPropertyName("preview")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Preview { get; set; }

    /// <summary>
    /// Configures the mode of the preview. Can be 'prefix', 'subword', or 'subwordSmart'.
    /// </summary>
    [JsonPropertyName("previewMode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PreviewMode { get; set; }

    /// <summary>
    /// Show details inline with the label. Defaults to true.
    /// </summary>
    [JsonPropertyName("showInlineDetails")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowInlineDetails { get; set; }

    // Additional properties for suggestion types
    [JsonPropertyName("showMethods")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowMethods { get; set; }

    [JsonPropertyName("showFunctions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowFunctions { get; set; }

    [JsonPropertyName("showConstructors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowConstructors { get; set; }

    [JsonPropertyName("showDeprecated")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowDeprecated { get; set; }

    [JsonPropertyName("matchOnWordStartOnly")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? MatchOnWordStartOnly { get; set; }

    [JsonPropertyName("showFields")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowFields { get; set; }

    [JsonPropertyName("showVariables")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowVariables { get; set; }

    [JsonPropertyName("showClasses")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowClasses { get; set; }

    [JsonPropertyName("showStructs")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowStructs { get; set; }

    [JsonPropertyName("showInterfaces")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowInterfaces { get; set; }

    [JsonPropertyName("showModules")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowModules { get; set; }

    [JsonPropertyName("showProperties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowProperties { get; set; }

    [JsonPropertyName("showEvents")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowEvents { get; set; }

    [JsonPropertyName("showOperators")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowOperators { get; set; }

    [JsonPropertyName("showUnits")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowUnits { get; set; }

    [JsonPropertyName("showValues")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowValues { get; set; }

    [JsonPropertyName("showConstants")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowConstants { get; set; }

    [JsonPropertyName("showEnums")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowEnums { get; set; }

    [JsonPropertyName("showEnumMembers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowEnumMembers { get; set; }

    [JsonPropertyName("showKeywords")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowKeywords { get; set; }

    [JsonPropertyName("showWords")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowWords { get; set; }

    [JsonPropertyName("showColors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowColors { get; set; }

    [JsonPropertyName("showFiles")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowFiles { get; set; }

    [JsonPropertyName("showReferences")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowReferences { get; set; }

    [JsonPropertyName("showFolders")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowFolders { get; set; }

    [JsonPropertyName("showTypeParameters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowTypeParameters { get; set; }

    [JsonPropertyName("showIssues")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowIssues { get; set; }

    [JsonPropertyName("showUsers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowUsers { get; set; }

    [JsonPropertyName("showSnippets")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowSnippets { get; set; }
}