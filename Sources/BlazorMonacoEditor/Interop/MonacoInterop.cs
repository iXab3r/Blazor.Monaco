using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorMonacoEditor.Scaffolding;
using BlazorMonacoEditor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Interop
{
    /// <summary>
    /// Main interop class for communicating with the TypeScript components.
    /// </summary>
    internal sealed class MonacoInterop : IMonacoInterop
    {
        private const string InteropPrefix = "monacoInterop.";

        private readonly IJSRuntime jsRuntime;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonacoInterop"/> class.
        /// </summary>
        /// <param name="jsRuntime">The JS runtime.</param>
        /// <param name="logFactory">The logger factory.</param>
        public MonacoInterop(IJSRuntime jsRuntime, ILoggerFactory logFactory)
        {
            this.jsRuntime = jsRuntime;
           
            this.logger = logFactory.CreateLogger<MonacoInterop>();
        }

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
        /// Create a new text model (representing a file).
        /// </summary>
        /// <param name="uri">The URI of the model.</param>
        /// <param name="value">The value of the model (i.e. content of the file).</param>
        /// <param name="languageId">The language ID for the file.</param>
        /// <returns>A text model.</returns>
        public async ValueTask<TextModelFacade> CreateTextModel(Uri uri, string value, string languageId = null)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            var textModel = new TextModelFacade(uri, value, this)
            {
                LanguageId = languageId,
            };

            await InvokeVoidAsync("createTextModel", textModel.Uri.ToString(), textModel.InitialText, textModel.ObjectReference, textModel.LanguageId);

            return textModel;
        }
        
        public async ValueTask<IAsyncDisposable> RegisterCompletionProvider(ICompletionProvider completionProvider)
        {
            var facade = new CompletionProviderFacade(completionProvider);
            await InvokeVoidAsync("registerCompletionProvider", facade.ObjectReference);
            return facade;
        }
        
        public async ValueTask<IAsyncDisposable> RegisterCodeActionProvider(ICodeActionProvider codeActionProvider)
        {
            var facade = new CodeActionProviderFacade(codeActionProvider);
            await InvokeVoidAsync("registerCodeActionProvider", facade.ObjectReference);
            return facade;
        }
        
        public async ValueTask DisposeEditor(MonacoEditorId editorId)
        {
            await InvokeVoidAsync("disposeEditor", editorId);
        }

        public async ValueTask UpdateOptions(MonacoEditorId editorId, EditorOptions options)
        {
            await InvokeVoidAsync("updateEditorOptions", editorId, options);
        }
        
        public async ValueTask UpdateOptions(MonacoEditorId editorId, GlobalEditorOptions options)
        {
            await InvokeVoidAsync("updateEditorOptions", editorId, options);
        }
        
        public async ValueTask ExecuteEdits(MonacoEditorId editorId, string editSource, IdentifiedSingleEditOperation[] operations)
        {
            await InvokeVoidAsync("executeEditorEdits", editorId, editSource, operations);
        }

        /// <summary>
        /// Sets the content of a given model.
        /// </summary>
        /// <param name="model">The text model to update.</param>
        /// <returns>Completion task.</returns>
        public async ValueTask DisposeModel(TextModelFacade model)
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
        /// <returns>Completion task.</returns>
        public async ValueTask SetModelContent(Uri modelUri, string newContent)
        {
            if (modelUri is null)
            {
                throw new ArgumentNullException(nameof(modelUri));
            }

            await InvokeVoidAsync("setModelContent", modelUri.ToString(), newContent);
        }
        
        /// <summary>
        /// Sets the markers of a given model.
        /// </summary>
        /// <param name="modelUri">The text model to update.</param>
        /// <param name="newContent">The new content of the file.</param>
        /// <returns>Completion task.</returns>
        public async ValueTask SetModelMarkers(Uri modelUri, string markersOwner, MarkerData[] markers)
        {
            if (modelUri is null)
            {
                throw new ArgumentNullException(nameof(modelUri));
            }

            await InvokeVoidAsync("setModelMarkers", modelUri.ToString(), markersOwner, markers);
        } 
        
        public async ValueTask SetModelDecorations(MonacoEditorId editorId, ModelDeltaDecoration[] decorations)
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
        public ValueTask<TResult> InvokeAsync<TResult>(string methodName, params object[] args)
        {
            var fullname = InteropPrefix + methodName;
            return jsRuntime.InvokeAsync<TResult>(fullname, args);
        }

        /// <summary>
        /// Invoke a method in the monaco typescript layer.
        /// </summary>
        /// <param name="methodName">The name of the method in the MonacoInterop TypeScript class.</param>
        /// <param name="args">Arguments to the method.</param>
        /// <returns>Completion task.</returns>
        public ValueTask InvokeVoidAsync(string methodName, params object[] args)
        {
            var fullname = InteropPrefix + methodName;
            return jsRuntime.InvokeVoidAsync(fullname, args);
        }
    }
}