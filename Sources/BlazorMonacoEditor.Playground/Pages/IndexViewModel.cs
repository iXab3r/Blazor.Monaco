using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using ReactiveUI;
using CompletionItem = BlazorMonacoEditor.Interop.CompletionItem;
using CompletionList = BlazorMonacoEditor.Interop.CompletionList;

namespace BlazorMonacoEditor.Playground.Pages;

public class IndexViewModel : ReactiveObject, ICompletionProvider
{
    private readonly AdhocWorkspace workspace;

    public IndexViewModel()
    {
        Theme = KnownThemes.First();
        LanguageId = KnownLanguages.First();
        workspace = new AdhocWorkspace();
        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        ProjectInfo = ProjectInfo.Create(ProjectId.CreateNewId(),
            VersionStamp.Default,
            "Test",
            "Test",
            LanguageNames.CSharp,
            compilationOptions: compilationOptions,
            isSubmission: true)
            .WithMetadataReferences(new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            }.Distinct());
        var project = workspace.AddProject(ProjectInfo);

        DocumentId = DocumentId.CreateNewId(project.Id);
        var scriptDocumentInfo = DocumentInfo.Create(DocumentId, "Script.csx", sourceCodeKind: SourceCodeKind.Script);
        workspace.AddDocument(scriptDocumentInfo);
        workspace.OpenDocument(DocumentId);
    }
    
    public ProjectInfo ProjectInfo { get; }
    
    public DocumentId DocumentId { get; }
    
    public bool IsVisible { get; set; } = true;
    
    public bool ShowLineNumbers { get; set; } = true;
    
    public bool ShowCodeMap { get; set; } = false;

    public string[] KnownThemes { get; } = { "vs-dark", "vs" };
    
    public string[] KnownLanguages { get; } = { "csharp", "html" };
    
    public string Theme { get; set; }
    
    public string LanguageId { get; set; }
    
    public ProjectId ProjectId => ProjectInfo.Id;

    public Document Document => workspace.CurrentSolution.GetDocument(DocumentId) ?? throw new ArgumentException($"Failed to find document by Id {DocumentId}"); 
    
    public Project Project => workspace.CurrentSolution.GetProject(ProjectId) ?? throw new ArgumentException($"Failed to find project by Id {ProjectId}"); 

    public SourceText SourceCode
    {
        get => Document.GetTextAsync(CancellationToken.None).Result;
        set
        {
            var updatedSolution = workspace.CurrentSolution.WithDocumentText(DocumentId, value);
            if (!workspace.TryApplyChanges(updatedSolution))
            {
                throw new InvalidOperationException($"Failed to update solution {updatedSolution}");
            }
        }
    }

    public async Task<CompletionList> ProvideCompletionItems(MonacoUri modelUri, Position caretPosition, int caretOffset)
    {
        var completionService = Document.Project.Services.GetRequiredService<CompletionService>();
        var roslynCompletionList = await completionService.GetCompletionsAsync(Document, caretOffset);
      
        var monacoSuggestions = new List<CompletionItem>();
        foreach (var roslynCompletion in roslynCompletionList.ItemsList)
        {
            var monacoCompletionItem = new CompletionItem()
            {
                Label = new CompletionItemLabel{ Label = roslynCompletion.DisplayText},
            };
            monacoSuggestions.Add(monacoCompletionItem);
        }
        var monacoCompletionList = new CompletionList()
        {
            Incomplete = false,
            Suggestions = monacoSuggestions
        };
        return monacoCompletionList;
    }
}