using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlazorMonacoEditor.Scaffolding;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using Shouldly;

namespace BlazorMonacoEditor.Tests;

[TestFixture]
public sealed class MonacoModelUriFixture
{
    /// <summary>
    /// WHAT: Monaco model URIs should keep the source path extension while still being unique per text model and diff role.
    /// HOW: Build two URIs for the same file path with different model identifiers and assert that path and query both carry the expected identity.
    /// </summary>
    [Test]
    public void ShouldPreservePathExtensionAndKeepModelIdentityInQuery()
    {
        // Given
        var editorId = new MonacoEditorId("editor-a");
        var firstModel = new TestTextModel(new TextModelId("model-a"), @"C:\Scripts\Bot.csx");
        var secondModel = new TestTextModel(new TextModelId("model-b"), @"C:\Scripts\Bot.csx");

        // When
        var firstUri = MonacoModelUri.Create(editorId, firstModel, "original");
        var secondUri = MonacoModelUri.Create(editorId, secondModel, "modified");

        // Then
        firstUri.Scheme.ShouldBe("inmemory");
        firstUri.Authority.ShouldBe(editorId.ToString());
        Uri.UnescapeDataString(firstUri.AbsolutePath).ShouldEndWith("/C:/Scripts/Bot.csx");
        firstUri.Query.ShouldContain("modelId=model-a");
        firstUri.Query.ShouldContain("role=original");
        secondUri.Query.ShouldContain("modelId=model-b");
        secondUri.Query.ShouldContain("role=modified");
        secondUri.ShouldNotBe(firstUri);
    }

    /// <summary>
    /// WHAT: Renaming or moving a local model should produce a new Monaco model URI instead of mutating the old one.
    /// HOW: Build a URI, change the local path, and assert that the next URI points at the new path while keeping the same model identity.
    /// </summary>
    [Test]
    public void ShouldCreateDifferentUri_WhenModelPathChanges()
    {
        // Given
        var editorId = new MonacoEditorId("editor-a");
        var textModel = new TestTextModel(new TextModelId("model-a"), "Scripts/Before.csx");
        var beforeUri = MonacoModelUri.Create(editorId, textModel, "main");

        // When
        textModel.Path = "Scripts/After.json";
        var afterUri = MonacoModelUri.Create(editorId, textModel, "main");

        // Then
        afterUri.ShouldNotBe(beforeUri);
        Uri.UnescapeDataString(afterUri.AbsolutePath).ShouldEndWith("/Scripts/After.json");
        afterUri.Query.ShouldContain("modelId=model-a");
    }

    /// <summary>
    /// WHAT: Provider registrations should retain and dispose both the JavaScript disposable and the .NET facade.
    /// HOW: Dispose the combined registration and assert that the JS registration is released before the facade reference.
    /// </summary>
    [Test]
    public async Task ShouldDisposeProviderJsRegistrationBeforeFacade()
    {
        // Given
        var events = new List<string>();
        var registration = new MonacoProviderRegistration(
            new TestAsyncDisposable("facade", events),
            new TestAsyncDisposable("js", events));

        // When
        await registration.DisposeAsync();

        // Then
        events.ShouldBe(new[] { "js", "facade" });
    }

    private sealed class TestTextModel : ITextModel
    {
        public TestTextModel(TextModelId id, string path)
        {
            Id = id;
            Path = path;
        }

        public TextModelId Id { get; }

        public string Path { get; set; }

        public IObservable<EventArgs> WhenChanged => Observable.Never<EventArgs>();

        public Task<SourceText> GetTextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(SourceText.From(string.Empty));
        }

        public Task SetTextAsync(SourceText sourceText, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class TestAsyncDisposable : IAsyncDisposable
    {
        private readonly string name;
        private readonly List<string> events;

        public TestAsyncDisposable(string name, List<string> events)
        {
            this.name = name;
            this.events = events;
        }

        public ValueTask DisposeAsync()
        {
            events.Add(name);
            return ValueTask.CompletedTask;
        }
    }
}
