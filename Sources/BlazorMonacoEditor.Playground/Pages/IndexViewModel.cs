using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlazorMonacoEditor.Roslyn.Scaffolding;
using BlazorMonacoEditor.Roslyn.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BlazorMonacoEditor.Playground.Pages;

public class IndexViewModel : ReactiveObject
{
    private readonly AdhocWorkspace workspace;
    private readonly List<DocumentId> documents = new();
    private readonly Dictionary<DocumentId, RoslynTextModel> textModelByDocumentId = new();

    public IndexViewModel(IRoslynCompletionProviderController roslynCompletionProviderController)
    {
        RoslynCompletionProviderController = roslynCompletionProviderController;
        Theme = KnownThemes.First();
        LanguageId = KnownLanguages.First();
        workspace = new AdhocWorkspace();
        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, usings: new[] {"System"});
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
        var newDocument = AddDocument();
        DocumentId = newDocument.Id;
        RoslynCompletionProviderController.CompletionProvider.AddWorkspace(workspace);
        this.WhenAnyValue(x => x.DocumentId)
            .Subscribe(x => TextModel = x == null ? null : textModelByDocumentId.GetValueOrDefault(x));
    }
    
    public IRoslynCompletionProviderController RoslynCompletionProviderController { get; }

    public ProjectInfo ProjectInfo { get; }
    
    [Reactive]
    public DocumentId DocumentId { get; set; }
    
    [Reactive]
    public RoslynTextModel TextModel { get; private set; }
    
    public string DocumentIdAsString
    {
        get => DocumentIdTypeConverter.Instance.ConvertToString(DocumentId);
        set => DocumentId = (DocumentId)DocumentIdTypeConverter.Instance.ConvertFromString(value);
    }

    public bool IsVisible { get; set; } = true;
    
    public bool ShowLineNumbers { get; set; } = true;
    
    public bool ShowCodeMap { get; set; } = false;

    public string[] KnownThemes { get; } = { "vs-dark", "vs" };
    
    public string[] KnownLanguages { get; } = { "csharp", "html" };
    
    public string Theme { get; set; }
    
    public string LanguageId { get; set; }
    
    public ProjectId ProjectId => ProjectInfo.Id;

    public IReadOnlyList<DocumentId> Documents => documents;

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

    public Document AddDocument()
    {
        var newDocumentId = DocumentId.CreateNewId(ProjectId);
        var scriptDocumentInfo = DocumentInfo.Create(newDocumentId, "Script.csx", sourceCodeKind: SourceCodeKind.Script);
        var document = workspace.AddDocument(scriptDocumentInfo);
        documents.Add(newDocumentId);
        textModelByDocumentId[newDocumentId] = new RoslynTextModel()
        {
            Workspace = workspace,
            Path = $"@{newDocumentId}",
            DocumentId = newDocumentId,
            Text = SourceText.From($"Code in {newDocumentId} doc")
        };
        return document;
    }
}