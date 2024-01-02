using System;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using ReactiveUI;

namespace BlazorMonacoEditor.Playground.Pages;

public class IndexViewModel : ReactiveObject
{
    private SourceText text = SourceText.From("var testCode = 123;");

    public bool IsVisible { get; set; } = true;
    
    public bool ShowLineNumbers { get; set; } = true;
    public bool ShowCodeMap { get; set; } = false;

    public string[] KnownThemes { get; } = { "vs-dark", "vs" };
    public string[] KnownLanguages { get; } = { "csharp", "html" };
    
    public string Theme { get; set; }
    
    public string LanguageId { get; set; }

    public IndexViewModel()
    {
        Theme = KnownThemes.First();
        LanguageId = KnownLanguages.First();
    }

    public SourceText Text
    {
        get => text;
        set
        {
            text = value;
            Console.WriteLine(value);
        }
    }

    public string TextAsString
    {
        get => Text.ToString();
        set
        {
            if (string.Equals(Text.ToString(), value))
            {
                return;
            }
            Text = SourceText.From(value);
        }
    }
}