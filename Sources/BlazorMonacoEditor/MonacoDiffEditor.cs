using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    private readonly ConcurrentDictionary<string, MonacoTextModelFacade> textModelFacadeByUri = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, Lazy<Task<MonacoTextModelFacade>>> textModelFacadeTasksByUri = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, string> textModelUriByIdentity = new(StringComparer.Ordinal);

    private ElementReference monacoContainer;
    private MonacoDiffEditorFacade? editor;
    private MonacoTextModelFacade? activeOriginalModel;
    private MonacoTextModelFacade? activeModifiedModel;

    /// <summary>
    /// Gets or sets the text model displayed in the original (left) pane.
    /// When <see langword="null"/>, an internal fallback model is used.
    /// </summary>
    /// <remarks>
    /// The model is synchronized to a Monaco text model through <see cref="TextSyncCoordinator"/>.
    /// Monaco text model API: https://microsoft.github.io/monaco-editor/typedoc/interfaces/editor.ITextModel.html
    /// </remarks>
    [Parameter] public ITextModel? OriginalTextModel { get; set; }

    /// <summary>
    /// Gets or sets the text model displayed in the modified (right) pane.
    /// When <see langword="null"/>, an internal fallback model is used.
    /// </summary>
    /// <remarks>
    /// The model is synchronized to a Monaco text model through <see cref="TextSyncCoordinator"/>.
    /// </remarks>
    [Parameter] public ITextModel? ModifiedTextModel { get; set; }

    /// <summary>
    /// Gets or sets the explicit Monaco language identifier for both panes.
    /// When empty and <see cref="AutoDetectLanguage"/> is enabled, language is inferred from the model URI extension.
    /// </summary>
    /// <remarks>
    /// This value maps to Monaco's <c>languageId</c> when each pane model is created.
    /// Monaco languages API: https://microsoft.github.io/monaco-editor/typedoc/modules/languages.html
    /// </remarks>
    [Parameter] public string? LanguageId { get; set; }

    /// <summary>
    /// Callback invoked when <see cref="LanguageId"/> changes.
    /// </summary>
    [Parameter] public EventCallback<string> LanguageIdChanged { get; set; }

    /// <summary>
    /// Gets or sets the Monaco theme name.
    /// </summary>
    /// <remarks>
    /// This component currently applies theme globally via Monaco interop theme setup.
    /// Monaco theme API: https://microsoft.github.io/monaco-editor/typedoc/functions/editor.defineTheme.html
    /// </remarks>
    [Parameter] public string? Theme { get; set; }

    /// <summary>
    /// Gets or sets the minimum width (in characters) reserved for line numbers.
    /// </summary>
    /// <remarks>
    /// Forwarded to Monaco option <c>lineNumbersMinChars</c> for the underlying diff editors.
    /// Editor options reference: https://microsoft.github.io/monaco-editor/typedoc/interfaces/editor.IDiffEditorConstructionOptions.html
    /// </remarks>
    [Parameter] public int? LineNumbersMinChars { get; set; }

    /// <summary>
    /// Gets or sets whether line numbers are visible in both panes.
    /// </summary>
    /// <remarks>
    /// Forwarded as Monaco option <c>lineNumbers</c> with values <c>on</c>/<c>off</c>.
    /// </remarks>
    [Parameter] public bool ShowLineNumbers { get; set; }

    /// <summary>
    /// Callback invoked when <see cref="ShowLineNumbers"/> changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowLineNumbersChanged { get; set; }

    /// <summary>
    /// Gets or sets whether the minimap (code map) is visible.
    /// </summary>
    /// <remarks>
    /// Forwarded to Monaco option <c>minimap.enabled</c>.
    /// </remarks>
    [Parameter] public bool ShowCodeMap { get; set; }

    /// <summary>
    /// Gets or sets whether language should be inferred from model URI extensions when <see cref="LanguageId"/> is empty.
    /// </summary>
    [Parameter] public bool AutoDetectLanguage { get; set; }

    /// <summary>
    /// Gets or sets whether the modified pane is read-only.
    /// </summary>
    /// <remarks>
    /// Forwarded to Monaco option <c>readOnly</c> for diff editor behavior.
    /// </remarks>
    [Parameter] public bool IsReadOnly { get; set; }

    /// <summary>
    /// Gets or sets whether the original pane is editable.
    /// </summary>
    /// <remarks>
    /// Forwarded to Monaco diff option <c>originalEditable</c>.
    /// </remarks>
    [Parameter] public bool OriginalEditable { get; set; }

    /// <summary>
    /// Gets or sets whether the diff editor should render two panes side by side.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    [Parameter] public bool RenderSideBySide { get; set; } = true;

    /// <summary>
    /// Gets or sets whether Monaco may collapse to inline diff mode when there is not enough space.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    [Parameter] public bool UseInlineViewWhenSpaceIsLimited { get; set; } = true;

    /// <summary>
    /// Gets or sets whether semantic highlighting is enabled for this diff editor instance.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    /// <remarks>
    /// Forwarded as global Monaco option <c>semanticHighlighting.enabled</c>.
    /// This controls semantic token rendering only; token generation is provided by registered semantic token providers.
    /// Monaco semantic tokens API: https://microsoft.github.io/monaco-editor/typedoc/interfaces/languages.DocumentSemanticTokensProvider.html
    /// </remarks>
    [Parameter] public bool SemanticHighlightingEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets whether inlay hints are enabled for this diff editor instance.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    /// <remarks>
    /// Forwarded as global Monaco option <c>inlayHints.enabled</c>.
    /// This controls hint rendering only; hint generation is provided by registered inlay hint providers.
    /// Monaco inlay hints API: https://microsoft.github.io/monaco-editor/typedoc/interfaces/languages.InlayHintsProvider.html
    /// </remarks>
    [Parameter] public bool InlayHintsEnabled { get; set; } = true;

    /// <summary>
    /// Callback invoked when <see cref="ShowCodeMap"/> changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ShowCodeMapChanged { get; set; }

    /// <summary>
    /// Gets disposables tied to this diff editor instance lifecycle.
    /// </summary>
    /// <remarks>
    /// Disposed by <see cref="DisposeAsync"/> to release Rx subscriptions and JS interop resources.
    /// </remarks>
    public CompositeDisposable Anchors { get; } = new();

    /// <summary>
    /// Gets the stable Monaco diff editor identifier used for JS interop calls.
    /// </summary>
    /// <remarks>
    /// Used as a key when routing update calls through <see cref="Services.MonacoInterop"/>.
    /// </remarks>
    public MonacoEditorId Id { get; }

    /// <summary>
    /// Initializes a new <see cref="MonacoDiffEditor"/> with a unique editor identifier.
    /// </summary>
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

        var remoteOriginalModelUpdate = await GetOrCreateRemoteModel(localOriginalModel, "original", cancellationToken);
        var remoteModifiedModelUpdate = await GetOrCreateRemoteModel(localModifiedModel, "modified", cancellationToken);
        var remoteOriginalModel = remoteOriginalModelUpdate.Model;
        var remoteModifiedModel = remoteModifiedModelUpdate.Model;

        var differentOriginalUri = activeOriginalModel == null || !Equals(activeOriginalModel.Uri, remoteOriginalModel.Uri);
        var differentModifiedUri = activeModifiedModel == null || !Equals(activeModifiedModel.Uri, remoteModifiedModel.Uri);
        if (differentOriginalUri || differentModifiedUri)
        {
            activeOriginalModel = remoteOriginalModel;
            activeModifiedModel = remoteModifiedModel;
            await editor.SetModel(remoteOriginalModel, remoteModifiedModel);
        }

        await DisposeRemoteModels(remoteOriginalModelUpdate.StaleModel, remoteModifiedModelUpdate.StaleModel);
        await UpdateOptionsIfNeeded();
        Logger.LogDebug($"Update completed, reason: {reason}");
    }

    private async Task<RemoteModelUpdate> GetOrCreateRemoteModel(ITextModel localModel, string role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var modelUri = MonacoModelUri.Create(Id, localModel, role);
        var modelKey = modelUri.ToString();
        var language = AutoDetectLanguage && string.IsNullOrEmpty(LanguageId)
            ? DetectLanguage(localModel.Path)
            : LanguageId;

        var modelFacade = await GetOrCreateRemoteModel(localModel, modelKey, modelUri, language, cancellationToken);
        await UpdateModelLanguageIfNeeded(modelFacade, language);
        var staleModel = TrackModelIdentity($"{localModel.Id}:{role}", modelKey);
        return new RemoteModelUpdate(modelFacade, staleModel);
    }

    private async Task<MonacoTextModelFacade> GetOrCreateRemoteModel(
        ITextModel localModel,
        string modelKey,
        Uri modelUri,
        string? language,
        CancellationToken cancellationToken)
    {
        var lazyTask = textModelFacadeTasksByUri.GetOrAdd(
            modelKey,
            _ => new Lazy<Task<MonacoTextModelFacade>>(
                () => CreateRemoteModel(localModel, modelKey, modelUri, language, cancellationToken),
                LazyThreadSafetyMode.ExecutionAndPublication));

        try
        {
            return await lazyTask.Value;
        }
        catch
        {
            textModelFacadeTasksByUri.TryRemove(new KeyValuePair<string, Lazy<Task<MonacoTextModelFacade>>>(modelKey, lazyTask));
            throw;
        }
    }

    private async Task<MonacoTextModelFacade> CreateRemoteModel(
        ITextModel localModel,
        string modelKey,
        Uri modelUri,
        string? language,
        CancellationToken cancellationToken)
    {
        Logger.LogDebug($"Creating new remote(Monaco) facade for local text model @ {modelUri}: {localModel}");

        Logger.LogDebug($"Creating model with language '{language}' @ {modelUri}: {localModel}");
        var actualText = await localModel.GetTextAsync(cancellationToken);
        var remoteModel = await MonacoInterop.CreateTextModel(modelUri, actualText, language);
        remoteModel.Anchors.Add(Disposable.Create(() => Logger.LogDebug("Remote(Monaco) text model is being been disposed: {remoteModel}", remoteModel)));

        Logger.LogDebug($"Created remote(Monaco) model: {remoteModel}");
        var syncCoordinator = new TextSyncCoordinator(localModel, remoteModel, Logger);
        remoteModel.Anchors.Add(syncCoordinator);
        remoteModel.Anchors.Add(Disposable.Create(() => Logger.LogDebug("Remote(Monaco) text model has been disposed: {remoteModel}", remoteModel)));

        textModelFacadeByUri[modelKey] = remoteModel;
        return remoteModel;
    }

    private MonacoTextModelFacade? TrackModelIdentity(string identityKey, string modelKey)
    {
        while (true)
        {
            if (!textModelUriByIdentity.TryGetValue(identityKey, out var previousModelKey))
            {
                if (textModelUriByIdentity.TryAdd(identityKey, modelKey))
                {
                    return null;
                }

                continue;
            }

            if (string.Equals(previousModelKey, modelKey, StringComparison.Ordinal))
            {
                return null;
            }

            if (!textModelUriByIdentity.TryUpdate(identityKey, modelKey, previousModelKey))
            {
                continue;
            }

            textModelFacadeTasksByUri.TryRemove(previousModelKey, out _);
            return textModelFacadeByUri.TryRemove(previousModelKey, out var previousModel)
                ? previousModel
                : null;
        }
    }

    private async Task UpdateModelLanguageIfNeeded(MonacoTextModelFacade modelFacade, string? language)
    {
        if (string.IsNullOrWhiteSpace(language) || string.Equals(modelFacade.LanguageId, language, StringComparison.Ordinal))
        {
            return;
        }

        await MonacoInterop.SetModelLanguage(modelFacade.Uri, language);
        modelFacade.LanguageId = language;
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
            RenderSideBySide = RenderSideBySide,
            UseInlineViewWhenSpaceIsLimited = UseInlineViewWhenSpaceIsLimited,
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
            SemanticHighlightingEnabled = SemanticHighlightingEnabled,
            InlayHintsEnabled = InlayHintsEnabled
        });
    }

    /// <summary>
    /// Assigns component parameters and forwards lifecycle updates to the diff-editor synchronization pipeline.
    /// </summary>
    /// <param name="parameters">The incoming parameter values.</param>
    /// <returns>A task that completes when parameter assignment has finished.</returns>
    /// <remarks>
    /// After assignment, updates are queued through <c>updateSink</c> to coalesce concurrent parameter/render updates.
    /// </remarks>
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);
    }

    /// <summary>
    /// Asynchronously disposes the diff editor, associated text models, and all lifecycle anchors.
    /// </summary>
    /// <returns>A value task that completes when disposal is finished.</returns>
    /// <remarks>
    /// Disposes in order:
    /// <c>Anchors</c>, diff editor facade (if created), then all model facades created for this component instance.
    /// </remarks>
    public async ValueTask DisposeAsync()
    {
        Anchors.DisposeJsSafe();
        if (editor != null)
        {
            await editor.DisposeJsSafeAsync();
        }

        await DisposeAllRemoteModels();
    }

    private static string? DetectLanguage(string? path)
    {
        var extension = Path.GetExtension(path ?? string.Empty)
            .TrimStart('.')
            .ToLowerInvariant();
        return extension switch
        {
            "csx" => "csharp",
            "cs" => "csharp",
            _ => null
        };
    }

    private async ValueTask DisposeAllRemoteModels()
    {
        var models = new HashSet<MonacoTextModelFacade>(textModelFacadeByUri.Values);
        foreach (var lazyTask in textModelFacadeTasksByUri.Values)
        {
            if (!lazyTask.IsValueCreated)
            {
                continue;
            }

            try
            {
                models.Add(await lazyTask.Value);
            }
            catch (Exception e) when (e is TaskCanceledException or OperationCanceledException)
            {
            }
        }

        foreach (var textModelFacade in models)
        {
            await textModelFacade.DisposeJsSafeAsync();
        }
    }

    private static async ValueTask DisposeRemoteModels(params MonacoTextModelFacade?[] models)
    {
        foreach (var model in models)
        {
            if (model != null)
            {
                await model.DisposeJsSafeAsync();
            }
        }
    }

    private sealed record RemoteModelUpdate(MonacoTextModelFacade Model, MonacoTextModelFacade? StaleModel);
}
