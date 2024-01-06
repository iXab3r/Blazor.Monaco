// noinspection JSUnusedGlobalSymbols,UnnecessaryLocalVariableJS

import {IEditorContext} from './IEditorContext';
import {ITextModelContext} from './ITextModelContext';
import {EditorEventHandler} from './EditorEventHandler';
import {TextModelEventHandler} from './TextModelEventHandler';
import {IBlazorInteropObject} from './IBlazorInteropObject';
import * as monaco from 'monaco-editor';
import '../css/blazor.monaco.css';
import * as logLevel from 'loglevel';

// Initialise the Monaco Environment with the relative URL.
// @ts-ignore
self.MonacoEnvironment = {
    getWorkerUrl: function (moduleId, label) {
        return "./_content/BlazorMonacoEditor/js/editor.worker.bundle.js";
    }
};

class MonacoInterop {

    private static instance:MonacoInterop;
    private readonly logger: logLevel.Logger = logLevel.getLogger(MonacoInterop.name);
    
    private readonly editors: { [id: string]: IEditorContext } = {};
    private readonly models: { [uri: string]: ITextModelContext } = {};
    
    constructor()
    {
        if (MonacoInterop.instance) {
            return MonacoInterop.instance;
        }

        this.logger.setLevel(logLevel.levels.TRACE);
        MonacoInterop.instance = this;
        this.logger.info(`MonacoInterop instance is being created`);
        monaco.editor.setTheme('vs-dark');
    }

    /**
     * Create an editor control.
     * @param editorId The id of the control (to reference it later)
     * @param container The HTML Element container.
     * @param blazorCallback An object on which to invoke event methods.
     */
    createEditor(editorId: string, container: HTMLElement, blazorCallback: IBlazorInteropObject) {
        this.logger.debug(`Creating editor with Id ${editorId} inside ${container}, .net callback: ${blazorCallback}`);
        const newEditor = monaco.editor.create(container);

        const editorContext: IEditorContext = {
            editorId: editorId,
            codeEditor: newEditor,
            updating: false,
            eventHandler: new EditorEventHandler(blazorCallback),
            eventSink: blazorCallback
        };

        const resizeObserver = new ResizeObserver(() => {
            if (newEditor) {
                newEditor.layout();
            }
        });
        resizeObserver.observe(container);

        this.editors[editorId] = editorContext;
    }
    
    disposeEditor(editorId: string){
        this.logger.debug(`Disposing editor ${editorId}`)
        const editorCtxt = this.getEditorById(editorId);
        this.editors[editorId] = null;
        editorCtxt.codeEditor.dispose();
    }
    
    updateEditorOptions(editorId: string, newOptions: monaco.editor.IEditorOptions & monaco.editor.IGlobalEditorOptions){
        this.logger.info(`Updating editor options: ${JSON.stringify(newOptions)}`);
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
        this.logger.debug(`Creating text model with Uri ${uri}, language: ${language}, .net callback: ${blazorCallback}`);
        
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
        this.logger.debug(`Created text model with Uri ${uri}, Id: ${model.id}`);
    }

    disposeTextModel(textModelUri: string){
        this.logger.debug(`Disposing text model with Uri ${textModelUri}`);
        
        const modelCtxt = this.getTextModelByUri(textModelUri);

        this.models[textModelUri] = null;
        modelCtxt.textModel.dispose();
        this.logger.debug(`Disposed text model with Uri ${textModelUri}, id: ${modelCtxt.textModel.id}`);
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

    async registerCompletionProvider(blazorCallback: IBlazorInteropObject) {
        this.logger.debug(`Registering new completion provider: ${blazorCallback}`);
        const languageId: string = await blazorCallback.invokeMethodAsync("GetLanguage");
        const triggerCharacters: string[] = await blazorCallback.invokeMethodAsync("GetTriggerCharacters");
        this.logger.debug(`Completion provider language: ${languageId}, trigger characters: ${JSON.stringify(triggerCharacters)}`);
        
        monaco.languages.registerCompletionItemProvider(languageId, {
            provideCompletionItems: async (model: monaco.editor.ITextModel, position: monaco.IPosition, completionContext: monaco.languages.CompletionContext) => {
                const caretOffset = model.getOffsetAt(position);
                this.logger.trace(`Completion request: ${model.uri}, context: ${completionContext}, position: ${position}, caretOffset: ${caretOffset}`);
                const completionList: monaco.languages.CompletionList = await blazorCallback.invokeMethodAsync("ProvideCompletionItems", model.uri, completionContext, position, caretOffset);
                this.logger.trace(`Completion list received: ${completionList.suggestions.length}`);
                return completionList;
            },
            resolveCompletionItem: async (item: monaco.languages.CompletionItem): Promise<monaco.languages.CompletionItem> => {
                const completionItem: monaco.languages.CompletionItem  = await blazorCallback.invokeMethodAsync("ResolveCompletionItem", item);
                return completionItem;
            },
            triggerCharacters: triggerCharacters
        });
    }
}

window['monacoInterop'] = new MonacoInterop();
