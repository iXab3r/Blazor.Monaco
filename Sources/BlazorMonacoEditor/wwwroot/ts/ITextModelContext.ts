import { editor } from 'monaco-editor/esm/vs/editor/editor.api';
import { IBlazorInteropObject } from './IBlazorInteropObject';
import { TextModelEventHandler } from './TextModelEventHandler';

/**
 * Context structure for a text model.
 */
export interface ITextModelContext {
    textModel: editor.ITextModel;
    ownsModel: boolean;
    updating: boolean;
    changeTimer: any;
    changeAnchor?: { dispose(): void };
    eventHandler?: TextModelEventHandler;
    eventSink?: IBlazorInteropObject;
}
