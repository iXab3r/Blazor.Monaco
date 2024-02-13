using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Scaffolding;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using PoeShared.Services;

namespace BlazorMonacoEditor;

partial class MonacoEditor : IAsyncDisposable
{
    private readonly MonacoRoslynAdapter roslynAdapter = new();
    
    /// <summary>
    /// Reason of using observable for it is that Render and Parameters events could be called simultaneously
    /// </summary>
    private readonly Subject<string> updateSink = new();
    
    private ElementReference monacoContainer;
    private CodeEditorFacade? editor;
    private TextModelFacade? currentTextModel;

    public CompositeDisposable Anchors { get; } = new();

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
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Logger.LogDebug("First Render");
            editor = await MonacoInterop.CreateEditor(monacoContainer);
            Logger.LogDebug("Editor Created");

            Anchors.Add(updateSink.SubscribeAsync(UpdateEditor));
            updateSink.OnNext("OnAfterRenderAsync");
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
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
        
        var textModel = TextModel ?? new TextModel();
        var modelUri = new Uri($"inmemory://{editor.Id}/{textModel.Path}");
        var differentUri = !string.Equals(modelUri.ToString(), currentTextModel?.Uri.ToString());

        if (currentTextModel == null || differentUri)
        {
            if (currentTextModel != null)
            {
                await currentTextModel.DisposeAsync();
            }

            Logger.LogDebug($"Requesting text from text model: {textModel}");
            var actualText = await textModel.GetTextAsync(cancellationToken);

            currentTextModel = await MonacoInterop.CreateTextModel(modelUri, string.Empty, LanguageId ?? string.Empty);
            Logger.LogDebug($"Created model: {currentTextModel}");
            await editor.SetModel(currentTextModel);
            await currentTextModel.SetContent(actualText.ToString());

            var modelSubscription = currentTextModel
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
            currentTextModel.Anchors.Add(modelSubscription);
        }

        await UpdateOptionsIfNeeded();
        Logger.LogDebug($"Update completed, reason: {reason}");
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

        if (currentTextModel != null)
        {
            await currentTextModel.DisposeAsync();
        }
    }
}