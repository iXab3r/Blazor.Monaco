using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BlazorMonacoEditor.Scaffolding;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Shouldly;

namespace BlazorMonacoEditor.Tests;

[TestFixture]
public sealed class TextSyncCoordinatorFixture
{
    [Test]
    public async Task Given_LocalTextChange_When_Processed_Then_TextIsPushedToMonaco()
    {
        // Given
        var localModel = new TestTextModel("alpha");
        var remoteHarness = new RemoteHarness();
        using var coordinator = CreateCoordinator(localModel, remoteHarness);

        // When
        localModel.SetTextFromExternalEditor("alpha+");
        await WaitUntilAsync(
            () => remoteHarness.SetContentCallCount == 1,
            "Expected local edit to be propagated to Monaco");

        // Then
        remoteHarness.GetSetContentCalls().ShouldBe(new[] { "alpha+" });
    }

    [Test]
    public async Task Given_StaleMonacoChangeBeforeLocalPush_When_BatchContainsNewGenerationChanges_Then_StaleChangeIsIgnored()
    {
        // Given
        var localModel = new TestTextModel("A");
        var remoteHarness = new RemoteHarness();
        using var coordinator = CreateCoordinator(localModel, remoteHarness);

        remoteHarness.EmitMonacoChange(CreateInsertChange(versionId: 1, position: 1, insertedText: "1", resultingText: "A1"));
        localModel.SetTextFromExternalEditor("L");
        await WaitUntilAsync(
            () => remoteHarness.SetContentCallCount == 1,
            "Expected authoritative local push to happen before Monaco batch flush");

        // When
        remoteHarness.EmitMonacoChange(CreateFlushChange(versionId: 2, resultingText: "L"));
        remoteHarness.EmitMonacoChange(CreateInsertChange(versionId: 3, position: 1, insertedText: "!", resultingText: "L!"));
        await WaitUntilAsync(
            () => localModel.CurrentText == "L!",
            "Expected stale generation Monaco change to be ignored and newer generation change to apply");

        // Then
        localModel.CurrentText.ShouldBe("L!");
    }

    [Test]
    public async Task Given_NoKnownMonacoVersion_When_LocalChangeIsPushed_Then_PushDoesNotRequireExpectedVersion()
    {
        // Given
        var localModel = new TestTextModel("alpha");
        var remoteHarness = new RemoteHarness();
        using var coordinator = CreateCoordinator(localModel, remoteHarness);

        // When
        localModel.SetTextFromExternalEditor("alpha+");
        await WaitUntilAsync(
            () => remoteHarness.SetContentCallCount == 1,
            "Expected local edit to be propagated to Monaco");

        // Then
        var request = remoteHarness.GetSetContentRequests()[0];
        request.Text.ShouldBe("alpha+");
        request.ExpectedVersionId.ShouldBeNull();
    }

    [Test]
    public async Task Given_InFlightLocalPush_When_FlushAndUserEditArriveBeforePushCompletion_Then_UserEditShouldNotBeDropped()
    {
        // Given
        var localModel = new TestTextModel("start");
        var remoteHarness = new RemoteHarness();
        using var coordinator = CreateCoordinator(localModel, remoteHarness);

        remoteHarness.EmitMonacoChange(CreateFlushChange(versionId: 22, resultingText: "start"));
        await Task.Delay(40);

        remoteHarness.BlockNextSetContentCompletionOnly();

        // When
        localModel.SetTextFromExternalEditor("imgui");
        await WaitUntilAsync(
            () => remoteHarness.HasPendingBlockedSetContent,
            "Expected local push completion to be blocked after Monaco apply");

        remoteHarness.EmitMonacoChange(CreateFlushChange(versionId: 23, resultingText: "imgui"));
        remoteHarness.EmitMonacoChange(CreateInsertChange(versionId: 24, position: 5, insertedText: "!", resultingText: "imgui!"));
        remoteHarness.CompleteBlockedSetContent();

        await WaitUntilAsync(
            () => localModel.CurrentText == "imgui!",
            "Expected Monaco user edit emitted during in-flight push to be preserved");

        // Then
        localModel.CurrentText.ShouldBe("imgui!");
    }

    private static TextSyncCoordinator CreateCoordinator(
        TestTextModel localModel,
        RemoteHarness remoteHarness)
    {
        return new TextSyncCoordinator(
            localModel,
            remoteHarness.WhenContentChanged,
            remoteHarness.SetContentAsync,
            NullLogger.Instance);
    }

    private static MonacoTextModelChange CreateInsertChange(int versionId, int position, string insertedText, string resultingText)
    {
        return new MonacoTextModelChange(
            versionId,
            new[] { new TextChange(new TextSpan(position, 0), insertedText) },
            SourceText.From(resultingText),
            false);
    }

    private static MonacoTextModelChange CreateFlushChange(int versionId, string resultingText)
    {
        return new MonacoTextModelChange(
            versionId,
            Array.Empty<TextChange>(),
            SourceText.From(resultingText),
            true);
    }

    private static async Task WaitUntilAsync(
        Func<bool> condition,
        string timeoutMessage,
        TimeSpan? timeout = null)
    {
        var limit = timeout ?? TimeSpan.FromSeconds(2);
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < limit)
        {
            if (condition())
            {
                return;
            }

            await Task.Delay(15);
        }

        condition().ShouldBeTrue(timeoutMessage);
    }

    private sealed class TestTextModel : ITextModel
    {
        private readonly object gate = new();
        private readonly Subject<EventArgs> whenChanged = new();
        private SourceText currentText;
        private bool dropNextSetTextMutation;

        public TestTextModel(string initialText)
        {
            currentText = SourceText.From(initialText);
        }

        public int SetTextCallCount { get; private set; }

        public string CurrentText
        {
            get
            {
                lock (gate)
                {
                    return currentText.ToString();
                }
            }
        }

        public SourceText CurrentSourceText
        {
            get
            {
                lock (gate)
                {
                    return currentText;
                }
            }
        }

        public TextModelId Id { get; } = new(Guid.NewGuid().ToString("N"));

        public string Path { get; } = "/fixture.csx";

        public IObservable<EventArgs> WhenChanged => whenChanged.AsObservable();

        public Task<SourceText> GetTextAsync(CancellationToken cancellationToken = default)
        {
            lock (gate)
            {
                return Task.FromResult(currentText);
            }
        }

        public Task SetTextAsync(SourceText sourceText, CancellationToken cancellationToken = default)
        {
            bool shouldEmitChange = true;
            lock (gate)
            {
                if (dropNextSetTextMutation)
                {
                    dropNextSetTextMutation = false;
                    shouldEmitChange = false;
                }
                else
                {
                    currentText = sourceText;
                }

                SetTextCallCount++;
            }

            if (shouldEmitChange)
            {
                whenChanged.OnNext(EventArgs.Empty);
            }

            return Task.CompletedTask;
        }

        public void SetTextFromExternalEditor(string text)
        {
            lock (gate)
            {
                currentText = SourceText.From(text);
            }

            whenChanged.OnNext(EventArgs.Empty);
        }

        public void RaiseChangedWithoutTextMutation()
        {
            whenChanged.OnNext(EventArgs.Empty);
        }

        public void DropNextSetTextMutation()
        {
            lock (gate)
            {
                dropNextSetTextMutation = true;
            }
        }
    }

    private sealed class RemoteHarness
    {
        private readonly object gate = new();
        private readonly Subject<MonacoTextModelChange> whenContentChanged = new();
        private readonly ConcurrentQueue<SetContentRequest> setContentCalls = new();
        private TaskCompletionSource<bool>? pendingBlockedSetContent;
        private SetContentRequest pendingBlockedSetContentRequest;
        private bool blockNextSetContent;
        private bool blockNextSetContentCompletionOnly;
        private bool pendingBlockedSetContentApplyOnComplete = true;
        private bool pendingBlockedSetContentResult;
        private int currentMonacoVersion;

        public IObservable<MonacoTextModelChange> WhenContentChanged => whenContentChanged.AsObservable();

        public int SetContentCallCount => setContentCalls.Count;

        public bool HasPendingBlockedSetContent
        {
            get
            {
                lock (gate)
                {
                    return pendingBlockedSetContent is not null;
                }
            }
        }

        public ValueTask<bool> SetContentAsync(string text, int? expectedVersionId, CancellationToken cancellationToken)
        {
            var request = new SetContentRequest(text, expectedVersionId);
            setContentCalls.Enqueue(request);

            lock (gate)
            {
                if (blockNextSetContentCompletionOnly)
                {
                    blockNextSetContentCompletionOnly = false;
                    pendingBlockedSetContentRequest = request;
                    pendingBlockedSetContentApplyOnComplete = false;
                    pendingBlockedSetContentResult = TryApplySetContentRequest(request);
                    pendingBlockedSetContent = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    return new ValueTask<bool>(pendingBlockedSetContent.Task);
                }

                if (blockNextSetContent)
                {
                    blockNextSetContent = false;
                    pendingBlockedSetContentRequest = request;
                    pendingBlockedSetContentApplyOnComplete = true;
                    pendingBlockedSetContent = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    return new ValueTask<bool>(pendingBlockedSetContent.Task);
                }

                return new ValueTask<bool>(TryApplySetContentRequest(request));
            }
        }

        public string[] GetSetContentCalls()
        {
            var calls = setContentCalls.ToArray();
            var result = new string[calls.Length];
            for (var i = 0; i < calls.Length; i++)
            {
                result[i] = calls[i].Text;
            }

            return result;
        }

        public SetContentRequest[] GetSetContentRequests()
        {
            return setContentCalls.ToArray();
        }

        public void BlockNextSetContent()
        {
            lock (gate)
            {
                blockNextSetContent = true;
            }
        }

        public void BlockNextSetContentCompletionOnly()
        {
            lock (gate)
            {
                blockNextSetContentCompletionOnly = true;
            }
        }

        public void CompleteBlockedSetContent()
        {
            TaskCompletionSource<bool>? completion;
            SetContentRequest request;
            bool applyOnComplete;
            bool result;
            lock (gate)
            {
                completion = pendingBlockedSetContent;
                if (completion is null)
                {
                    throw new InvalidOperationException("No blocked SetContent call to complete.");
                }

                request = pendingBlockedSetContentRequest;
                applyOnComplete = pendingBlockedSetContentApplyOnComplete;
                result = pendingBlockedSetContentResult;
                if (applyOnComplete)
                {
                    result = TryApplySetContentRequest(request);
                }

                pendingBlockedSetContent = null;
                pendingBlockedSetContentApplyOnComplete = true;
                pendingBlockedSetContentResult = false;
            }

            completion.TrySetResult(result);
        }

        public void EmitMonacoChange(MonacoTextModelChange change)
        {
            lock (gate)
            {
                if (change.VersionId > currentMonacoVersion)
                {
                    currentMonacoVersion = change.VersionId;
                }
            }

            whenContentChanged.OnNext(change);
        }

        private bool TryApplySetContentRequest(SetContentRequest request)
        {
            if (request.ExpectedVersionId is int expectedVersionId && expectedVersionId != currentMonacoVersion)
            {
                return false;
            }

            currentMonacoVersion++;
            return true;
        }

        public readonly record struct SetContentRequest(string Text, int? ExpectedVersionId);
    }
}
