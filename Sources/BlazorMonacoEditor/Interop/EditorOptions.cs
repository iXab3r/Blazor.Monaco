using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMonacoEditor.Interop;

public sealed record EditorOptions
{
    /// <summary>
    /// Indicates whether this editor is used inside a diff editor.
    /// </summary>
    [JsonPropertyName("inDiffEditor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? InDiffEditor { get; set; }

    /// <summary>
    /// The aria label for the editor's textarea (when it is focused).
    /// </summary>
    [JsonPropertyName("ariaLabel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Whether the aria-required attribute should be set on the editor's textarea.
    /// </summary>
    [JsonPropertyName("ariaRequired")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? AriaRequired { get; set; }

    /// <summary>
    /// Control whether a screen reader announces inline suggestion content immediately.
    /// </summary>
    [JsonPropertyName("screenReaderAnnounceInlineSuggestion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ScreenReaderAnnounceInlineSuggestion { get; set; }

    /// <summary>
    /// The `tabindex` property of the editor's textarea.
    /// </summary>
    [JsonPropertyName("tabIndex")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? TabIndex { get; set; }

    /// <summary>
    /// Render vertical lines at the specified columns. Defaults to an empty array.
    /// </summary>
    [JsonPropertyName("rulers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<RulerOption>? Rulers { get; set; } 

    /// <summary>
    /// A string containing the word separators used when doing word navigation.
    /// Defaults to `~!@#$%^&*()-=+[{]}\\|;:\'",.<>/?
    /// </summary>
    [JsonPropertyName("wordSeparators")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? WordSeparators { get; set; }

    /// <summary>
    /// Enable Linux primary clipboard. Defaults to true.
    /// </summary>
    [JsonPropertyName("selectionClipboard")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? SelectionClipboard { get; set; }

    /// <summary>
    /// Control the rendering of line numbers. Can be 'on', 'off', 'relative', 'interval', or a function.
    /// </summary>
    [JsonPropertyName("lineNumbers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? LineNumbers { get; set; } // This could be an enum or delegate type based on your implementation

    /// <summary>
    /// Controls the minimal number of visible leading and trailing lines surrounding the cursor.
    /// Defaults to 0.
    /// </summary>
    [JsonPropertyName("cursorSurroundingLines")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? CursorSurroundingLines { get; set; }

    /// <summary>
    /// Controls when `cursorSurroundingLines` should be enforced. Can be 'default' or 'all'.
    /// Defaults to `default`, `cursorSurroundingLines` is not enforced when cursor position is changed by mouse.
    /// </summary>
    [JsonPropertyName("cursorSurroundingLinesStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? CursorSurroundingLinesStyle { get; set; }

    /// <summary>
    /// Render the last line number when the file ends with a newline. Can be 'on', 'off', or 'dimmed'.
    /// Defaults to 'on' for Windows and macOS and 'dimmed' for Linux.
    /// </summary>
    [JsonPropertyName("renderFinalNewline")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? RenderFinalNewline { get; set; }

    /// <summary>
    /// Remove unusual line terminators like LINE SEPARATOR (LS), PARAGRAPH SEPARATOR (PS). Can be 'auto', 'off', or 'prompt'.
    /// Defaults to 'prompt'.
    /// </summary>
    [JsonPropertyName("unusualLineTerminators")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? UnusualLineTerminators { get; set; }

    /// <summary>
    /// Should the corresponding line be selected when clicking on the line number? Defaults to true.
    /// </summary>
    [JsonPropertyName("selectOnLineNumbers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? SelectOnLineNumbers { get; set; }

    /// <summary>
    /// Control the width of line numbers, by reserving horizontal space for rendering at least an amount of digits.
    /// Defaults to 5.
    /// </summary>
    [JsonPropertyName("lineNumbersMinChars")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? LineNumbersMinChars { get; set; }

    /// <summary>
    /// Enable the rendering of the glyph margin. Defaults to true in vscode and to false in monaco-editor.
    /// </summary>
    [JsonPropertyName("glyphMargin")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? GlyphMargin { get; set; }

    /// <summary>
    /// The width reserved for line decorations (in px). Line decorations are placed between line numbers and the editor content.
    /// You can pass in a string in the format floating point followed by "ch". e.g. 1.3ch.
    /// Defaults to 10.
    /// </summary>
    [JsonPropertyName("lineDecorationsWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? LineDecorationsWidth { get; set; } 

    /// <summary>
    /// When revealing the cursor, a virtual padding (px) is added to the cursor, turning it into a rectangle.
    /// This virtual padding ensures that the cursor gets revealed before hitting the edge of the viewport.
    /// Defaults to 30 (px).
    /// </summary>
    [JsonPropertyName("revealHorizontalRightPadding")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? RevealHorizontalRightPadding { get; set; }

    /// <summary>
    /// Render the editor selection with rounded borders. Defaults to true.
    /// </summary>
    [JsonPropertyName("roundedSelection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? RoundedSelection { get; set; }

    /// <summary>
    /// Class name to be added to the editor.
    /// </summary>
    [JsonPropertyName("extraEditorClassName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ExtraEditorClassName { get; set; }

    /// <summary>
    /// Should the editor be read-only? See also `domReadOnly`. Defaults to false.
    /// </summary>
    [JsonPropertyName("readOnly")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ReadOnly { get; set; }

    /// <summary>
    /// The message to display when the editor is readonly.
    /// </summary>
    [JsonPropertyName("readOnlyMessage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public MarkdownString? ReadOnlyMessage { get; set; } 

    /// <summary>
    /// Should the textarea used for input use the DOM `readonly` attribute? Defaults to false.
    /// </summary>
    [JsonPropertyName("domReadOnly")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? DomReadOnly { get; set; }

    /// <summary>
    /// Enable linked editing. Defaults to false.
    /// </summary>
    [JsonPropertyName("linkedEditing")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? LinkedEditing { get; set; }

    /// <summary>
    /// Deprecated, use linkedEditing instead.
    /// </summary>
    [JsonPropertyName("renameOnType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? RenameOnType { get; set; }

    /// <summary>
    /// Should the editor render validation decorations? Can be 'editable', 'on', or 'off'.
    /// Defaults to 'editable'.
    /// </summary>
    [JsonPropertyName("renderValidationDecorations")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? RenderValidationDecorations { get; set; }

    /// <summary>
    /// Control the behavior and rendering of the scrollbars.
    /// </summary>
    [JsonPropertyName("scrollbar")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public EditorScrollbarOptions? Scrollbar { get; set; }

    /// <summary>
    /// Control the behavior of sticky scroll options.
    /// </summary>
    [JsonPropertyName("stickyScroll")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public EditorStickyScrollOptions? StickyScroll { get; set; }

    /// <summary>
    /// Control the behavior and rendering of the minimap.
    /// </summary>
    [JsonPropertyName("minimap")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public EditorMinimapOptions? Minimap { get; set; }

    /// <summary>
    /// Control the behavior of the find widget.
    /// </summary>
    [JsonPropertyName("find")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public EditorFindOptions? Find { get; set; }

    /// <summary>
    /// Display overflow widgets as `fixed`. Defaults to `false`.
    /// </summary>
    [JsonPropertyName("fixedOverflowWidgets")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? FixedOverflowWidgets { get; set; }
    
     /// <summary>
    /// The number of vertical lanes the overview ruler should render. Defaults to 3.
    /// </summary>
    [JsonPropertyName("overviewRulerLanes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? OverviewRulerLanes { get; set; }

    /// <summary>
    /// Controls if a border should be drawn around the overview ruler. Defaults to true.
    /// </summary>
    [JsonPropertyName("overviewRulerBorder")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? OverviewRulerBorder { get; set; }

    /// <summary>
    /// Control the cursor animation style. Possible values are 'blink', 'smooth', 'phase', 'expand', and 'solid'. Defaults to 'blink'.
    /// </summary>
    [JsonPropertyName("cursorBlinking")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? CursorBlinking { get; set; }

    /// <summary>
    /// Zoom the font in the editor when using the mouse wheel in combination with holding Ctrl. Defaults to false.
    /// </summary>
    [JsonPropertyName("mouseWheelZoom")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? MouseWheelZoom { get; set; }

    /// <summary>
    /// Control the mouse pointer style, either 'text', 'default', or 'copy'. Defaults to 'text'.
    /// </summary>
    [JsonPropertyName("mouseStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MouseStyle { get; set; }

    /// <summary>
    /// Enable smooth caret animation. Defaults to 'off'.
    /// </summary>
    [JsonPropertyName("cursorSmoothCaretAnimation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? CursorSmoothCaretAnimation { get; set; }

    /// <summary>
    /// Control the cursor style, either 'block', 'line', 'underline', 'line-thin', 'block-outline', or 'underline-thin'. Defaults to 'line'.
    /// </summary>
    [JsonPropertyName("cursorStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? CursorStyle { get; set; }

    /// <summary>
    /// Control the width of the cursor when cursorStyle is set to 'line'.
    /// </summary>
    [JsonPropertyName("cursorWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? CursorWidth { get; set; }

    /// <summary>
    /// Enable font ligatures. Can be a boolean or a string. Defaults to false.
    /// </summary>
    [JsonPropertyName("fontLigatures")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public object? FontLigatures { get; set; }  // Can be bool or string

    /// <summary>
    /// Enable font variations. Can be a boolean or a string. Defaults to false.
    /// </summary>
    [JsonPropertyName("fontVariations")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public object? FontVariations { get; set; }  // Can be bool or string

    /// <summary>
    /// Controls whether to use default color decorations or not using the default document color provider.
    /// </summary>
    [JsonPropertyName("defaultColorDecorators")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? DefaultColorDecorators { get; set; }

    /// <summary>
    /// Disable the use of `transform: translate3d(0px, 0px, 0px)` for the editor margin and lines layers. Defaults to false.
    /// </summary>
    [JsonPropertyName("disableLayerHinting")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? DisableLayerHinting { get; set; }

    /// <summary>
    /// Disable the optimizations for monospace fonts. Defaults to false.
    /// </summary>
    [JsonPropertyName("disableMonospaceOptimizations")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? DisableMonospaceOptimizations { get; set; }

    /// <summary>
    /// Should the cursor be hidden in the overview ruler. Defaults to false.
    /// </summary>
    [JsonPropertyName("hideCursorInOverviewRuler")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? HideCursorInOverviewRuler { get; set; }

    /// <summary>
    /// Enable that scrolling can go one screen size after the last line. Defaults to true.
    /// </summary>
    [JsonPropertyName("scrollBeyondLastLine")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ScrollBeyondLastLine { get; set; }

    /// <summary>
    /// Enable that scrolling can go beyond the last column by a number of columns. Defaults to 5.
    /// </summary>
    [JsonPropertyName("scrollBeyondLastColumn")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? ScrollBeyondLastColumn { get; set; }

    /// <summary>
    /// Enable that the editor animates scrolling to a position. Defaults to false.
    /// </summary>
    [JsonPropertyName("smoothScrolling")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? SmoothScrolling { get; set; }

    /// <summary>
    /// Enable that the editor will install a ResizeObserver to check if its container dom node size has changed. Defaults to false.
    /// </summary>
    [JsonPropertyName("automaticLayout")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? AutomaticLayout { get; set; }

    /// <summary>
    /// Control the wrapping of the editor. Can be 'off', 'on', 'wordWrapColumn', or 'bounded'. Defaults to "off".
    /// </summary>
    [JsonPropertyName("wordWrap")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? WordWrap { get; set; }

    /// <summary>
    /// Override the `wordWrap` setting. Can be 'off', 'on', or 'inherit'.
    /// </summary>
    [JsonPropertyName("wordWrapOverride1")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? WordWrapOverride1 { get; set; }

    /// <summary>
    /// Override the `wordWrapOverride1` setting. Can be 'off', 'on', or 'inherit'.
    /// </summary>
    [JsonPropertyName("wordWrapOverride2")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? WordWrapOverride2 { get; set; }

    /// <summary>
    /// Control the wrapping of the editor. When `wordWrap` = "wordWrapColumn", the lines will wrap at `wordWrapColumn`. When `wordWrap` = "bounded", the lines will wrap at min(viewport width, wordWrapColumn). Defaults to 80.
    /// </summary>
    [JsonPropertyName("wordWrapColumn")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? WordWrapColumn { get; set; }

    /// <summary>
    /// Control indentation of wrapped lines. Can be: 'none', 'same', 'indent', or 'deepIndent'. Defaults to 'same' in vscode and to 'none' in monaco-editor.
    /// </summary>
    [JsonPropertyName("wrappingIndent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? WrappingIndent { get; set; }

    /// <summary>
    /// Controls the wrapping strategy to use. Defaults to 'simple'.
    /// </summary>
    [JsonPropertyName("wrappingStrategy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? WrappingStrategy { get; set; }

    /// <summary>
    /// Configure word wrapping characters. A break will be introduced before these characters.
    /// </summary>
    [JsonPropertyName("wordWrapBreakBeforeCharacters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? WordWrapBreakBeforeCharacters { get; set; }

    /// <summary>
    /// Configure word wrapping characters. A break will be introduced after these characters.
    /// </summary>
    [JsonPropertyName("wordWrapBreakAfterCharacters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? WordWrapBreakAfterCharacters { get; set; }

    /// <summary>
    /// Sets whether line breaks appear wherever the text would otherwise overflow its content box. Can be 'normal' or 'keepAll'.
    /// </summary>
    [JsonPropertyName("wordBreak")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? WordBreak { get; set; }

    /// <summary>
    /// Performance guard: Stop rendering a line after x characters. Use -1 to never stop rendering. Defaults to 10000.
    /// </summary>
    [JsonPropertyName("stopRenderingLineAfter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? StopRenderingLineAfter { get; set; }
    
    
    /// <summary>
    /// Configure the editor's hover.
    /// </summary>
    [JsonPropertyName("hover")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public EditorHoverOptions? Hover { get; set; }

    /// <summary>
    /// Enable detecting links and making them clickable. Defaults to true.
    /// </summary>
    [JsonPropertyName("links")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Links { get; set; }

    /// <summary>
    /// Enable inline color decorators and color picker rendering.
    /// </summary>
    [JsonPropertyName("colorDecorators")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ColorDecorators { get; set; }

    /// <summary>
    /// Controls what is the condition to spawn a color picker from a color decorator.
    /// Possible values: 'clickAndHover', 'click', 'hover'.
    /// </summary>
    [JsonPropertyName("colorDecoratorsActivatedOn")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ColorDecoratorsActivatedOn { get; set; }

    /// <summary>
    /// Controls the max number of color decorators that can be rendered in an editor at once.
    /// </summary>
    [JsonPropertyName("colorDecoratorsLimit")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? ColorDecoratorsLimit { get; set; }

    /// <summary>
    /// Control the behaviour of comments in the editor.
    /// </summary>
    [JsonPropertyName("comments")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public EditorCommentsOptions? Comments { get; set; }

    /// <summary>
    /// Enable custom contextmenu. Defaults to true.
    /// </summary>
    [JsonPropertyName("contextmenu")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Contextmenu { get; set; }

    /// <summary>
    /// A multiplier to be used on the `deltaX` and `deltaY` of mouse wheel scroll events. Defaults to 1.
    /// </summary>
    [JsonPropertyName("mouseWheelScrollSensitivity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public double? MouseWheelScrollSensitivity { get; set; }

    /// <summary>
    /// FastScrolling multiplier speed when pressing `Alt`. Defaults to 5.
    /// </summary>
    [JsonPropertyName("fastScrollSensitivity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public double? FastScrollSensitivity { get; set; }

    /// <summary>
    /// Enable that the editor scrolls only the predominant axis. Prevents horizontal drift when scrolling vertically on a trackpad. Defaults to true.
    /// </summary>
    [JsonPropertyName("scrollPredominantAxis")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ScrollPredominantAxis { get; set; }

    /// <summary>
    /// Enable that the selection with the mouse and keys is doing column selection. Defaults to false.
    /// </summary>
    [JsonPropertyName("columnSelection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ColumnSelection { get; set; }

    /// <summary>
    /// The modifier to be used to add multiple cursors with the mouse. Defaults to 'alt'. Possible values: 'ctrlCmd', 'alt'.
    /// </summary>
    [JsonPropertyName("multiCursorModifier")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MultiCursorModifier { get; set; }

    /// <summary>
    /// Merge overlapping selections. Defaults to true.
    /// </summary>
    [JsonPropertyName("multiCursorMergeOverlapping")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? MultiCursorMergeOverlapping { get; set; }

    /// <summary>
    /// Configure the behaviour when pasting a text with the line count equal to the cursor count. Defaults to 'spread'. Possible values: 'spread', 'full'.
    /// </summary>
    [JsonPropertyName("multiCursorPaste")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MultiCursorPaste { get; set; }

    /// <summary>
    /// Controls the max number of text cursors that can be in an active editor at once.
    /// </summary>
    [JsonPropertyName("multiCursorLimit")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? MultiCursorLimit { get; set; }

    /// <summary>
    /// Configure the editor's accessibility support. Defaults to 'auto'. Possible values: 'auto', 'off', 'on'.
    /// </summary>
    [JsonPropertyName("accessibilitySupport")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AccessibilitySupport { get; set; }

    /// <summary>
    /// Controls the number of lines in the editor that can be read out by a screen reader.
    /// </summary>
    [JsonPropertyName("accessibilityPageSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? AccessibilityPageSize { get; set; }

    /// <summary>
    /// Suggest options.
    /// </summary>
    [JsonPropertyName("suggest")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public SuggestOptions? Suggest { get; set; }

    /// <summary>
    /// Inline suggest options.
    /// </summary>
    [JsonPropertyName("inlineSuggest")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public InlineSuggestOptions? InlineSuggest { get; set; }

    /// <summary>
    /// Smart select options.
    /// </summary>
    [JsonPropertyName("smartSelect")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public SmartSelectOptions? SmartSelect { get; set; }

    /// <summary>
    /// Go to location options.
    /// </summary>
    [JsonPropertyName("gotoLocation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public GotoLocationOptions? GotoLocation { get; set; }

    /// <summary>
    /// Enable quick suggestions (shadow suggestions). Defaults to true. Can be boolean or IQuickSuggestionsOptions.
    /// </summary>
    [JsonPropertyName("quickSuggestions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public object? QuickSuggestions { get; set; } // Can be bool or IQuickSuggestionsOptions

    /// <summary>
    /// Quick suggestions show delay (in ms). Defaults to 10 (ms).
    /// </summary>
    [JsonPropertyName("quickSuggestionsDelay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? QuickSuggestionsDelay { get; set; }

    /// <summary>
    /// Controls the spacing around the editor.
    /// </summary>
    [JsonPropertyName("padding")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public EditorPaddingOptions? Padding { get; set; }
    
    /// <summary>
    /// Parameter hint options.
    /// </summary>
    [JsonPropertyName("parameterHints")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public EditorParameterHintOptions? ParameterHints { get; set; }

    /// <summary>
    /// Options for auto closing brackets. Defaults to language defined behavior.
    /// 'always' | 'languageDefined' | 'beforeWhitespace' | 'never'
    /// </summary>
    [JsonPropertyName("autoClosingBrackets")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AutoClosingBrackets { get; set; }

    /// <summary>
    /// Options for auto closing comments. Defaults to language defined behavior.
    /// 'always' | 'languageDefined' | 'beforeWhitespace' | 'never'
    /// </summary>
    [JsonPropertyName("autoClosingComments")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AutoClosingComments { get; set; }

    /// <summary>
    /// Options for auto closing quotes. Defaults to language defined behavior.
    /// 'always' | 'languageDefined' | 'beforeWhitespace' | 'never'
    /// </summary>
    [JsonPropertyName("autoClosingQuotes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AutoClosingQuotes { get; set; }

    /// <summary>
    /// Options for pressing backspace near quotes or bracket pairs.
    /// 'always' | 'auto' | 'never'
    /// </summary>
    [JsonPropertyName("autoClosingDelete")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AutoClosingDelete { get; set; }

    /// <summary>
    /// Options for typing over closing quotes or brackets.
    /// 'always' | 'auto' | 'never'
    /// </summary>
    [JsonPropertyName("autoClosingOvertype")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AutoClosingOvertype { get; set; }

    /// <summary>
    /// Options for auto surrounding. Defaults to always allowing auto surrounding.
    /// 'languageDefined' | 'quotes' | 'brackets' | 'never'
    /// </summary>
    [JsonPropertyName("autoSurround")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AutoSurround { get; set; }

    /// <summary>
    /// Controls whether the editor should automatically adjust the indentation when users type, paste, move or indent lines. 
    /// Defaults to advanced. Possible values: 'none', 'keep', 'brackets', 'advanced', 'full'.
    /// </summary>
    [JsonPropertyName("autoIndent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AutoIndent { get; set; }

    /// <summary>
    /// Emulate selection behaviour of tab characters when using spaces for indentation.
    /// This means selection will stick to tab stops.
    /// </summary>
    [JsonPropertyName("stickyTabStops")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? StickyTabStops { get; set; }

    /// <summary>
    /// Enable format on type. Defaults to false.
    /// </summary>
    [JsonPropertyName("formatOnType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? FormatOnType { get; set; }

    /// <summary>
    /// Enable format on paste. Defaults to false.
    /// </summary>
    [JsonPropertyName("formatOnPaste")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? FormatOnPaste { get; set; }

    /// <summary>
    /// Controls if the editor should allow to move selections via drag and drop. Defaults to false.
    /// </summary>
    [JsonPropertyName("dragAndDrop")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? DragAndDrop { get; set; }

    /// <summary>
    /// Enable the suggestion box to pop-up on trigger characters. Defaults to true.
    /// </summary>
    [JsonPropertyName("suggestOnTriggerCharacters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? SuggestOnTriggerCharacters { get; set; }

    /// <summary>
    /// Accept suggestions on ENTER. Defaults to 'on'. Possible values: 'on', 'smart', 'off'.
    /// </summary>
    [JsonPropertyName("acceptSuggestionOnEnter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AcceptSuggestionOnEnter { get; set; }

    /// <summary>
    /// Accept suggestions on provider defined characters. Defaults to true.
    /// </summary>
    [JsonPropertyName("acceptSuggestionOnCommitCharacter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? AcceptSuggestionOnCommitCharacter { get; set; }

    /// <summary>
    /// Enable snippet suggestions. Default to 'true'. Possible values: 'top', 'bottom', 'inline', 'none'.
    /// </summary>
    [JsonPropertyName("snippetSuggestions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SnippetSuggestions { get; set; }

    /// <summary>
    /// Copying without a selection copies the current line.
    /// </summary>
    [JsonPropertyName("emptySelectionClipboard")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? EmptySelectionClipboard { get; set; }

    /// <summary>
    /// Syntax highlighting is copied.
    /// </summary>
    [JsonPropertyName("copyWithSyntaxHighlighting")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CopyWithSyntaxHighlighting { get; set; }

    /// <summary>
    /// The history mode for suggestions. Possible values: 'first', 'recentlyUsed', 'recentlyUsedByPrefix'.
    /// </summary>
    [JsonPropertyName("suggestSelection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SuggestSelection { get; set; }

    /// <summary>
    /// The font size for the suggest widget. Defaults to the editor font size.
    /// </summary>
    [JsonPropertyName("suggestFontSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? SuggestFontSize { get; set; }

    /// <summary>
    /// The line height for the suggest widget. Defaults to the editor line height.
    /// </summary>
    [JsonPropertyName("suggestLineHeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? SuggestLineHeight { get; set; }

    /// <summary>
    /// Enable tab completion. Possible values: 'on', 'off', 'onlySnippets'.
    /// </summary>
    [JsonPropertyName("tabCompletion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? TabCompletion { get; set; }

    /// <summary>
    /// Enable selection highlight. Defaults to true.
    /// </summary>
    [JsonPropertyName("selectionHighlight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? SelectionHighlight { get; set; }

    /// <summary>
    /// Enable semantic occurrences highlight. Defaults to 'singleFile'.
    /// Possible values: 'off', 'singleFile', 'multiFile'.
    /// </summary>
    [JsonPropertyName("occurrencesHighlight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? OccurrencesHighlight { get; set; }

    /// <summary>
    /// Show code lens. Defaults to true.
    /// </summary>
    [JsonPropertyName("codeLens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CodeLens { get; set; }

    /// <summary>
    /// Code lens font family. Defaults to editor font family.
    /// </summary>
    [JsonPropertyName("codeLensFontFamily")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? CodeLensFontFamily { get; set; }

    /// <summary>
    /// Code lens font size. Default to 90% of the editor font size.
    /// </summary>
    [JsonPropertyName("codeLensFontSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? CodeLensFontSize { get; set; }
    
    /// <summary>
    /// Control the behavior and rendering of the code action lightbulb.
    /// </summary>
    [JsonPropertyName("lightbulb")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public EditorLightbulbOptions? Lightbulb { get; set; }

    /// <summary>
    /// Timeout for running code actions on save.
    /// </summary>
    [JsonPropertyName("codeActionsOnSaveTimeout")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? CodeActionsOnSaveTimeout { get; set; }

    /// <summary>
    /// Enable code folding. Defaults to true.
    /// </summary>
    [JsonPropertyName("folding")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Folding { get; set; }

    /// <summary>
    /// Selects the folding strategy. 'auto' uses the strategies contributed for the current document, 
    /// 'indentation' uses the indentation based folding strategy. Defaults to 'auto'.
    /// </summary>
    [JsonPropertyName("foldingStrategy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? FoldingStrategy { get; set; }

    /// <summary>
    /// Enable highlight for folded regions. Defaults to true.
    /// </summary>
    [JsonPropertyName("foldingHighlight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? FoldingHighlight { get; set; }

    /// <summary>
    /// Auto fold imports folding regions. Defaults to true.
    /// </summary>
    [JsonPropertyName("foldingImportsByDefault")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? FoldingImportsByDefault { get; set; }

    /// <summary>
    /// Maximum number of foldable regions. Defaults to 5000.
    /// </summary>
    [JsonPropertyName("foldingMaximumRegions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? FoldingMaximumRegions { get; set; }

    /// <summary>
    /// Controls whether the fold actions in the gutter stay always visible or hide unless the mouse is over the gutter. 
    /// Defaults to 'mouseover'. Possible values: 'always', 'never', 'mouseover'.
    /// </summary>
    [JsonPropertyName("showFoldingControls")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ShowFoldingControls { get; set; }

    /// <summary>
    /// Controls whether clicking on the empty content after a folded line will unfold the line. Defaults to false.
    /// </summary>
    [JsonPropertyName("unfoldOnClickAfterEndOfLine")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? UnfoldOnClickAfterEndOfLine { get; set; }

    /// <summary>
    /// Enable highlighting of matching brackets. Defaults to 'always'. Possible values: 'never', 'near', 'always'.
    /// </summary>
    [JsonPropertyName("matchBrackets")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MatchBrackets { get; set; }

    /// <summary>
    /// Enable experimental whitespace rendering. Defaults to 'svg'. Possible values: 'svg', 'font', 'off'.
    /// </summary>
    [JsonPropertyName("experimentalWhitespaceRendering")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ExperimentalWhitespaceRendering { get; set; }

    /// <summary>
    /// Enable rendering of whitespace. Defaults to 'selection'. Possible values: 'none', 'boundary', 'selection', 'trailing', 'all'.
    /// </summary>
    [JsonPropertyName("renderWhitespace")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? RenderWhitespace { get; set; }

    /// <summary>
    /// Enable rendering of control characters. Defaults to true.
    /// </summary>
    [JsonPropertyName("renderControlCharacters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? RenderControlCharacters { get; set; }

    /// <summary>
    /// Enable rendering of current line highlight. Defaults to all. Possible values: 'none', 'gutter', 'line', 'all'.
    /// </summary>
    [JsonPropertyName("renderLineHighlight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? RenderLineHighlight { get; set; }

    /// <summary>
    /// Control if the current line highlight should be rendered only the editor is focused. Defaults to false.
    /// </summary>
    [JsonPropertyName("renderLineHighlightOnlyWhenFocus")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? RenderLineHighlightOnlyWhenFocus { get; set; }

    /// <summary>
    /// Inserting and deleting whitespace follows tab stops.
    /// </summary>
    [JsonPropertyName("useTabStops")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? UseTabStops { get; set; }

    /// <summary>
    /// The font family.
    /// </summary>
    [JsonPropertyName("fontFamily")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? FontFamily { get; set; }

    /// <summary>
    /// The font weight.
    /// </summary>
    [JsonPropertyName("fontWeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? FontWeight { get; set; }

    /// <summary>
    /// The font size.
    /// </summary>
    [JsonPropertyName("fontSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? FontSize { get; set; }

    /// <summary>
    /// The line height.
    /// </summary>
    [JsonPropertyName("lineHeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? LineHeight { get; set; }

    /// <summary>
    /// The letter spacing.
    /// </summary>
    [JsonPropertyName("letterSpacing")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public double? LetterSpacing { get; set; }

    /// <summary>
    /// Controls fading out of unused variables.
    /// </summary>
    [JsonPropertyName("showUnused")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowUnused { get; set; }

    /// <summary>
    /// Controls whether to focus the inline editor in the peek widget by default. Defaults to false. 
    /// Possible values: 'tree', 'editor'.
    /// </summary>
    [JsonPropertyName("peekWidgetDefaultFocus")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PeekWidgetDefaultFocus { get; set; }

    /// <summary>
    /// Controls whether the definition link opens element in the peek widget. Defaults to false.
    /// </summary>
    [JsonPropertyName("definitionLinkOpensInPeek")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? DefinitionLinkOpensInPeek { get; set; }

    /// <summary>
    /// Controls strikethrough deprecated variables.
    /// </summary>
    [JsonPropertyName("showDeprecated")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ShowDeprecated { get; set; }

    /// <summary>
    /// Controls whether suggestions allow matches in the middle of the word instead of only at the beginning.
    /// </summary>
    [JsonPropertyName("matchOnWordStartOnly")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? MatchOnWordStartOnly { get; set; }

    
}