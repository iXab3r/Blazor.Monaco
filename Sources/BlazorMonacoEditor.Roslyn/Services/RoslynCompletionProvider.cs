using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Roslyn.Scaffolding;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using CompletionContext = BlazorMonacoEditor.Interop.CompletionContext;
using CompletionItem = BlazorMonacoEditor.Interop.CompletionItem;
using CompletionList = BlazorMonacoEditor.Interop.CompletionList;
using RoslynCompletionItem = Microsoft.CodeAnalysis.Completion.CompletionItem;
using RoslynCompletionList = Microsoft.CodeAnalysis.Completion.CompletionList;

namespace BlazorMonacoEditor.Roslyn.Services;

public sealed class RoslynCompletionProvider : IRoslynCompletionProvider
{
    private readonly ILogger log;
    private readonly IMemoryCache documentsByUri;
    private readonly ConcurrentDictionary<Workspace, int> workspaces = new();
    private long completionRevision;

    public RoslynCompletionProvider(ILoggerFactory logFactory)
    {
        log = logFactory.CreateLogger<RoslynCompletionProvider>();
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

        if (!DocumentIdTypeConverter.TryConvert(documentUri.AbsolutePath, out var documentId) || documentId == null)
        {
            return null;
        }

        // ReSharper disable once InconsistentlySynchronizedField expected
        foreach (var workspace in workspaces.Keys)
        {
            var workspaceDocument = workspace.CurrentSolution.GetDocument(documentId);
            if (workspaceDocument == null)
            {
                continue;
            }

            var completionService = workspaceDocument.Project.Services.GetRequiredService<CompletionService>();
            var result = new DocumentCacheEntry(workspace, documentId, documentUri, CompletionService: completionService);
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
        var revision = Interlocked.Increment(ref completionRevision);
        var documentUri = modelUri.ToUriOrDefault();
        if (documentUri == null)
        {
            return CompletionList.Empty;
        }

        var entry = FindEntryByUri(documentUri);
        if (entry == null)
        {
            return CompletionList.Empty;
        }

        var document = entry.Workspace.CurrentSolution.GetDocument(entry.DocumentId);
        if (document == null)
        {
            return CompletionList.Empty;
        }

        var completionService = entry.CompletionService;

        RoslynCompletionList roslynCompletionList;
        try
        {
            roslynCompletionList = await completionService.GetCompletionsAsync(document, caretOffset).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            log.LogError(e, "Failed to build completion list");
            return CompletionList.Empty;
        }
        documentsByUri.Set(documentUri, entry with {CompletionList = roslynCompletionList, CompletionListRevision = revision}, new MemoryCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });

        var orderedRoslynCompletions = roslynCompletionList.ItemsList
            .Select((x, idx) => new {CompletionItem = x, Idx = idx})
            .Where(roslynCompletion => roslynCompletion.CompletionItem is not {IsComplexTextEdit: true, InlineDescription.Length: > 0})
            .OrderByDescending(x => x.CompletionItem.Rules.MatchPriority);

        var monacoSuggestions = new List<CompletionItem>();
        foreach (var kvp in orderedRoslynCompletions)
        {
            var insertText = kvp.CompletionItem.Properties.TryGetValue("InsertionText", out var text) ? text : kvp.CompletionItem.DisplayText;
            var documentationLink = new RoslynCompletionItemLink(documentUri.ToString(), revision, kvp.Idx);
            var monacoCompletionItem = new CompletionItem()
            {
                Label = new CompletionItemLabel
                {
                    Label = kvp.CompletionItem.DisplayText,
                    Description = string.Join(" ", kvp.CompletionItem.Tags)
                },
                InsertText = insertText,
                Kind = ConvertTextTagToCompletionItemKind(kvp.CompletionItem.Tags),
                Documentation = new MarkdownString()
                {
                    Value = documentationLink.Serialize(),
                }
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

    public async Task<CompletionItem> ResolveCompletionItem(CompletionItem item)
    {
        var serializedLink = item.Documentation?.Value;
        if (string.IsNullOrEmpty(serializedLink))
        {
            return item;
        }

        if (!RoslynCompletionItemLink.TryDeserialize(serializedLink, out var itemLink) || itemLink == null)
        {
            return item;
        }

        if (!Uri.TryCreate(itemLink.DocumentUri, UriKind.RelativeOrAbsolute, out var documentUri))
        {
            return item;
        }

        var entry = FindEntryByUri(documentUri);
        if (entry == null)
        {
            return item;
        }
        
        var document = entry.Workspace.CurrentSolution.GetDocument(entry.DocumentId);
        if (document == null)
        {
            return item;
        }
        
        if (entry.CompletionList == null || entry.CompletionListRevision != itemLink.ListVersion)
        {
            return item;
        }

        var completionItem = entry.CompletionList.ItemsList[itemLink.ItemIdx];
        var documentation = BuildDocumentation(completionItem);

        CompletionDescription? description;
        try
        {
            description = await entry.CompletionService.GetDescriptionAsync(document, completionItem);
        }
        catch (Exception e)
        {
            log.LogError(e, $"Failed to build description for item {item}");
            return item;
        }
        
        return item with {  Documentation = new MarkdownString { Value = $"{documentation}<br>{description?.Text ?? "No documentation"}" }};
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

    public static CompletionItemKind ConvertTextTagToCompletionItemKind(IEnumerable<string> tags)
    {
        foreach (var tag in tags)
        {
            var kind = ConvertTextTagToCompletionItemKind(tag);
            if (kind != null)
            {
                return kind.Value;
            }
        }

        return CompletionItemKind.Text;
    }
    
    public static CompletionItemKind? ConvertTextTagToCompletionItemKind(string textTag)
    {
        return textTag switch
        {
            TextTags.Class => CompletionItemKind.Class,
            TextTags.Delegate => CompletionItemKind.Function,
            TextTags.Enum => CompletionItemKind.Enum,
            TextTags.Event => CompletionItemKind.Event,
            TextTags.Field => CompletionItemKind.Field,
            TextTags.Interface => CompletionItemKind.Interface,
            TextTags.Keyword => CompletionItemKind.Keyword,
            TextTags.Method => CompletionItemKind.Method,
            TextTags.ExtensionMethod => CompletionItemKind.Method,
            TextTags.Module => CompletionItemKind.Module,
            TextTags.Namespace => CompletionItemKind.Module,
            TextTags.Operator => CompletionItemKind.Operator,
            TextTags.Parameter => CompletionItemKind.Value,
            TextTags.Property => CompletionItemKind.Property,
            TextTags.Struct or "Structure" => CompletionItemKind.Struct,
            TextTags.TypeParameter => CompletionItemKind.TypeParameter,
            TextTags.EnumMember => CompletionItemKind.EnumMember,
            TextTags.Constant => CompletionItemKind.Constant,
            TextTags.Record => CompletionItemKind.Class,
            TextTags.RecordStruct => CompletionItemKind.Struct,
            _ => default
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

    private sealed record DocumentCacheEntry(Workspace Workspace, DocumentId DocumentId, Uri DocumentUri, CompletionService CompletionService, RoslynCompletionList? CompletionList = default, long? CompletionListRevision = default);

    private sealed record RoslynCompletionItemLink(string DocumentUri, long ListVersion, int ItemIdx)
    {
        public string Serialize()
        {
            return $"{DocumentUri}|{ListVersion}|{ItemIdx}";
        }

        public static bool TryDeserialize(string serialized, out RoslynCompletionItemLink? result)
        {
            if (string.IsNullOrEmpty(serialized))
            {
                result = null;
                return false;
            }

            var split = serialized.Split("|");
            if (split.Length != 3 || string.IsNullOrEmpty(split[0]) || !long.TryParse(split[1], out var revision) || !int.TryParse(split[2], out var itemIdx))
            {
                result = null;
                return false;
            }

            result = new RoslynCompletionItemLink(split[0], revision, itemIdx);
            return true;
        }
    }
}