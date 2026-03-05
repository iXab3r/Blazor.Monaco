using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;

namespace BlazorMonacoEditor.Scaffolding;

internal readonly record struct MonacoTextModelChange(
    int VersionId,
    IReadOnlyList<TextChange> Changes,
    SourceText NewText,
    bool IsFlush);
