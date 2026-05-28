using System;
using BlazorMonacoEditor.Scaffolding;

namespace BlazorMonacoEditor;

internal static class MonacoModelUri
{
    public static Uri Create(MonacoEditorId editorId, ITextModel textModel, string role)
    {
        if (textModel == null)
        {
            throw new ArgumentNullException(nameof(textModel));
        }

        var path = NormalizePath(textModel.Path, textModel.Id);
        var builder = new UriBuilder("inmemory", editorId.ToString())
        {
            Path = path,
            Query = $"modelId={Uri.EscapeDataString(textModel.Id.ToString())}&role={Uri.EscapeDataString(role ?? "main")}"
        };

        return builder.Uri;
    }

    private static string NormalizePath(string? path, TextModelId textModelId)
    {
        var normalizedPath = string.IsNullOrWhiteSpace(path)
            ? textModelId.ToString()
            : path;

        normalizedPath = normalizedPath
            .Replace('\\', '/')
            .Trim('/');

        return string.IsNullOrWhiteSpace(normalizedPath)
            ? "untitled.txt"
            : normalizedPath;
    }
}
