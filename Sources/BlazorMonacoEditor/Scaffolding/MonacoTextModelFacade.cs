using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Roslyn;
using Microsoft.CodeAnalysis.Text;
using Microsoft.JSInterop;

namespace BlazorMonacoEditor.Scaffolding
{
    /// <summary>
    /// Representation of a text model that can be displayed by the Code Editor.
    /// </summary>
    internal sealed class MonacoTextModelFacade : IAsyncDisposable
    {
        private readonly MonacoInterop interop;
        private readonly Subject<ModelContentChangedEventArgs> modelContentChangesSink = new();
        private readonly MonacoTextContainer textContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonacoTextModelFacade"/> class.
        /// </summary>
        /// <param name="uri">The file URI.</param>
        /// <param name="initialText">Initial value of the model.</param>
        /// <param name="interop">The interop instance.</param>
        internal MonacoTextModelFacade(Uri uri, SourceText initialText, MonacoInterop interop)
        {
            Uri = uri;
            this.interop = interop;
            InitialText = initialText;
            ObjectReference = DotNetObjectReference.Create(this);
            Anchors.Add(ObjectReference);

            textContainer = new MonacoTextContainer(initialText);
            var remoteModelChangesSubscription = modelContentChangesSink
                .Subscribe(x =>
                {
                    var currentText = Text;
                    var updatedText = MonacoRoslynAdapter.ApplyChanges(currentText, x);
                    textContainer.UpdateText(updatedText); //raise events
                });
            Anchors.Add(remoteModelChangesSubscription);
        }

        public CompositeDisposable Anchors { get; } = new();
        
        public DotNetObjectReference<MonacoTextModelFacade> ObjectReference { get; }

        /// <summary>
        /// Gets the URI of the model.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Gets or sets the language ID of the text model.
        /// </summary>
        public string? LanguageId { get; set; }

        /// <summary>
        /// Gets the initial model value (as loaded from the store).
        /// </summary>
        public SourceText InitialText { get; }

        /// <summary>
        /// Gets the current value of the model.
        /// </summary>
        public SourceText Text => textContainer.CurrentText;
        
        [JSInvokable]
        public void HandleModelContentChanged(ModelContentChangedEventArgs args)
        {
            modelContentChangesSink.OnNext(args);
        }

        public async ValueTask SetContent(string content)
        {
            await interop.SetModelContent(Uri, content);
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
            catch (Exception e) when (e is JSException or JSDisconnectedException)
            {
                // During disposal ignore such errors because there is a chance that browser context is already disposed
            }
        }
    }
}
