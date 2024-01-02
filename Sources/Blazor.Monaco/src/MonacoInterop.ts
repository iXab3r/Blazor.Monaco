import { editor, Uri } from 'monaco-editor/esm/vs/editor/editor.api';
import { languages } from 'monaco-editor/esm/metadata';
import { IEditorContext } from './IEditorContext';
import { ITextModelContext } from './ITextModelContext';
import { EditorEventHandler } from './EditorEventHandler';
import { TextModelEventHandler } from './TextModelEventHandler';
import { IBlazorInteropObject } from './IBlazorInteropObject';

// Initialise the Monaco Environment with the relative URL.
// @ts-ignore
self.MonacoEnvironment = {
    getWorkerUrl: function (moduleId, label) {
        return "./_content/Blazor.Monaco/editor.worker.bundle.js";
    }
};

/**
 * Monaco Interop TypeScript
 * */
class MonacoInterop {

    /**
     * Set of created editors
     */
    private editors: { [id: string]: IEditorContext } = {};
    /**
     * Set of created text models
     */
    private models: { [uri: string]: ITextModelContext } = {};
    
    constructor()
    {
        editor.setTheme('vs-dark');
    }

    /**
     * Create an editor control.
     * @param id The id of the control (to reference it later)
     * @param container The HTML Element container.
     * @param blazorCallback An object on which to invoke event methods.
     */
    createEditor(id: string, container: HTMLElement, blazorCallback: IBlazorInteropObject) {

        const newEditor = editor.create(container);

        const editorContext: IEditorContext = {
            id: id,
            codeEditor: newEditor,
            updating: false,
            eventHandler: new EditorEventHandler(blazorCallback),
            eventSink: blazorCallback
        };

        this.editors[id] = editorContext;
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
        const monacoUri = Uri.parse(uri);

        const model = editor.createModel(value, language, monacoUri);

        const modelContext: ITextModelContext = {
            textModel: model,
            changeTimer: 0,
            updating: false,
            eventHandler: new TextModelEventHandler(blazorCallback),
            eventSink: blazorCallback
        };

        // Attach events to the model.
        model.onDidChangeContent(ev => {

            if (modelContext.changeTimer)
            {
                clearTimeout(modelContext.changeTimer);
                modelContext.changeTimer = 0;
            }

            modelContext.changeTimer = setTimeout(() => {

                modelContext.changeTimer = 0;
                // Wait 1 second for someone to finish typing,
                // then raise the event.
                modelContext.eventHandler.modelUpdated(model.getValue());

            }, 1000);
        });

        this.models[uri] = modelContext;
    }

    /**
     * Set the model markers for a text model.
     * @param textModelUri The URI of the text model.
     * @param owner The owner of the markers.
     * @param markers The full set of new markers for the model.
     */
    setModelMarkers(textModelUri: string, owner: string, markers: editor.IMarkerData[])
    {
        const modelCtxt = this.models[textModelUri];

        if (!modelCtxt) {
            throw "Specified model not created.";
        }

        editor.setModelMarkers(modelCtxt.textModel, owner, markers);

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
        const modelCtxt = this.models[textModelUri];

        if (!modelCtxt) {
            throw "Specified model not created.";
        }

        modelCtxt.textModel.setValue(newContent);
    }

    /**
     * Change the model an editor is displaying.
     * @param editorId The ID of the editor.
     * @param textModelUri The URI of the text model.
     */
    setEditorModel(editorId: string, textModelUri: string)
    {
        const editorCtxt = this.editors[editorId];
        const modelCtxt = this.models[textModelUri];

        if (!editorCtxt)
        {
            throw "Specified editor not created.";
        }

        if (!modelCtxt)
        {
            throw "Specified model not created.";
        }

        editorCtxt.codeEditor.setModel(modelCtxt.textModel);
    }
}

// This is what we'll export to the 'window' for monaco
// interop work.
window['monacoInterop'] = new MonacoInterop();
