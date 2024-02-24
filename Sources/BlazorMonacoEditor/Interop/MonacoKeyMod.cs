using System;

namespace BlazorMonacoEditor.Interop;

[Flags]
public enum MonacoKeyMod : int
{
    None = 0,
    // Define modifier keys following Monaco's KeyMod values
    CtrlCmd = 1 << 11, // Either Ctrl on Windows/Unix or Cmd on Mac
    Shift = 1 << 10,   // Shift key
    Alt = 1 << 9,      // Alt key
    WinCtrl = 1 << 8,  // Windows key or Ctrl key
}