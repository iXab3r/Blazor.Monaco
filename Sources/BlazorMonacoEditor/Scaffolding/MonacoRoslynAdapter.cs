using BlazorMonacoEditor.Interop;
using Microsoft.CodeAnalysis.Text;
using Range = BlazorMonacoEditor.Interop.Range;

namespace BlazorMonacoEditor.Scaffolding;

internal sealed class MonacoRoslynAdapter
{
    public SourceText ApplyChanges(SourceText sourceText, ModelContentChangedEventArgs contentChangedEvent)
    {
        var result = sourceText;
        foreach (var change in contentChangedEvent.Changes)
        {
            result = ApplyChange(result, change);
        }
        return result;
    }

    private SourceText ApplyChange(SourceText sourceText, ModelContentChange change)
    {
        var span = GetTextSpanFromRange(sourceText, change.Range);
        var newText = change.Text;

        return sourceText.WithChanges(new TextChange(span, newText));
    }

    private TextSpan GetTextSpanFromRange(SourceText sourceText, Range range)
    {
        var start = GetPosition(sourceText, range.StartLineNumber, range.StartColumn);
        var end = GetPosition(sourceText, range.EndLineNumber, range.EndColumn);

        return TextSpan.FromBounds(start, end);
    }

    private int GetPosition(SourceText sourceText, int lineNumber, int column)
    {
        var line = sourceText.Lines[lineNumber - 1]; // Monaco lines are 1-based
        return line.Start + column - 1; // Monaco columns are 1-based
    }
}