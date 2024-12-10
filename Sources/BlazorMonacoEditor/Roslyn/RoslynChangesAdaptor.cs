#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BlazorMonacoEditor.Roslyn;

public sealed class RoslynChangesAdaptor
{
    public static async Task<IReadOnlyList<IdentifiedSingleEditOperation>> PrepareDiff(Document oldDocument, Document newDocument)
    {
        var oldText = await oldDocument.GetTextAsync();
        var newText = await newDocument.GetTextAsync();
        return PrepareDiff(oldText, newText);
    }
    
    public static IReadOnlyList<IdentifiedSingleEditOperation> PrepareDiff(SourceText oldText, SourceText newText)
    {
        var changes = newText.GetTextChanges(oldText);
        return Adapt(changes, oldText);
    }
    
    private static IReadOnlyList<IdentifiedSingleEditOperation> Adapt(
        IEnumerable<TextChange> changes,
        SourceText oldDocumentText)
    {
        var monacoChanges = new List<IdentifiedSingleEditOperation>();
        foreach (var textChange in changes)
        {
            var startPosition = oldDocumentText.Lines.GetLinePosition(textChange.Span.Start);
            var endPosition = oldDocumentText.Lines.GetLinePosition(textChange.Span.End);

            var editOperation = new IdentifiedSingleEditOperation
            {
                Range = new MonacoRange
                {
                    StartLineNumber = startPosition.Line + 1,
                    StartColumn = startPosition.Character + 1,
                    EndLineNumber = endPosition.Line + 1,
                    EndColumn = endPosition.Character + 1,
                },
                Text = textChange.NewText
            };

            monacoChanges.Add(editOperation);
        }

        return monacoChanges;
    }
}