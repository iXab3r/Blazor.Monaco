import { IBlazorInteropObject } from './IBlazorInteropObject';
import { editor, Uri } from 'monaco-editor/esm/vs/editor/editor.api';

/**
 * Text model event handler.
 */
export class TextModelEventHandler {
    eventSink: IBlazorInteropObject;

    constructor(eventSink: IBlazorInteropObject) {
        this.eventSink = eventSink;
    }

    public handleModelContentChanged(ev: editor.IModelContentChangedEvent)
    {
        this.eventSink.invokeMethodAsync("HandleModelContentChanged", ev);
    }
}