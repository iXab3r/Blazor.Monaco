namespace BlazorMonacoEditor.Interop;

public static class MonacoKeybinding
{
    public static int Create(MonacoKeyCode keyCode, MonacoKeyMod modifiers = MonacoKeyMod.None)
    {
        return (int)modifiers | (int)keyCode;
    }
}