using System;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding
{
    /// <summary>
    /// Representation of a text model that can be displayed by the Code Editor.
    /// </summary>
    internal sealed class TextModel : IAsyncDisposable
    {
        private readonly MonacoInterop interop;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextModel"/> class.
        /// </summary>
        /// <param name="uri">The file URI.</param>
        /// <param name="initialText">Initial value of the model.</param>
        /// <param name="interop">The interop instance.</param>
        internal TextModel(Uri uri, string initialText, MonacoInterop interop)
        {
            Uri = uri;
            this.interop = interop;
            InitialText = initialText;
            Text = initialText;
        }

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

        /// <summary>
        /// Gets or sets the OnModelChanged event handler.
        /// </summary>
        public EventHandler<ModelContentChangedEventArgs> OnModelContentChanged { get; set; }
        
        [JSInvokable]
        public void HandleModelContentChanged(ModelContentChangedEventArgs args)
        {
            OnModelContentChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Sets the content of the text model.
        /// </summary>
        /// <param name="content">The model content.</param>
        /// <returns>Completion task.</returns>
        public async ValueTask SetContent(string content)
        {
            InitialText = content;
            await interop.SetModelContent(this, content);
            Text = content;
        }

        public async ValueTask DisposeAsync()
        {
            await interop.DisposeModel(this);
        }
    }
}
