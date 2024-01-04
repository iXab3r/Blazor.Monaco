namespace BlazorMonacoEditor.Interop;

public enum CompletionItemInsertTextRule
{
    None = 0,

    /// <summary>
    /// Adjust whitespace/indentation of multiline insert texts to
    /// match the current line indentation.
    /// </summary>
    KeepWhitespace = 1,

    /// <summary>
    /// `insertText` is a snippet.
    /// </summary>
    InsertAsSnippet = 4
}