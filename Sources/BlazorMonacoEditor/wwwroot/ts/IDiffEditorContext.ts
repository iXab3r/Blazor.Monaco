import * as monaco from 'monaco-editor/esm/vs/editor/editor.api';
import { EditorEventHandler } from './EditorEventHandler';
import { IBlazorInteropObject } from './IBlazorInteropObject';

/**
 * Context structure for a diff editor.
 */
export interface IDiffEditorContext
{
    editorId: string;
    diffEditor: monaco.editor.IStandaloneDiffEditor;
    updating: boolean;
    eventHandler?: EditorEventHandler;
    eventSink?: IBlazorInteropObject;
}
