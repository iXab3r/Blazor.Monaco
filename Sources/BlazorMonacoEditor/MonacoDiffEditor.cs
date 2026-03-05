using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Scaffolding;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace BlazorMonacoEditor;

partial class MonacoDiffEditor : IAsyncDisposable
{
    private static long editorIdx = 0;

    /// <summary>
    /// Reason of using observable for it is that Render and Parameters events could be called simultaneously
    /// </summary>
    private readonly Subject<string> updateSink = new();
    private readonly ITextModel fallbackOriginalTextModel = new TextModel();
    private readonly ITextModel fallbackModifiedTextModel = new TextModel();

    //concurrent collections are needed as async/load process could be in async mode
    private readonly ConcurrentDictionary<TextModelId, ITextModel> textModelById = new();
    private readonly ConcurrentDictionary<TextModelId, MonacoTextModelFacade> textModelFacadeById = new();

    private ElementReference monacoContainer;
    private MonacoDiffEditorFacade? editor;
    private MonacoTextModelFacade? activeOriginalModel;
    private MonacoTextModelFacade? activeModifiedModel;

    [Parameter] public ITextModel? OriginalTextModel { get; set; }

    [Parameter] public ITextModel? ModifiedTextModel { get; set; }

    [Parameter] public string? LanguageId { get; set; }

    [Parameter] public EventCallback<string> LanguageIdChanged { get; set; }

    [Parameter] public string? Theme { get; set; }

    [Parameter] public int? LineNumbersMinChars { get; set; }

    [Parameter] public bool ShowLineNumbers { get; set; }

    [Parameter] public EventCallback<bool> ShowLineNumbersChanged { get; set; }

    [Parameter] public bool ShowCodeMap { get; set; }

    [Parameter] public bool AutoDetectLanguage { get; set; }

    [Parameter] public bool IsReadOnly { get; set; }

    [Parameter] public bool OriginalEditable { get; set; }

    [Parameter] public EventCallback<bool> ShowCodeMapChanged { get; set; }

    public CompositeDisposable Anchors { get; } = new();

    public MonacoEditorId Id { get; }

    public MonacoDiffEditor()
    {
        var editorId = $"monaco-diff-{Interlocked.Increment(ref editorIdx)}";
        Id = new MonacoEditorId(editorId);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Logger.LogDebug("First Render");

            editor = await MonacoInterop.CreateDiffEditor(monacoContainer, Id);
            Logger.LogDebug("Diff Editor Created");

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
            throw new InvalidOperationException("Diff editor must be created at this point");
        }
        cancellationToken.ThrowIfCancellationRequested();

        Logger.LogDebug($"Updating diff editor, reason: {reason}");

        var localOriginalModel = OriginalTextModel ?? fallbackOriginalTextModel;
        var localModifiedModel = ModifiedTextModel ?? fallbackModifiedTextModel;

        textModelById.TryAdd(localOriginalModel.Id, localOriginalModel);
        textModelById.TryAdd(localModifiedModel.Id, localModifiedModel);

        var remoteOriginalModel = await GetOrCreateRemoteModel(localOriginalModel, cancellationToken);
        var remoteModifiedModel = await GetOrCreateRemoteModel(localModifiedModel, cancellationToken);

        var differentOriginalUri = activeOriginalModel == null || !Equals(activeOriginalModel.Uri, remoteOriginalModel.Uri);
        var differentModifiedUri = activeModifiedModel == null || !Equals(activeModifiedModel.Uri, remoteModifiedModel.Uri);
        if (differentOriginalUri || differentModifiedUri)
        {
            activeOriginalModel = remoteOriginalModel;
            activeModifiedModel = remoteModifiedModel;
            await editor.SetModel(remoteOriginalModel, remoteModifiedModel);
        }

        await UpdateOptionsIfNeeded();
        Logger.LogDebug($"Update completed, reason: {reason}");
    }

    private async Task<MonacoTextModelFacade> GetOrCreateRemoteModel(ITextModel localModel, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (textModelFacadeById.TryGetValue(localModel.Id, out var modelFacade))
        {
            return modelFacade;
        }

        var modelUri = new Uri($"inmemory://{Id}/{localModel.Id}");
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
        remoteModel.Anchors.Add(Disposable.Create(() => Logger.LogDebug("Remote(Monaco) text model is being been disposed: {remoteModel}", remoteModel)));

        Logger.LogDebug($"Created remote(Monaco) model: {remoteModel}");
        var syncCoordinator = new TextSyncCoordinator(localModel, remoteModel, Logger);
        remoteModel.Anchors.Add(syncCoordinator);
        remoteModel.Anchors.Add(Disposable.Create(() => Logger.LogDebug("Remote(Monaco) text model has been disposed: {remoteModel}", remoteModel)));

        textModelFacadeById[localModel.Id] = remoteModel;
        return remoteModel;
    }

    private async Task UpdateOptionsIfNeeded()
    {
        if (editor == null)
        {
            throw new InvalidOperationException("Diff editor is not loaded yet");
        }

        var options = new EditorOptions()
        {
            AutomaticLayout = true,
            FixedOverflowWidgets = true,
            InDiffEditor = true,
            OriginalEditable = OriginalEditable,
            LineNumbers = ShowLineNumbers ? "on" : "off",
            LineNumbersMinChars = LineNumbersMinChars,
            GlyphMargin = true,
            ReadOnly = IsReadOnly,
            Minimap = new EditorMinimapOptions()
            {
                Enabled = ShowCodeMap
            }
        };
        Logger.LogDebug("Updating diff editor options, AutomaticLayout: {AutomaticLayout}", options.AutomaticLayout);
        await editor.UpdateOptions(options);
        await MonacoInterop.UpdateOptions(Id, new GlobalEditorOptions
        {
            SemanticHighlightingEnabled = true
        });
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

    private static string DetectLanguage(Uri uri)
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
