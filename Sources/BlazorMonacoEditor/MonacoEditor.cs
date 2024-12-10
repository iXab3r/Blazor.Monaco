using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Roslyn;
using BlazorMonacoEditor.Scaffolding;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;

namespace BlazorMonacoEditor;

partial class MonacoEditor : IAsyncDisposable
{
    private static long editorIdx = 0;
    private readonly MonacoRoslynAdapter roslynAdapter = new();
    
    /// <summary>
    /// Reason of using observable for it is that Render and Parameters events could be called simultaneously
    /// </summary>
    private readonly Subject<string> updateSink = new();
    private readonly ITextModel fallbackTextModel = new TextModel();
    
    //concurrent collections are needed as async/load process could be in async mode
    private readonly ConcurrentDictionary<TextModelId, ITextModel> textModelById = new();
    private readonly ConcurrentDictionary<TextModelId, MonacoTextModelFacade> textModelFacadeById = new();
    
    private ElementReference monacoContainer;
    private CodeEditorFacade? editor;
    private MonacoTextModelFacade? activeModel;

    [Parameter] public ITextModel? TextModel { get; set; }

    [Parameter] public string? LanguageId { get; set; }

    [Parameter] public EventCallback<string> LanguageIdChanged { get; set; }

    [Parameter] public string? Theme { get; set; }

    [Parameter] public int? LineNumbersMinChars { get; set; }

    [Parameter] public bool ShowLineNumbers { get; set; }

    [Parameter] public EventCallback<bool> ShowLineNumbersChanged { get; set; }

    [Parameter] public bool ShowCodeMap { get; set; }
    
    [Parameter] public bool AutoDetectLanguage { get; set; }
    
    [Parameter] public bool IsReadOnly { get; set; }
    
    [Parameter] public EventCallback<bool> ShowCodeMapChanged { get; set; }
    
    public CompositeDisposable Anchors { get; } = new();
    
    public MonacoEditorId Id { get; }

    public MonacoEditor()
    {
        var editorId = $"monaco-{Interlocked.Increment(ref editorIdx)}";
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

    private async Task UpdateEditor(string reason, CancellationToken cancellationToken = default)
    {
        if (editor == null)
        {
            throw new InvalidOperationException("Editor must be created at this point");
        }
        cancellationToken.ThrowIfCancellationRequested();

        Logger.LogDebug($"Updating editor, reason: {reason}");
        
        var localModel = TextModel ?? fallbackTextModel;
        textModelById.TryAdd(localModel.Id, localModel);
        var remoteModel = await GetOrCreateRemoteModel(localModel, cancellationToken);
        
        var differentUri = activeModel == null || !Equals(activeModel.Uri, remoteModel.Uri);
        if (differentUri)
        {
            activeModel = remoteModel;
            await editor.SetModel(remoteModel);
        }

        await UpdateOptionsIfNeeded();
        Logger.LogDebug($"Update completed, reason: {reason}");
    }

    private async Task<MonacoTextModelFacade> GetOrCreateRemoteModel(ITextModel localModel, CancellationToken cancellationToken)
    {
        if (editor == null)
        {
            throw new InvalidOperationException("Editor must be created at this point");
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (textModelFacadeById.TryGetValue(localModel.Id, out var modelFacade))
        {
            return modelFacade;
        }

        var modelUri = new Uri($"inmemory://{editor.Id}/{localModel.Id}");
        Logger.LogDebug($"Creating new remote(Monaco) facade for local text model @ {modelUri}: {localModel}");

        string? language;
        if (AutoDetectLanguage && string.IsNullOrEmpty(LanguageId))
        {
            language = DetectLanguage(modelUri);
        }
        else
        {
            language = LanguageId;
        }
        
        Logger.LogDebug($"Creating model with language '{language}' @ {modelUri}: {localModel}");
        var actualText = await localModel.GetTextAsync(cancellationToken);
        var remoteModel = await MonacoInterop.CreateTextModel(modelUri, actualText, language ?? string.Empty);

        var remoteTextContainer = remoteModel.Text.Container;
        var remoteTextChangesSubscription = Observable.FromEventPattern<TextChangeEventArgs>(
            h => remoteTextContainer.TextChanged += h,
            h => remoteTextContainer.TextChanged -= h
        )
        .SubscribeAsync(async (x, token) =>
        {
            if (x.EventArgs.Changes.Count <= 0)
            {
                return;
            }
            
            await localModel.SetTextAsync(x.EventArgs.NewText, cancellationToken);
        });
        remoteModel.Anchors.Add(remoteTextChangesSubscription);

        Logger.LogDebug($"Created remote(Monaco) model: {remoteModel}");
        var localModelChangesSubscription = localModel
            .WhenChanged
            .SubscribeAsync(async (x, token) =>
            {
                try
                {
                    var localText = await localModel.GetTextAsync(token);
                    var remoteText = remoteModel.Text;
                    if (localText.ContentEquals(remoteText))
                    {
                        return;
                    }
                    
                    var monacoChanges = RoslynChangesAdaptor.PrepareDiff(oldText: remoteText, newText: localText);
                    if (monacoChanges.Any())
                    {
                        await MonacoInterop.ExecuteModelEdits(remoteModel.Uri, monacoChanges);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError($"Failed to apply local changes to remote(Monaco) text model {localModel}", e);
                    throw;
                }
            });
        remoteModel.Anchors.Add(localModelChangesSubscription);
         
        textModelFacadeById[localModel.Id] = remoteModel;
        return remoteModel;
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
        Anchors.DisposeJsSafe();
        if (editor != null)
        {
            await editor.DisposeJsSafeAsync();
        }

        foreach (var textModelFacade in textModelFacadeById.Values)
        {
            await textModelFacade.DisposeJsSafeAsync();
        }
    }
    
    private static string DetectLanguage(System.Uri uri)
    {
        var extension = Path.GetExtension(uri.LocalPath)
            .TrimStart('.')
            .ToLowerInvariant();
        return extension switch
        {
            "css" => "css",
            "js" => "javascript", 
            "ts" => "typescript",
            "html" => "html",
            "cshtml" => "razor",
            "razor" => "razor",
            "csx" => "csharp",
            "cs" => "csharp",
            _ => string.Empty
        };
    }
}