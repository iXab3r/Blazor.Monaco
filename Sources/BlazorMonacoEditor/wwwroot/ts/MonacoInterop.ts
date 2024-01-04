import { IEditorContext } from './IEditorContext';
import { ITextModelContext } from './ITextModelContext';
import { EditorEventHandler } from './EditorEventHandler';
import { TextModelEventHandler } from './TextModelEventHandler';
import { IBlazorInteropObject } from './IBlazorInteropObject';
import * as monaco from 'monaco-editor';
import '../css/blazor.monaco.css';

// Initialise the Monaco Environment with the relative URL.
// @ts-ignore
self.MonacoEnvironment = {
    getWorkerUrl: function (moduleId, label) {
        return "./_content/BlazorMonacoEditor/js/editor.worker.bundle.js";
    }
};

class MonacoInterop {

    private editors: { [id: string]: IEditorContext } = {};
    private models: { [uri: string]: ITextModelContext } = {};
    
    constructor()
    {
        monaco.editor.setTheme('vs-dark');
    }

    /**
     * Create an editor control.
     * @param editorId The id of the control (to reference it later)
     * @param container The HTML Element container.
     * @param blazorCallback An object on which to invoke event methods.
     */
    createEditor(editorId: string, container: HTMLElement, blazorCallback: IBlazorInteropObject) {
        const newEditor = monaco.editor.create(container);

        const editorContext: IEditorContext = {
            editorId: editorId,
            codeEditor: newEditor,
            updating: false,
            eventHandler: new EditorEventHandler(blazorCallback),
            eventSink: blazorCallback
        };

        this.editors[editorId] = editorContext;
    }
    
    disposeEditor(editorId: string){
        const editorCtxt = this.getEditorById(editorId);
        this.editors[editorId] = null;
        editorCtxt.codeEditor.dispose();
    }
    
    updateEditorOptions(editorId: string, newOptions: monaco.editor.IEditorOptions & monaco.editor.IGlobalEditorOptions){
        console.info(`Updating editor options: ${JSON.stringify(newOptions)}`);
        const editorCtxt = this.getEditorById(editorId);
        editorCtxt.codeEditor.updateOptions(newOptions);
    }
    
    /**
     * Create a new text model.
     * @param uri The URI of the model.
     * @param value The content of the model.
     * @param blazorCallback An object on which to call event handling methods.
     * @param language The ID of the model's language.
     */
    createTextModel(uri: string, value: string, blazorCallback: IBlazorInteropObject, language?: string)
    {
        const monacoUri = monaco.Uri.parse(uri);

        const model = monaco.editor.createModel(value, language, monacoUri);
        
        const modelContext: ITextModelContext = {
            textModel: model,
            changeTimer: 0,
            updating: false,
            eventHandler: new TextModelEventHandler(blazorCallback),
            eventSink: blazorCallback
        };

        model.onDidChangeContent(ev => {

            modelContext.eventHandler.handleModelContentChanged(ev);
        });

        this.models[uri] = modelContext;
    }

    disposeTextModel(textModelUri: string){
        const modelCtxt = this.getTextModelByUri(textModelUri);

        this.models[textModelUri] = null;
        modelCtxt.textModel.dispose();
    }

    /**
     * Set the model markers for a text model.
     * @param textModelUri The URI of the text model.
     * @param owner The owner of the markers.
     * @param markers The full set of new markers for the model.
     */
    setModelMarkers(textModelUri: string, owner: string, markers: monaco.editor.IMarkerData[])
    {
        const modelCtxt = this.getTextModelByUri(textModelUri);

        monaco.editor.setModelMarkers(modelCtxt.textModel, owner, markers);

        // Force a background re-tokenise when we get the model markers through, because compilation changes may have caused
        // everything to change.
        const unsafeModel: any = modelCtxt.textModel;
        unsafeModel._tokenization._resetTokenizationState();
    }

    /**
     * Sets the content for a model.
     * @param textModelUri The text model URI.
     * @param newContent The new content of the model.
     */
    setModelContent(textModelUri: string, newContent: string)
    {
        const modelCtxt = this.getTextModelByUri(textModelUri);
        modelCtxt.textModel.setValue(newContent);
    }

    /**
     * Sets the content for a model.
     * @param textModelUri The text model URI.
     * @param languageId LanguageId
     */
    setEditorModelLanguage(textModelUri: string, languageId: string)
    {
        const modelCtxt = this.getTextModelByUri(textModelUri);
        monaco.editor.setModelLanguage(modelCtxt.textModel, languageId);
    }

    /**
     * Change the model an editor is displaying.
     * @param editorId The ID of the editor.
     * @param textModelUri The URI of the text model.
     */
    setEditorModel(editorId: string, textModelUri: string)
    {
        const editorCtxt = this.getEditorById(editorId);
        const modelCtxt = this.getTextModelByUri(textModelUri);
        editorCtxt.codeEditor.setModel(modelCtxt.textModel);
    }
    
    getTextModelByUri(textModelUri: string) : ITextModelContext{
        const modelCtxt = this.models[textModelUri];
        if (!modelCtxt)
        {
            throw `Specified model ${textModelUri} is not created.`;
        }
        return modelCtxt;
    }
    
    getEditorById(editorId: string) : IEditorContext{
        const editorCtxt = this.editors[editorId];
        if (!editorCtxt)
        {
            throw `Specified editor ${editorId} is not created.`;
        }
        return editorCtxt;
    }

    registerCompletionProvider(languageId: string, blazorCallback: IBlazorInteropObject) {
        monaco.languages.registerCompletionItemProvider(languageId, {
            provideCompletionItems: async (model: monaco.editor.ITextModel, position: monaco.IPosition, completionContext: monaco.languages.CompletionContext) => {
                console.info(`Completion request, model uri: ${model.uri}, position: ${position}`);
                const caretOffset = model.getOffsetAt(position);
                const completions:monaco.languages.CompletionList = await blazorCallback.invokeMethodAsync("ProvideCompletionItems", model.uri, position, caretOffset);
                return completions;
            }
        });
    }
}

window['monacoInterop'] = new MonacoInterop();
