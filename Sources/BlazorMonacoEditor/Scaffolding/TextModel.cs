using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;

namespace BlazorMonacoEditor.Scaffolding;

public sealed class TextModel : ITextModel
{
    public SourceText Text { get; set; } = SourceText.From("");

    public TextModelId Id { get; } = new(Guid.NewGuid().ToString());
    
    public string Path { get; init; } = "";
    
    public async Task<SourceText> GetTextAsync(CancellationToken cancellationToken = default)
    {
        return Text ?? SourceText.From("");
    }

    public async Task SetTextAsync(SourceText sourceText, CancellationToken cancellationToken = default)
    {
        Text = sourceText;
    }
}