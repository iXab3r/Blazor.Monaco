using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.Extensions.Caching.Memory;
using CompletionContext = BlazorMonacoEditor.Interop.CompletionContext;
using CompletionItem = BlazorMonacoEditor.Interop.CompletionItem;
using CompletionList = BlazorMonacoEditor.Interop.CompletionList;
using RoslynCompletionItem = Microsoft.CodeAnalysis.Completion.CompletionItem;

namespace BlazorMonacoEditor.Roslyn.Services;

public sealed class RoslynCompletionProvider : IRoslynCompletionProvider
{
    private readonly IMemoryCache documentsByUri;
    private readonly ConcurrentDictionary<Workspace, int> workspaces = new();

    public RoslynCompletionProvider()
    {
        documentsByUri = new MemoryCache(new MemoryCacheOptions());
    }

    public async Task<string[]> GetTriggerCharacters()
    {
        return new[] {"."};
    }

    public async Task<string> GetLanguage()
    {
        return "csharp";
    }
    
    public IDisposable AddWorkspace(Workspace workspace)
    {
        lock (workspaces)
        {
            workspaces.AddOrUpdate(workspace, _ => 1, (_, existingReferences) => existingReferences + 1);
        }
        return Disposable.Create(() =>
        {
            lock (workspaces)
            {
                var references = workspaces.AddOrUpdate(workspace, _ => 0, (_, existingReferences) => existingReferences - 1);
                if (references <= 0)
                {
                    workspaces.TryRemove(workspace, out var _);
                }
            }
        });
    }

    private DocumentCacheEntry? FindEntryByUri(Uri? documentUri)
    {
        if (documentUri == null || documentUri.Segments.Length != 3)
        {
            return null;
        }

        if (documentsByUri.TryGetValue(documentUri, out var cachedDocument) && cachedDocument is DocumentCacheEntry documentCacheEntry)
        {
            return documentCacheEntry;
        }

        if (!Guid.TryParse(documentUri.Segments[1].Trim('/', '\\'), out var projectGuid))
        {
            return null;
        }

        if (!Guid.TryParse(documentUri.Segments[2].Trim('/', '\\'), out var documentGuid))
        {
            return null;
        }

        var projectId = ProjectId.CreateFromSerialized(projectGuid);
        var documentId = DocumentId.CreateFromSerialized(projectId, documentGuid);

        // ReSharper disable once InconsistentlySynchronizedField expected
        foreach (var workspace in workspaces.Keys)
        {
            var workspaceDocument = workspace.CurrentSolution.GetDocument(documentId);
            if (workspaceDocument == null)
            {
                continue;
            }

            var completionService = workspaceDocument.Project.Services.GetRequiredService<CompletionService>();
            var result = new DocumentCacheEntry(workspace, documentId, CompletionService: completionService);
            documentsByUri.Set(documentUri, result, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });
            return result;
        }

        return null;
    }

    public async Task<CompletionList?> ProvideCompletionItems(MonacoUri modelUri, CompletionContext completionContext, Position caretPosition, int caretOffset)
    {
        var entry = FindEntryByUri(modelUri.ToUriOrDefault());
        if (entry == null)
        {
            return null;
        }

        var document = entry.Workspace.CurrentSolution.GetDocument(entry.DocumentId);
        if (document == null)
        {
            return null;
        }

        var roslynCompletionList = await entry.CompletionService.GetCompletionsAsync(document, caretOffset).ConfigureAwait(false);
        var orderedRoslynCompletions = roslynCompletionList.ItemsList
            .Where(roslynCompletion => roslynCompletion is not {IsComplexTextEdit: true, InlineDescription.Length: > 0})
            .OrderByDescending(x => x.Rules.MatchPriority)
            .ThenBy(x => x.SortText);

        var monacoSuggestions = new List<CompletionItem>();
        foreach (var roslynCompletion in orderedRoslynCompletions)
        {
            var insertText = roslynCompletion.Properties.TryGetValue("InsertionText", out var text) ? text : roslynCompletion.DisplayText;
            var monacoCompletionItem = new CompletionItem()
            {
                Label = new CompletionItemLabel
                {
                    Label = roslynCompletion.DisplayText,
                },
                InsertText = insertText,
                Kind = CompletionItemKind.Interface
            };
            monacoSuggestions.Add(monacoCompletionItem);
        }

        var monacoCompletionList = new CompletionList()
        {
            Incomplete = false,
            Suggestions = monacoSuggestions
        };
        return monacoCompletionList;
    }

    private static string BuildDocumentation(RoslynCompletionItem roslynCompletion)
    {
        var documentationBuilder = new StringBuilder();
        documentationBuilder.Append($"**{roslynCompletion.DisplayText}**\n");
        if (!string.IsNullOrEmpty(roslynCompletion.DisplayTextPrefix))
        {
            documentationBuilder.AppendLine($"{nameof(roslynCompletion.DisplayTextPrefix)}: {roslynCompletion.DisplayTextPrefix}");
        }

        if (!string.IsNullOrEmpty(roslynCompletion.DisplayTextSuffix))
        {
            documentationBuilder.AppendLine($"{nameof(roslynCompletion.DisplayTextSuffix)}: {roslynCompletion.DisplayTextSuffix}");
        }

        if (roslynCompletion.IsComplexTextEdit)
        {
            documentationBuilder.AppendLine($"{nameof(roslynCompletion.IsComplexTextEdit)}: {roslynCompletion.IsComplexTextEdit}");
        }

        if (!string.IsNullOrEmpty(roslynCompletion.InlineDescription))
        {
            documentationBuilder.AppendLine($"{nameof(roslynCompletion.InlineDescription)}: {roslynCompletion.InlineDescription}");
        }
        if (roslynCompletion.Rules.MatchPriority != default)
        {
            documentationBuilder.AppendLine($"{nameof(roslynCompletion.Rules.MatchPriority)}: {roslynCompletion.Rules.MatchPriority}");
        }

        if (roslynCompletion.Tags.Length > 0)
        {
            documentationBuilder.AppendLine($"Tags: {string.Join(", ", roslynCompletion.Tags)}");
        }

        if (roslynCompletion.Properties.Count > 0)
        {
            foreach (var completionProperty in roslynCompletion.Properties)
            {
                documentationBuilder.AppendLine($"{completionProperty.Key}: {completionProperty.Value}");
            }
        }

        return documentationBuilder.ToString();
    }

    private static string GetDisplayText(RoslynCompletionItem item)
    {
        var text = item.DisplayTextPrefix + item.DisplayText + item.DisplayTextSuffix;
        if (item.Tags.Length > 0)
        {
            var classification = TextTagToClassificationTypeName(item.Tags.First());
            var prefix = GetCompletionItemSymbolPrefix(classification, useUnicode: true);
            return $"{prefix}{text}";
        }

        return text;
    }

    public static string? TextTagToClassificationTypeName(string taggedTextTag)
    {
        return taggedTextTag switch
        {
            TextTags.Keyword => ClassificationTypeNames.Keyword,
            TextTags.Class => ClassificationTypeNames.ClassName,
            TextTags.Delegate => ClassificationTypeNames.DelegateName,
            TextTags.Enum => ClassificationTypeNames.EnumName,
            TextTags.Interface => ClassificationTypeNames.InterfaceName,
            TextTags.Module => ClassificationTypeNames.ModuleName,
            TextTags.Struct or "Structure" => ClassificationTypeNames.StructName,
            TextTags.TypeParameter => ClassificationTypeNames.TypeParameterName,
            TextTags.Field => ClassificationTypeNames.FieldName,
            TextTags.Event => ClassificationTypeNames.EventName,
            TextTags.Label => ClassificationTypeNames.LabelName,
            TextTags.Local => ClassificationTypeNames.LocalName,
            TextTags.Method => ClassificationTypeNames.MethodName,
            TextTags.Namespace => ClassificationTypeNames.NamespaceName,
            TextTags.Parameter => ClassificationTypeNames.ParameterName,
            TextTags.Property => ClassificationTypeNames.PropertyName,
            TextTags.ExtensionMethod => ClassificationTypeNames.ExtensionMethodName,
            TextTags.EnumMember => ClassificationTypeNames.EnumMemberName,
            TextTags.Constant => ClassificationTypeNames.ConstantName,
            TextTags.Alias or TextTags.Assembly or TextTags.ErrorType or TextTags.RangeVariable => ClassificationTypeNames.Identifier,
            TextTags.NumericLiteral => ClassificationTypeNames.NumericLiteral,
            TextTags.StringLiteral => ClassificationTypeNames.StringLiteral,
            TextTags.Space or TextTags.LineBreak => ClassificationTypeNames.WhiteSpace,
            TextTags.Operator => ClassificationTypeNames.Operator,
            TextTags.Punctuation => ClassificationTypeNames.Punctuation,
            TextTags.AnonymousTypeIndicator or TextTags.Text => ClassificationTypeNames.Text,
            TextTags.Record => ClassificationTypeNames.RecordClassName,
            TextTags.RecordStruct => ClassificationTypeNames.RecordStructName,
            _ => null,
        };
    }

    public static string GetCompletionItemSymbolPrefix(string? classification, bool useUnicode)
    {
        Span<char> prefix = stackalloc char[3];
        if (useUnicode)
        {
            var symbol = classification switch
            {
                ClassificationTypeNames.Keyword => "🔑",
                ClassificationTypeNames.MethodName or ClassificationTypeNames.ExtensionMethodName => "🟣",
                ClassificationTypeNames.PropertyName => "🟡",
                ClassificationTypeNames.FieldName or ClassificationTypeNames.ConstantName or ClassificationTypeNames.EnumMemberName => "🔵",
                ClassificationTypeNames.EventName => "⚡",
                ClassificationTypeNames.ClassName or ClassificationTypeNames.RecordClassName => "🟨",
                ClassificationTypeNames.InterfaceName => "🔷",
                ClassificationTypeNames.StructName or ClassificationTypeNames.RecordStructName => "🟦",
                ClassificationTypeNames.EnumName => "🟧",
                ClassificationTypeNames.DelegateName => "💼",
                ClassificationTypeNames.NamespaceName => "⬜",
                ClassificationTypeNames.TypeParameterName => "⬛",
                _ => "⚫",
            };
            Debug.Assert(symbol.Length <= prefix.Length);
            symbol.CopyTo(prefix);
            prefix[symbol.Length] = ' ';
            prefix = prefix[..(symbol.Length + 1)];
            return prefix.ToString();
        }
        else
        {
            return "";
        }
    }

    private sealed record DocumentCacheEntry(Workspace Workspace, DocumentId DocumentId, CompletionService CompletionService);
}