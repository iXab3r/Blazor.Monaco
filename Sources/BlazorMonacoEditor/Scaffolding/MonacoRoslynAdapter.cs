using System;
using System.Collections.Generic;
using BlazorMonacoEditor.Interop;
using Microsoft.CodeAnalysis.Text;

namespace BlazorMonacoEditor.Scaffolding;

internal sealed class MonacoRoslynAdapter
{
    public static IReadOnlyList<TextChange> PrepareChanges(SourceText sourceText, ModelContentChangedEventArgs contentChangedEvent)
    {
        if (contentChangedEvent.Changes == null || contentChangedEvent.Changes.Count <= 0)
        {
            return Array.Empty<TextChange>();
        }

        var result = new List<TextChange>(contentChangedEvent.Changes.Count);
        var currentText = sourceText;
        foreach (var change in contentChangedEvent.Changes)
        {
            var textChange = PrepareChange(currentText, change);
            result.Add(textChange);
            currentText = currentText.WithChanges(textChange);
        }

        return result;
    }

    public static SourceText ApplyChanges(SourceText sourceText, ModelContentChangedEventArgs contentChangedEvent)
    {
        var preparedChanges = PrepareChanges(sourceText, contentChangedEvent);
        return ApplyChanges(sourceText, preparedChanges);
    }

    public static SourceText ApplyChanges(SourceText sourceText, IReadOnlyList<TextChange> changes)
    {
        if (changes == null || changes.Count <= 0)
        {
            return sourceText;
        }

        var result = sourceText;
        foreach (var change in changes)
        {
            result = result.WithChanges(change);
        }

        return result;
    }

    private static TextChange PrepareChange(SourceText sourceText, ModelContentChange change)
    {
        var span = GetTextSpanFromRange(sourceText, change.Range);
        var newText = change.Text;

        return new TextChange(span, newText);
    }

    private static TextSpan GetTextSpanFromRange(SourceText sourceText, MonacoRange range)
    {
        var start = GetPosition(sourceText, range.StartLineNumber, range.StartColumn);
        var end = GetPosition(sourceText, range.EndLineNumber, range.EndColumn);

        return TextSpan.FromBounds(start, end);
    }

    private static int GetPosition(SourceText sourceText, int lineNumber, int column)
    {
        var line = sourceText.Lines[lineNumber - 1]; // Monaco lines are 1-based
        return line.Start + column - 1; // Monaco columns are 1-based
    }
}
