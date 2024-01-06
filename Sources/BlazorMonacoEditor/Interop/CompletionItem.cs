using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record CompletionItem
{
    /// <summary>
    /// The label of this completion item. By default this is also the text that is inserted when selecting
    /// this completion.
    /// </summary>
    [JsonPropertyName("label")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public CompletionItemLabel? Label { get; set; } 

    /// <summary>
    /// The kind of this completion item. Based on the kind an icon is chosen by the editor.
    /// </summary>
    [JsonPropertyName("kind")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public CompletionItemKind? Kind { get; set; }

    /// <summary>
    /// A modifier to the `kind` which affect how the item is rendered, e.g. Deprecated is rendered with a strikeout.
    /// </summary>
    [JsonPropertyName("tags")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<CompletionItemTag>? Tags { get; set; }

    /// <summary>
    /// A human-readable string with additional information about this item, like type or symbol information.
    /// </summary>
    [JsonPropertyName("detail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Detail { get; set; }

    /// <summary>
    /// A human-readable string that represents a doc-comment.
    /// </summary>
    [JsonPropertyName("documentation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public MarkdownString? Documentation { get; set; } // Can be string or IMarkdownString

    /// <summary>
    /// A string that should be used when comparing this item with other items.
    /// </summary>
    [JsonPropertyName("sortText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SortText { get; set; }

    /// <summary>
    /// A string that should be used when filtering a set of completion items.
    /// </summary>
    [JsonPropertyName("filterText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? FilterText { get; set; }

    /// <summary>
    /// Select this item when showing.
    /// </summary>
    [JsonPropertyName("preselect")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Preselect { get; set; }

    /// <summary>
    /// A string or snippet that should be inserted in a document when selecting this completion.
    /// </summary>
    [JsonPropertyName("insertText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? InsertText { get; set; }

    /// <summary>
    /// Additional rules that should be applied when inserting this completion.
    /// </summary>
    [JsonPropertyName("insertTextRules")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public CompletionItemInsertTextRule? InsertTextRules { get; set; }

    /// <summary>
    /// A range of text that should be replaced by this completion item.
    /// </summary>
    [JsonPropertyName("range")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Range? Range { get; set; } // Can be IRange or CompletionItemRanges

    /// <summary>
    /// An optional set of characters that accept this completion when pressed.
    /// </summary>
    [JsonPropertyName("commitCharacters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IEnumerable<string>? CommitCharacters { get; set; }

    /// <summary>
    /// An optional array of additional text edits that are applied when selecting this completion.
    /// </summary>
    [JsonPropertyName("additionalTextEdits")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IEnumerable<SingleEditOperation>? AdditionalTextEdits { get; set; }

    /// <summary>
    /// A command that should be run upon acceptance of this item.
    /// </summary>
    [JsonPropertyName("command")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Command? Command { get; set; }
}