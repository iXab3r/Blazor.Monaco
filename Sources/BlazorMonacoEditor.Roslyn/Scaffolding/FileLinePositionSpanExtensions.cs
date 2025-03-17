using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BlazorMonacoEditor.Roslyn.Scaffolding;

public static class FileLinePositionSpanExtensions
{
    public static FileLinePositionSpan ToMonacoFileLinePositionSpan(this FileLinePositionSpan fileLinePositionSpan)
    {
        return new FileLinePositionSpan(
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract There is a way how to make Path null, despite annotations
            path: fileLinePositionSpan.Path ?? string.Empty,
            new LinePosition(fileLinePositionSpan.StartLinePosition.Line + 1, fileLinePositionSpan.StartLinePosition.Character + 1),
            new LinePosition(fileLinePositionSpan.EndLinePosition.Line + 1, fileLinePositionSpan.EndLinePosition.Character + 1));
    }
}