using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Scaffolding;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace BlazorMonacoEditor;

partial class MonacoEditor : IAsyncDisposable
{
    private readonly MonacoRoslynAdapter roslynAdapter = new();
    
    /// <summary>
    /// Reason of using observable for it is that Render and Parameters events could be called simultaneously
    /// </summary>
    private readonly Subject<string> updateSink = new();
    private readonly ITextModel fallbackTextModel = new TextModel();
    private readonly Dictionary<TextModelId, ITextModel> textModelById = new();
    private readonly Dictionary<TextModelId, TextModelFacade> textModelFacadeById = new();
    
    private ElementReference monacoContainer;
    private CodeEditorFacade? editor;
    private TextModelFacade? activeModel;

    [Parameter] public ITextModel? TextModel { get; set; }

    [Parameter] public string? LanguageId { get; set; }

    [Parameter] public EventCallback<string> LanguageIdChanged { get; set; }

    [Parameter] public string? Theme { get; set; }

    [Parameter] public int? LineNumbersMinChars { get; set; }

    [Parameter] public bool ShowLineNumbers { get; set; }

    [Parameter] public EventCallback<bool> ShowLineNumbersChanged { get; set; }

    [Parameter] public bool ShowCodeMap { get; set; }
    
    [Parameter] public bool IsReadOnly { get; set; }
    
    [Parameter] public EventCallback<bool> ShowCodeMapChanged { get; set; }
    
    public CompositeDisposable Anchors { get; } = new();
    
    public MonacoEditorId Id { get; }

    public MonacoEditor()
    {
        var editorId = $"Monaco-{Guid.NewGuid().ToString().Replace("-", "")}";
        Id = new MonacoEditorId(editorId);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Logger.LogDebug("First Render");

            editor = await MonacoInterop.CreateEditor(monacoContainer, Id);
            await MonacoInterop.ShowCompletionDetails(Id, true);
            Logger.LogDebug("Editor Created");

            Anchors.Add(updateSink.SubscribeAsync(UpdateEditor));
            updateSink.OnNext("OnAfterRenderAsync");
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        updateSink.OnNext("OnParametersSetAsync");
    }

    private async Task UpdateEditor( string reason, CancellationToken cancellationToken = default)
    {
        if (editor == null)
        {
            throw new InvalidOperationException("Editor must be created at this point");
        }
        cancellationToken.ThrowIfCancellationRequested();

        Logger.LogDebug($"Updating editor, reason: {reason}");
        
        var textModel = TextModel ?? fallbackTextModel;
        textModelById.TryAdd(textModel.Id, textModel);
        var textModelFacade = await GetOrCreate(textModel, cancellationToken);
        
        var differentUri = activeModel == null || !Equals(activeModel.Uri, textModelFacade.Uri);
        if (differentUri)
        {
            activeModel = textModelFacade;
            await editor.SetModel(textModelFacade);
        }

        await UpdateOptionsIfNeeded();
        Logger.LogDebug($"Update completed, reason: {reason}");
    }

    private async Task<TextModelFacade> GetOrCreate(ITextModel textModel, CancellationToken cancellationToken)
    {
        if (editor == null)
        {
            throw new InvalidOperationException("Editor must be created at this point");
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (textModelFacadeById.TryGetValue(textModel.Id, out var modelFacade))
        {
            return modelFacade;
        }

        var modelUri = new Uri($"inmemory://{editor.Id}/{textModel.Id}");
        Logger.LogDebug($"Creating new facade for text model @ {modelUri}: {textModel}");
        var textModelFacade = await MonacoInterop.CreateTextModel(modelUri, string.Empty, LanguageId ?? string.Empty);
        Logger.LogDebug($"Created model: {textModelFacade}");

        var actualText = await textModel.GetTextAsync(cancellationToken);
        await textModelFacade.SetContent(actualText.ToString());
        
        var modelSubscription = textModelFacade
            .WhenModelContentChanged
            .SubscribeAsync(async (x, token) =>
            {
                try
                {
                    var currentText = await textModel.GetTextAsync(token);
                    var updatedText = roslynAdapter.ApplyChanges(currentText, x);
                    await textModel.SetTextAsync(updatedText, token);
                }
                catch (Exception e)
                {
                    Logger.LogError($"Failed to apply changes to text model {textModel}", e);
                    throw;
                }
            });
        textModelFacade.Anchors.Add(modelSubscription);
         
        textModelFacadeById[textModel.Id] = textModelFacade;
        return textModelFacade;
    }

    private async Task UpdateOptionsIfNeeded()
    {
        if (editor == null)
        {
            throw new InvalidOperationException("Editor is not loaded yet");
        }

        var options = new EditorOptions()
        {
            LineNumbers = ShowLineNumbers ? "on" : "off",
            LineNumbersMinChars = LineNumbersMinChars,
            GlyphMargin = true,
            ReadOnly = IsReadOnly,
            Minimap = new EditorMinimapOptions()
            {
                Enabled = ShowCodeMap
            }
        };
        await editor.UpdateOptions(options);
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);
    }

    public async ValueTask DisposeAsync()
    {
        Anchors.Dispose();
        if (editor != null)
        {
            await editor.DisposeAsync();
        }

        foreach (var textModelFacade in textModelFacadeById.Values)
        {
            await textModelFacade.DisposeAsync();
        }
    }
}