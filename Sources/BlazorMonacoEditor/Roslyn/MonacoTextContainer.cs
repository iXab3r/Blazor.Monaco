using System;
using Microsoft.CodeAnalysis.Text;

namespace BlazorMonacoEditor.Roslyn;

internal sealed class MonacoTextContainer : SourceTextContainer
{
    private SourceText currentText;

    public MonacoTextContainer(SourceText initialText)
    {
        currentText = new MonacoSourceText(initialText, this);
    }

    public override SourceText CurrentText => currentText;

    public override event EventHandler<TextChangeEventArgs>? TextChanged;

    public void UpdateText(SourceText text)
    {
        var oldText = currentText;
        var newText = new MonacoSourceText(text, this);
        currentText = newText;

        var changes = newText.GetChangeRanges(oldText);
        TextChanged?.Invoke(this, new TextChangeEventArgs(oldText, newText, changes));
    }
}