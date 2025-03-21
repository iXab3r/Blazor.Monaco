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

interface IMonacoActionArgs {
    modelUri: monaco.Uri,
    caretPosition: monaco.IPosition,
    caretOffset: number,
    editorId: string,
}

class MonacoInterop {

    private static instance: MonacoInterop;
    private readonly logger: logLevel.Logger = logLevel.getLogger(MonacoInterop.name);

    private readonly editors: { [id: string]: IEditorContext } = {};
    private readonly models: { [uri: string]: ITextModelContext } = {};
    private readonly editorIdByMonacoEditorId: Map<string, string> = new Map<string, string>();
    private readonly editorActionsById: Map<string, monaco.editor.IActionDescriptor> = new Map<string, monaco.editor.IActionDescriptor>();

    constructor() {
        if (MonacoInterop.instance) {
            return MonacoInterop.instance;
        }

        this.logger.setLevel(logLevel.levels.DEBUG);
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
        this.logger.debug(`Created editor with Id ${editorId}, MonacoEditorId: ${newEditor.getId()}`);

        this.addDuplicateLineCommand(newEditor);

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
        this.editorIdByMonacoEditorId[newEditor.getId()] = editorId;
    }

    private addDuplicateLineCommand(editor: monaco.editor.IStandaloneCodeEditor) {
        // Add a custom keybinding for Ctrl + D to duplicate the current line
        editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyD, function () {
            const model = editor.getModel();
            const selection = editor.getSelection();

            // Get the line number at the current cursor position
            const lineNumber = selection.positionLineNumber;

            // Get the current column position of the cursor
            const columnNumber = selection.positionColumn;

            // Get the content of the current line (no extra blank line)
            const lineContent = model.getLineContent(lineNumber);

            // Define the edit operation (insert lineContent into the next line)
            const range = new monaco.Range(lineNumber + 1, 1, lineNumber + 1, 1);
            const textEdits = [{
                range: range,
                text: lineContent + '\n'
            }];

            // Push the edit operation as undoable
            editor.pushUndoStop(); // Stop any previous undo group
            editor.executeEdits("duplicateLine", textEdits);
            editor.pushUndoStop(); // Stop the current undo group

            // Move the cursor to the new duplicated line at the same column
            editor.setSelection(new monaco.Selection(lineNumber + 1, columnNumber, lineNumber + 1, columnNumber));
        });
    }

    disposeEditor(editorId: string) {
        this.logger.debug(`Disposing editor ${editorId}`)
        const editorCtxt = this.getEditorById(editorId);
        this.editors[editorId] = null;
        editorCtxt.codeEditor.dispose();
    }

    updateEditorOptions(editorId: string, newOptions: monaco.editor.IEditorOptions & monaco.editor.IGlobalEditorOptions) {
        this.logger.info(`Updating editor options: ${JSON.stringify(newOptions)}`);
        const editorCtxt = this.getEditorById(editorId);
        editorCtxt.codeEditor.updateOptions(newOptions);
    }

    setEditorCompletionDetailsVisibility(editorId: string, isVisible: boolean) {
        const editorCtxt = this.getEditorById(editorId);
        const editor = editorCtxt.codeEditor;
        const suggestionController: any = editor.getContribution("editor.contrib.suggestController");
        suggestionController.widget.value._setDetailsVisible(isVisible);
        //suggestionController.widget.value._persistedSize.store({width: 400, height: 256});
    }

    executeEditorEdits(editorId: string, source: string, edits: monaco.editor.IIdentifiedSingleEditOperation[]) {
        const editorCtxt = this.getEditorById(editorId);
        editorCtxt.codeEditor.executeEdits(source, edits);
    }

    focusEditor(editorId: string) {
        const editorCtxt = this.getEditorById(editorId);
        editorCtxt.codeEditor.focus();
    }

    focusEditorAtPosition(editorId: string, lineNumber: number, column: number) {
        const editorCtxt = this.getEditorById(editorId);
        editorCtxt.codeEditor.focus(); // Transfers keyboard input focus to the editor
        editorCtxt.codeEditor.setPosition({lineNumber: lineNumber, column: column});
        editorCtxt.codeEditor.revealPosition({lineNumber: lineNumber, column: column});
    }

    setEditorSelection(editorId: string, range: monaco.Range) {
        const editorCtxt = this.getEditorById(editorId);
        editorCtxt.codeEditor.setSelection(range);
    }

    getEditorSelection(editorId: string) {
        const editorCtxt = this.getEditorById(editorId);
        const selection = editorCtxt.codeEditor.getSelection();
        const startPosition = selection.getStartPosition();
        const endPosition = selection.getEndPosition();
        return new monaco.Range(startPosition.lineNumber, startPosition.column, endPosition.lineNumber, endPosition.column);
    }

    revealEditorRangeInCenter(editorId: string, range: monaco.Range) {
        const editorCtxt = this.getEditorById(editorId);
        editorCtxt.codeEditor.revealRangeInCenter(range);
    }

    /**
     * Create a new text model.
     * @param uri The URI of the model.
     * @param value The content of the model.
     * @param blazorCallback An object on which to call event handling methods.
     * @param language The ID of the model's language.
     */
    createTextModel(uri: string, value: string, blazorCallback: IBlazorInteropObject, language?: string) {
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

            ev.isEolChange
            modelContext.eventHandler.handleModelContentChanged(ev);
        });

        this.models[uri] = modelContext;
        this.logger.debug(`Created text model with Uri ${uri}, Id: ${model.id}`);
    }

    disposeTextModel(textModelUri: string) {
        this.logger.debug(`Disposing text model with Uri ${textModelUri}`);

        const modelCtxt = this.getTextModelByUri(textModelUri);

        this.models[textModelUri] = null;
        modelCtxt.textModel.dispose();
        this.logger.debug(`Disposed text model with Uri ${textModelUri}, id: ${modelCtxt.textModel.id}`);
    }

    executeModelEdits(textModelUri: string, edits: monaco.editor.IIdentifiedSingleEditOperation[]) {
        const modelCtxt = this.getTextModelByUri(textModelUri);
        modelCtxt.textModel.pushEditOperations(null, edits, () => null);
    }

    setModelMarkers(textModelUri: string, owner: string, markers: monaco.editor.IMarkerData[]) {
        const modelCtxt = this.getTextModelByUri(textModelUri);
        monaco.editor.setModelMarkers(modelCtxt.textModel, owner, markers);
    }

    setModelDecorations(editorId: string, decorations: monaco.editor.IModelDeltaDecoration[]) {
        const editorCtxt = this.getEditorById(editorId);
        let collection = editorCtxt.codeEditor.createDecorationsCollection(decorations);
    }

    /**
     * Sets the content for a model.
     * @param textModelUri The text model URI.
     * @param newContent The new content of the model.
     */
    setModelContent(textModelUri: string, newContent: string) {
        const modelCtxt = this.getTextModelByUri(textModelUri);
        modelCtxt.textModel.setValue(newContent);
    }

    /**
     * Sets the content for a model.
     * @param textModelUri The text model URI.
     * @param languageId LanguageId
     */
    setEditorModelLanguage(textModelUri: string, languageId: string) {
        const modelCtxt = this.getTextModelByUri(textModelUri);
        monaco.editor.setModelLanguage(modelCtxt.textModel, languageId);
    }

    /**
     * Change the model an editor is displaying.
     * @param editorId The ID of the editor.
     * @param textModelUri The URI of the text model.
     */
    setEditorModel(editorId: string, textModelUri: string) {
        const editorCtxt = this.getEditorById(editorId);
        const modelCtxt = this.getTextModelByUri(textModelUri);
        editorCtxt.codeEditor.setModel(modelCtxt.textModel);
    }

    getTextModelByUri(textModelUri: string): ITextModelContext {
        const modelCtxt = this.models[textModelUri];
        if (!modelCtxt) {
            throw `Specified model ${textModelUri} is not created.`;
        }
        return modelCtxt;
    }

    getEditorById(editorId: string): IEditorContext {
        const editorCtxt = this.editors[editorId];
        if (!editorCtxt) {
            throw `Specified editor ${editorId} is not created.`;
        }
        return editorCtxt;
    }

    /**
     * Adds an action to ALL Monaco Editor instances.
     *
     * This method registers an action that is available across all editors.
     * Actions added this way can appear in the context menu, and when properly registered, in the F1 Command Palette.
     *
     */
    async addEditorAction(actionDescriptor: monaco.editor.IActionDescriptor, blazorCallback: IBlazorInteropObject) {
        const action = this.prepareActionDescriptor(actionDescriptor, blazorCallback);
        this.logger.debug(`Adding new action(total:${this.editorActionsById.size}) to ALL Editors: ${JSON.stringify(actionDescriptor)}`);

        const anchor = monaco.editor.addEditorAction(action);
        this.editorActionsById.set(actionDescriptor.id, actionDescriptor);
        return anchor;
    }

    /**
     * Adds an action to a SPECIFIC Monaco Editor instance.
     *
     * This method registers an action that is scoped to a specific editor instance, allowing for targeted functionality.
     * Ideal for per-editor logic or when dynamic context-specific behavior is required.
     *
     */
    async addAction(editorId: string, actionDescriptor: monaco.editor.IActionDescriptor, blazorCallback: IBlazorInteropObject) {
        const editor = this.getEditorById(editorId);
        const action = this.prepareActionDescriptor(actionDescriptor, blazorCallback);
        this.logger.debug(`Adding new action to SPECIFIC editor ${editorId}: ${JSON.stringify(actionDescriptor)}`);

        const anchor = editor.codeEditor.addAction(action);
        return anchor;
    }

    /**
     * Executes a registered Monaco Editor action by its command ID.
     *
     * @param editorId - The Monaco Editor instance id.
     * @param actionId - The ID of the action to execute.
     * @throws Will throw an error if the specified action is not found or fails to execute.
     */
    async runEditorAction(editorId: string, actionId: string) {
        const editorCtxt = this.getEditorById(editorId);
        const action = editorCtxt.codeEditor.getAction(actionId);
        if (action) {
            await action.run();
            return;
        }

        const editorAction = this.editorActionsById.get(actionId);
        if (editorAction) {
            await editorAction.run(editorCtxt.codeEditor);
            return;
        }
        
        console.log(`Default supported actions(${editorCtxt.editorId})`);
        const actions = editorCtxt.codeEditor.getSupportedActions();
        console.table(actions.map(action => ({
            Id: action.id,
            Label: action.label
        })));

        console.log(`Custom actions(${editorCtxt.editorId})`);
        const actionsArray: { Id: string; Label: string }[] = [];
        this.editorActionsById.forEach((action, key) => {
            actionsArray.push({
                Id: action.id,
                Label: action.label
            });
        });

        console.table(actionsArray);
        throw new Error(`Action with ID "${actionId}" was not found for editor ${editorId}.`);
    }

    async registerCodeActionProvider(blazorCallback: IBlazorInteropObject) {
        this.logger.debug(`Registering new code action provider: ${blazorCallback}`);
        const languageId: string = await blazorCallback.invokeMethodAsync("GetLanguage");
        this.logger.debug(`Code action provider language: ${languageId}`);

        monaco.editor.registerCommand("BlazorQuickFixApplyCommand", async function (editor, ...args) {
            await blazorCallback.invokeMethodAsync("ApplyCodeAction", args);
        });

        monaco.languages.registerCodeActionProvider(languageId, {
            provideCodeActions: async (
                model: monaco.editor.ITextModel,
                range: monaco.Range,
                context: monaco.languages.CodeActionContext,
                token: monaco.CancellationToken
            ): Promise<monaco.languages.CodeActionList> => {
                this.logger.trace(`Code action request: ${model.uri}, range: ${range}, context: ${context}`);
                const codeActionList: monaco.languages.CodeActionList = await blazorCallback.invokeMethodAsync("ProvideCodeActions", model.uri, range, context);
                if (!codeActionList) {
                    this.logger.trace(`Code action list is empty`);
                    return null;
                }
                this.logger.trace(`Code action list received: ${codeActionList.actions.length}`);
                return {
                    actions: codeActionList.actions,
                    dispose: () => {
                    }
                };
            },
            resolveCodeAction: async (
                codeAction: monaco.languages.CodeAction,
                token: monaco.CancellationToken
            ): Promise<monaco.languages.CodeAction> => {
                this.logger.trace(`Code action resolve request: ${codeAction}`);
                const resolvedCodeAction: monaco.languages.CodeAction = await blazorCallback.invokeMethodAsync("ResolveCodeAction", codeAction);
                return resolvedCodeAction;
            }
        });
    }

    async registerHoverProvider(blazorCallback: IBlazorInteropObject) {
        this.logger.debug(`Registering new hover provider: ${blazorCallback}`);
        const languageId: string = await blazorCallback.invokeMethodAsync("GetLanguage");
        this.logger.debug(`Hover provider language: ${languageId}`);

        monaco.languages.registerHoverProvider(languageId, {
            provideHover: async (model: monaco.editor.ITextModel, position: monaco.Position, token: monaco.CancellationToken) => {
                this.logger.trace(`Code hover request: ${model.uri}, position: ${position}`);
                const hover: monaco.languages.Hover = await blazorCallback.invokeMethodAsync("ProvideHover", model.uri, position);
                if (!hover) {
                    this.logger.trace(`Code hover is empty`);
                    return null;
                }
                this.logger.trace(`Code hover received`);
                return hover;
            }
        });
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
                if (!completionList) {
                    this.logger.trace(`Completion list is empty`);
                    return null;
                }
                this.logger.trace(`Completion list received: ${completionList.suggestions.length}`);
                return completionList;
            },
            resolveCompletionItem: async (item: monaco.languages.CompletionItem): Promise<monaco.languages.CompletionItem> => {
                const completionItem: monaco.languages.CompletionItem = await blazorCallback.invokeMethodAsync("ResolveCompletionItem", item);
                return completionItem;
            },
            triggerCharacters: triggerCharacters
        });
    }

    async registerSignatureHelpProvider(blazorCallback: IBlazorInteropObject) {
        this.logger.debug(`Registering new signature help provider: ${blazorCallback}`);
        const languageId: string = await blazorCallback.invokeMethodAsync("GetLanguage");
        const triggerCharacters: string[] = await blazorCallback.invokeMethodAsync("GetTriggerCharacters");
        const retriggerCharacters: string[] = await blazorCallback.invokeMethodAsync("GetRetriggerCharacters");
        this.logger.debug(`Signature help provider language: ${languageId}, trigger characters: ${JSON.stringify(triggerCharacters)}, retrigger characters: ${JSON.stringify(retriggerCharacters)}, `);

        monaco.languages.registerSignatureHelpProvider(languageId, {
            provideSignatureHelp: async (model: monaco.editor.ITextModel, position: monaco.Position, token: monaco.CancellationToken, context: monaco.languages.SignatureHelpContext) => {
                this.logger.trace(`Code signature help request: ${model.uri}, position: ${position}, context: ${context}`);
                const signatureHelp: monaco.languages.SignatureHelp = await blazorCallback.invokeMethodAsync("ProvideSignatureHelp", model.uri, context, position);
                if (!signatureHelp) {
                    this.logger.trace(`Code signature help is empty`);
                    return null;
                }
                this.logger.trace(`Code signature help received: ${signatureHelp}`);
                return {
                    value: signatureHelp,
                    dispose: () => {
                    }
                };
            },
            signatureHelpTriggerCharacters: triggerCharacters,
            signatureHelpRetriggerCharacters: retriggerCharacters
        });
    }

    prepareActionDescriptor(actionDescriptor: monaco.editor.IActionDescriptor, blazorCallback: IBlazorInteropObject) {
        const action = actionDescriptor;
        action.run = async (editor: monaco.editor.ICodeEditor) => {
            let model = editor.getModel();
            const caretPosition = editor.getPosition();
            const caretOffset = model.getOffsetAt(caretPosition);
            const editorId = this.editorIdByMonacoEditorId[editor.getId()];
            const args: IMonacoActionArgs = {
                modelUri: model.uri,
                caretPosition: caretPosition,
                caretOffset: caretOffset,
                editorId: editorId
            };
            await blazorCallback.invokeMethodAsync("HandleExecuted", args);
        };
        return action;
    }
}

window['monacoInterop'] = new MonacoInterop();
