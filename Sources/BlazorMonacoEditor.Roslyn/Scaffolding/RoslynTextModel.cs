using System;
using System.Threading;
using System.Threading.Tasks;
using BlazorMonacoEditor.Scaffolding;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BlazorMonacoEditor.Roslyn.Scaffolding;

public sealed class RoslynTextModel : ITextModel
{
    public SourceText Text { get; set; } = SourceText.From("");
    
    public required string Path { get; init; }

    public required DocumentId DocumentId { get; init; }
    
    public required Workspace Workspace { get; init; }
    
    public async Task<SourceText> GetTextAsync(CancellationToken cancellationToken = default)
    {
        var document = Workspace.CurrentSolution.GetDocument(DocumentId) ?? throw new InvalidOperationException($"Failed to get document {DocumentId}");
        var text = await document.GetTextAsync(cancellationToken);
        return text;
    }

    public async Task SetTextAsync(SourceText sourceText, CancellationToken cancellationToken = default)
    {
        var updatedSolution = Workspace.CurrentSolution.WithDocumentText(DocumentId, sourceText);
        if (!Workspace.TryApplyChanges(updatedSolution))
        {
            throw new InvalidOperationException($"Failed to update solution {updatedSolution}");
        }
    }
}