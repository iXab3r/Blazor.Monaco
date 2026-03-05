using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Services;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding
{
    /// <summary>
    /// Represents a Monaco Diff Editor instance on the C# side.
    /// Calling methods on this object will affect the corresponding diff editor.
    /// </summary>
    internal sealed class MonacoDiffEditorFacade : IAsyncDisposable
    {
        private readonly MonacoInterop interop;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonacoDiffEditorFacade"/> class.
        /// </summary>
        /// <param name="id">The editor ID.</param>
        /// <param name="interop">The shared monaco interop wrapper.</param>
        public MonacoDiffEditorFacade(MonacoEditorId id, MonacoInterop interop)
        {
            Id = id;
            this.interop = interop;
        }

        public CompositeDisposable Anchors { get; } = new();

        /// <summary>
        /// Gets the generated id of the diff editor.
        /// </summary>
        public MonacoEditorId Id { get; }

        public EditorOptions? Options { get; private set; }

        public async ValueTask UpdateOptions(EditorOptions options)
        {
            if (options == Options)
            {
                return;
            }
            await interop.UpdateDiffOptions(Id, options);
            Options = options;
        }

        /// <summary>
        /// Changes the original/modified models displayed in the diff editor.
        /// </summary>
        /// <param name="originalModel">The original text model.</param>
        /// <param name="modifiedModel">The modified text model.</param>
        /// <returns>A completion task.</returns>
        public async ValueTask SetModel(MonacoTextModelFacade originalModel, MonacoTextModelFacade modifiedModel)
        {
            if (originalModel is null)
            {
                throw new ArgumentNullException(nameof(originalModel));
            }
            if (modifiedModel is null)
            {
                throw new ArgumentNullException(nameof(modifiedModel));
            }

            await interop.SetDiffEditorModel(Id, originalModel.Uri, modifiedModel.Uri);
        }

        public async ValueTask DisposeAsync()
        {
            if (Anchors.IsDisposed)
            {
                return;
            }

            try
            {
                await interop.DisposeDiffEditor(Id);
            }
            catch (Exception e) when (e is JSException or JSDisconnectedException)
            {
                // During disposal ignore such errors because there is a chance that browser context is already disposed
            }
        }
    }
}
