using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding
{
    /// <summary>
    /// Representation of a text model that can be displayed by the Code Editor.
    /// </summary>
    internal sealed class TextModelFacade : IAsyncDisposable
    {
        private readonly MonacoInterop interop;
        private readonly Subject<ModelContentChangedEventArgs> modelContentChangesSink = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TextModelFacade"/> class.
        /// </summary>
        /// <param name="uri">The file URI.</param>
        /// <param name="initialText">Initial value of the model.</param>
        /// <param name="interop">The interop instance.</param>
        internal TextModelFacade(Uri uri, string initialText, MonacoInterop interop)
        {
            Uri = uri;
            this.interop = interop;
            InitialText = initialText;
            Text = initialText;
            ObjectReference = DotNetObjectReference.Create(this);
        }

        public CompositeDisposable Anchors { get; } = new();
        
        public DotNetObjectReference<TextModelFacade> ObjectReference { get; }

        /// <summary>
        /// Gets the URI of the model.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Gets or sets the language ID of the text model.
        /// </summary>
        public string LanguageId { get; set; }

        /// <summary>
        /// Gets the initial model value (as loaded from the store).
        /// </summary>
        public string InitialText { get; private set; }

        /// <summary>
        /// Gets the current value of the model.
        /// </summary>
        public string Text { get; private set; }

        public IObservable<ModelContentChangedEventArgs> WhenModelContentChanged => modelContentChangesSink;
        
        [JSInvokable]
        public void HandleModelContentChanged(ModelContentChangedEventArgs args)
        {
            modelContentChangesSink.OnNext(args);
        }

        /// <summary>
        /// Sets the content of the text model.
        /// </summary>
        /// <param name="content">The model content.</param>
        /// <returns>Completion task.</returns>
        public async ValueTask SetContent(string content)
        {
            InitialText = content;
            await interop.SetModelContent(this.Uri, content);
            Text = content;
        }

        public async ValueTask DisposeAsync()
        {
            if (Anchors.IsDisposed)
            {
                return;
            }
            Anchors.DisposeJsSafe();
            try
            {
                await interop.DisposeModel(this);
            }
            catch (JSException)
            {
                // During disposal ignore such errors because there is a chance that browser context is already disposed
            }
        }
    }
}
