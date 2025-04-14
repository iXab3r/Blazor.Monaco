using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding
{
    /// <summary>
    /// Represents a Monaco Code Editor instance on the C# Side.
    /// Calling methods on this object will effect the corresponding editor.
    /// </summary>
    internal sealed class CodeEditorFacade : IAsyncDisposable
    {
        private readonly MonacoInterop interop;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeEditorFacade"/> class.
        /// </summary>
        /// <param name="id">The editor ID.</param>
        /// <param name="interop">The shared monaco interop wrapper.</param>
        public CodeEditorFacade(MonacoEditorId id, MonacoInterop interop)
        {
            Id = id;
            this.interop = interop;
        }

        public CompositeDisposable Anchors { get; } = new();

        /// <summary>
        /// Gets the generated id of the editor.
        /// </summary>
        public MonacoEditorId Id { get; }
        
        public EditorOptions? Options { get; private set; }

        public ValueTask ShowCompletionDetails(MonacoEditorId editorId, bool isVisible)
        {
            return interop.ShowCompletionDetails(editorId, isVisible);
        }
        
        public async ValueTask UpdateOptions(EditorOptions options)
        {
            if (options == Options)
            {
                return;
            }
            await interop.UpdateOptions(Id, options);
            Options = options;
        }

        /// <summary>
        /// Changes the model being displayed in the editor
        /// (effectively like changing the file).
        /// </summary>
        /// <param name="model">The text model to change to.</param>
        /// <returns>A completion task.</returns>
        public async ValueTask SetModel(MonacoTextModelFacade model)
        {
            if (model is null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }

            await interop.InvokeVoidAsync("setEditorModel", Id, model.Uri.ToString());
        }

        public async ValueTask DisposeAsync()
        {
            if (Anchors.IsDisposed)
            {
                return;
            }

            try
            {
                await interop.DisposeEditor(Id);
            }
            catch (Exception e) when (e is JSException or JSDisconnectedException)
            {
                // During disposal ignore such errors because there is a chance that browser context is already disposed
            }
        }
    }
}
