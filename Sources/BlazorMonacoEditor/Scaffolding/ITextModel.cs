using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;

namespace BlazorMonacoEditor.Scaffolding;

public interface ITextModel
{
    TextModelId Id { get; }
    string Path { get; }
    Task<SourceText> GetTextAsync(CancellationToken cancellationToken = default);
    Task SetTextAsync(SourceText sourceText, CancellationToken cancellationToken = default);
}
