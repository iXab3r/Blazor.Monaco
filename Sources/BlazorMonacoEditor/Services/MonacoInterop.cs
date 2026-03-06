using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Scaffolding;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Services
{
    internal sealed class MonacoInterop : IMonacoInterop, IAsyncDisposable
    {
        private const string JsUtilsFilePath = "./_content/BlazorMonacoEditor/scriptLoader.js";
        private const string InteropPrefix = "monacoInterop.";

        private readonly Lazy<Task<IJSObjectReference>> loaderTask;
        private readonly CompositeDisposable anchors = new();
        private readonly IJSRuntime jsRuntime;
        private readonly ILoggerFactory logFactory;
        private readonly string monacoInteropFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonacoInterop"/> class.
        /// </summary>
        /// <param name="jsRuntime">The JS runtime.</param>
        /// <param name="logFactory">The logger factory.</param>
        public MonacoInterop(IJSRuntime jsRuntime, ILoggerFactory logFactory)
        {
            Log = logFactory.CreateLogger<MonacoInterop>();
            this.jsRuntime = jsRuntime;
            this.logFactory = logFactory;
            
            const string defaultMonacoInteropFilePath = $"./_content/BlazorMonacoEditor/js/monaco.bundle.js";
#if DEBUG
            //want to ensure we'll be using new non-cached version every time to avoid hot edits
            monacoInteropFilePath = $"{defaultMonacoInteropFilePath}?v={DateTime.UtcNow.Ticks}";
#else
            monacoInteropFilePath = defaultMonacoInteropFilePath;
#endif

            loaderTask = new Lazy<Task<IJSObjectReference>>(async () =>
            {
                Log.LogDebug("Loading monaco interop from {monacoInteropFilePath}", monacoInteropFilePath);
                var module = await jsRuntime.InvokeAsync<IJSObjectReference>("import", JsUtilsFilePath);
                await module.InvokeVoidAsync("loadScript", monacoInteropFilePath);
                return module;
            });
        }

        public ILogger Log { get; }

        /// <summary>
        /// Create a new Monaco Code Editor, inside the specified HTML element.
        /// </summary>
        /// <param name="element">The HTML element.</param>
        /// <param name="editorId"></param>
        /// <returns>A code editor.</returns>
        public async ValueTask<CodeEditorFacade> CreateEditor(ElementReference element, MonacoEditorId editorId)
        {
            var monacoEditor = new CodeEditorFacade(editorId, this);

            await InvokeVoidAsync("createEditor", editorId, element, DotNetObjectReference.Create(monacoEditor));

            return monacoEditor;
        }

        /// <summary>
        /// Create a new Monaco Diff Editor, inside the specified HTML element.
        /// </summary>
        /// <param name="element">The HTML element.</param>
        /// <param name="editorId">The editor id.</param>
        /// <returns>A diff editor.</returns>
        public async ValueTask<MonacoDiffEditorFacade> CreateDiffEditor(ElementReference element, MonacoEditorId editorId)
        {
            var monacoDiffEditor = new MonacoDiffEditorFacade(editorId, this);

            await InvokeVoidAsync("createDiffEditor", editorId, element, DotNetObjectReference.Create(monacoDiffEditor));

            return monacoDiffEditor;
        }

        /// <summary>
        /// Create a new text model (representing a file).
        /// </summary>
        /// <param name="uri">The URI of the model.</param>
        /// <param name="value">The value of the model (i.e. content of the file).</param>
        /// <param name="languageId">The language ID for the file.</param>
        /// <returns>A text model.</returns>
        public async ValueTask<MonacoTextModelFacade> CreateTextModel(Uri uri, SourceText value, string? languageId = null)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            var textModel = new MonacoTextModelFacade(uri, value, this)
            {
                LanguageId = languageId,
            };

            await InvokeVoidAsync("createTextModel", textModel.Uri.ToString(), textModel.InitialText.ToString(), textModel.ObjectReference, textModel.LanguageId);

            return textModel;
        }

        public async ValueTask<IAsyncDisposable> RegisterCompletionProvider(ICompletionProvider completionProvider)
        {
            var facade = new CompletionProviderFacade(completionProvider, logFactory);
            await InvokeVoidAsync("registerCompletionProvider", facade.ObjectReference);
            return facade;
        }

        public async ValueTask<IAsyncDisposable> RegisterHoverProvider(IHoverProvider hoverProvider)
        {
            var facade = new HoverProviderFacade(hoverProvider);
            await InvokeVoidAsync("registerHoverProvider", facade.ObjectReference);
            return facade;
        }

        public async ValueTask<IAsyncDisposable> RegisterDefinitionProvider(IDefinitionProvider definitionProvider)
        {
            var facade = new DefinitionProviderFacade(definitionProvider, logFactory);
            await InvokeVoidAsync("registerDefinitionProvider", facade.ObjectReference);
            return facade;
        }

        public async ValueTask<IAsyncDisposable> RegisterCodeActionProvider(ICodeActionProvider codeActionProvider)
        {
            var facade = new CodeActionProviderFacade(codeActionProvider);
            await InvokeVoidAsync("registerCodeActionProvider", facade.ObjectReference);
            return facade;
        }

        public async ValueTask<IAsyncDisposable> RegisterSignatureHelpProvider(ISignatureHelpProvider signatureHelpProvider)
        {
            var facade = new SignatureHelpProviderFacade(signatureHelpProvider, Log);
            await InvokeVoidAsync("registerSignatureHelpProvider", facade.ObjectReference);
            return facade;
        }

        public async ValueTask<IAsyncDisposable> RegisterSemanticTokensProvider(ISemanticTokensProvider semanticTokensProvider)
        {
            var facade = new SemanticTokensProviderFacade(semanticTokensProvider, logFactory);
            await InvokeVoidAsync("registerSemanticTokensProvider", facade.ObjectReference);
            return facade;
        }

        public async ValueTask<IAsyncDisposable> RegisterInlayHintsProvider(IInlayHintsProvider inlayHintsProvider)
        {
            var facade = new InlayHintsProviderFacade(inlayHintsProvider, logFactory);
            await InvokeVoidAsync("registerInlayHintsProvider", facade.ObjectReference);
            return facade;
        }

        public async ValueTask DisposeEditor(MonacoEditorId editorId)
        {
            await InvokeVoidAsync("disposeEditor", editorId);
        }

        public async ValueTask DisposeDiffEditor(MonacoEditorId editorId)
        {
            await InvokeVoidAsync("disposeDiffEditor", editorId);
        }

        public async ValueTask UpdateOptions(MonacoEditorId editorId, EditorOptions options)
        {
            await InvokeVoidAsync("updateEditorOptions", editorId, options);
        }

        public async ValueTask UpdateDiffOptions(MonacoEditorId editorId, EditorOptions options)
        {
            await InvokeVoidAsync("updateDiffEditorOptions", editorId, options);
        }

        public async ValueTask ShowCompletionDetails(MonacoEditorId editorId, bool isVisible)
        {
            await InvokeVoidAsync("setEditorCompletionDetailsVisibility", editorId, isVisible);
        }

        public async ValueTask UpdateOptions(MonacoEditorId editorId, GlobalEditorOptions options)
        {
            await InvokeVoidAsync("updateEditorOptions", editorId, options);
        }

        public async ValueTask ExecuteEdits(MonacoEditorId editorId, string editSource, IReadOnlyList<IdentifiedSingleEditOperation> operations)
        {
            await InvokeVoidAsync("executeEditorEdits", editorId, editSource, operations);
        }

        public async ValueTask Focus(MonacoEditorId editorId)
        {
            await InvokeVoidAsync("focusEditor", editorId);
        }

        public async ValueTask FocusAtPosition(MonacoEditorId editorId, int line, int column)
        {
            await InvokeVoidAsync("focusEditorAtPosition", editorId, line, column);
        }

        public async ValueTask SetSelection(MonacoEditorId editorId, MonacoRange range)
        {
            await InvokeVoidAsync("setEditorSelection", editorId, range);
        }

        public async ValueTask RunEditorAction(MonacoEditorId editorId, string actionId)
        {
            await InvokeVoidAsync("runEditorAction", editorId, actionId);
        }

        public async ValueTask<MonacoRange> GetSelection(MonacoEditorId editorId)
        {
            return await InvokeAsync<MonacoRange>("getEditorSelection", editorId);
        }

        public async ValueTask RevealRangeInCenter(MonacoEditorId editorId, MonacoRange range)
        {
            await InvokeVoidAsync("revealEditorRangeInCenter", editorId, range);
        }

        public async ValueTask ExecuteModelEdits(Uri modelUri, IReadOnlyList<IdentifiedSingleEditOperation> operations)
        {
            await InvokeVoidAsync("executeModelEdits", modelUri.ToString(), operations);
        }

        public async ValueTask SetDiffEditorModel(MonacoEditorId editorId, Uri originalModelUri, Uri modifiedModelUri)
        {
            if (originalModelUri is null)
            {
                throw new ArgumentNullException(nameof(originalModelUri));
            }

            if (modifiedModelUri is null)
            {
                throw new ArgumentNullException(nameof(modifiedModelUri));
            }

            await InvokeVoidAsync("setDiffEditorModel", editorId, originalModelUri.ToString(), modifiedModelUri.ToString());
        }

        /// <summary>
        /// Sets the content of a given model.
        /// </summary>
        /// <param name="model">The text model to update.</param>
        /// <returns>Completion task.</returns>
        public async ValueTask DisposeModel(MonacoTextModelFacade model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            await InvokeVoidAsync("disposeTextModel", model.Uri.ToString());
        }

        /// <summary>
        /// Sets the content of a given model.
        /// </summary>
        /// <param name="modelUri">The text model to update.</param>
        /// <param name="newContent">The new content of the file.</param>
        /// <param name="expectedVersionId">The Monaco model version expected by the caller. If specified and mismatched, content is not applied.</param>
        /// <returns>Completion task.</returns>
        public async ValueTask<bool> SetModelContent(Uri modelUri, string newContent, int? expectedVersionId = null)
        {
            if (modelUri is null)
            {
                throw new ArgumentNullException(nameof(modelUri));
            }

            if (expectedVersionId is int expectedVersion)
            {
                return await InvokeAsync<bool>("setModelContent", modelUri.ToString(), newContent, expectedVersion);
            }

            return await InvokeAsync<bool>("setModelContent", modelUri.ToString(), newContent);
        }

        /// <summary>
        /// Sets the markers of a given model.
        /// </summary>
        public async ValueTask SetModelMarkers(Uri modelUri, string markersOwner, IReadOnlyList<MarkerData> markers)
        {
            if (modelUri is null)
            {
                throw new ArgumentNullException(nameof(modelUri));
            }

            await InvokeVoidAsync("setModelMarkers", modelUri.ToString(), markersOwner, markers);
        }

        /// <summary>
        /// Shows a transient in-editor notification for editors currently displaying the specified model.
        /// </summary>
        /// <param name="modelUri">The model URI used to locate active editors.</param>
        /// <param name="message">The notification message text.</param>
        /// <param name="severity">Visual severity hint. Typical values: <c>info</c>, <c>warning</c>, <c>error</c>.</param>
        /// <param name="timeoutMs">Notification visibility timeout in milliseconds.</param>
        /// <param name="closeable">When <see langword="true"/>, renders an explicit close affordance in the notification UI.</param>
        /// <returns>
        /// <see langword="true"/> when at least one editor displayed the notification; otherwise <see langword="false"/>.
        /// </returns>
        public async ValueTask<bool> ShowModelNotification(Uri modelUri, string message, string severity = "warning", int timeoutMs = 4500, bool closeable = false)
        {
            if (modelUri is null)
            {
                throw new ArgumentNullException(nameof(modelUri));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Notification message cannot be empty", nameof(message));
            }

            return await InvokeAsync<bool>("showModelNotification", modelUri.ToString(), message, severity, timeoutMs, closeable);
        }

        public async ValueTask SetModelDecorations(MonacoEditorId editorId, IReadOnlyList<ModelDeltaDecoration> decorations)
        {
            await InvokeVoidAsync("setModelDecorations", editorId.ToString(), decorations);
        }

        /// <summary>
        /// Invoke a method in the monaco typescript layer.
        /// </summary>
        /// <typeparam name="TResult">Result of the method call.</typeparam>
        /// <param name="methodName">The name of the method in the MonacoInterop TypeScript class.</param>
        /// <param name="args">Arguments to the method.</param>
        /// <returns>The result.</returns>
        public async ValueTask<TResult> InvokeAsync<TResult>(string methodName, params object[] args)
        {
            var moduleInterop = await GetMonacoInteropAsync();
            return await moduleInterop.InvokeAsync<TResult>(InteropPrefix + methodName, args);
        }

        /// <summary>
        /// Invoke a method in the monaco typescript layer.
        /// </summary>
        /// <param name="methodName">The name of the method in the MonacoInterop TypeScript class.</param>
        /// <param name="args">Arguments to the method.</param>
        /// <returns>Completion task.</returns>
        public async ValueTask InvokeVoidAsync(string methodName, params object[] args)
        {
            var moduleInterop = await GetMonacoInteropAsync();
            await moduleInterop.InvokeVoidAsync(InteropPrefix + methodName, args);
        }

        private async Task<IJSRuntime> GetMonacoInteropAsync()
        {
            EnsureNotDisposed();
            await loaderTask.Value; //ensure that everything is loaded
            return jsRuntime;
        }

        private void EnsureNotDisposed()
        {
            if (anchors.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(MonacoInterop));
            }
        }

        public async ValueTask DisposeAsync()
        {
            anchors.DisposeJsSafe();

            if (loaderTask.IsValueCreated)
            {
                var module = await loaderTask.Value;
                await module.DisposeJsSafeAsync();
            }
        }
    }
}
