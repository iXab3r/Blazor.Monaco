using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlazorMonacoEditor.Interop;
using BlazorMonacoEditor.Roslyn;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;

namespace BlazorMonacoEditor.Scaffolding;

internal sealed class TextSyncCoordinator : IDisposable
{
    private static readonly TimeSpan DefaultMonacoChangeThrottle = TimeSpan.FromMilliseconds(225);

    private readonly CompositeDisposable anchors = new();
    private readonly ITextModel localModel;
    private readonly ILogger logger;
    private readonly Func<SourceText> getRemoteTextSnapshot;
    private readonly Func<IReadOnlyList<IdentifiedSingleEditOperation>, CancellationToken, ValueTask> applyLocalChangesToRemoteAsync;

    public TextSyncCoordinator(
        ITextModel localModel,
        MonacoTextModelFacade remoteModel,
        ILogger logger,
        TimeSpan? monacoChangeThrottle = null,
        IScheduler? monacoThrottleScheduler = null)
        : this(
            localModel,
            remoteModel?.WhenContentChanged ?? throw new ArgumentNullException(nameof(remoteModel)),
            () => remoteModel.Text,
            (changes, _) => remoteModel.ApplyChanges(changes),
            logger,
            monacoChangeThrottle,
            monacoThrottleScheduler)
    {
    }

    internal TextSyncCoordinator(
        ITextModel localModel,
        IObservable<MonacoTextModelChange> remoteChanges,
        Func<SourceText> getRemoteTextSnapshot,
        Func<IReadOnlyList<IdentifiedSingleEditOperation>, CancellationToken, ValueTask> applyLocalChangesToRemoteAsync,
        ILogger logger,
        TimeSpan? monacoChangeThrottle = null,
        IScheduler? monacoThrottleScheduler = null)
    {
        this.localModel = localModel ?? throw new ArgumentNullException(nameof(localModel));
        this.getRemoteTextSnapshot = getRemoteTextSnapshot ?? throw new ArgumentNullException(nameof(getRemoteTextSnapshot));
        this.applyLocalChangesToRemoteAsync = applyLocalChangesToRemoteAsync ?? throw new ArgumentNullException(nameof(applyLocalChangesToRemoteAsync));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (remoteChanges == null)
        {
            throw new ArgumentNullException(nameof(remoteChanges));
        }

        var throttle = monacoChangeThrottle ?? DefaultMonacoChangeThrottle;
        var throttleScheduler = monacoThrottleScheduler ?? DefaultScheduler.Instance;

        logger.LogDebug(
            "Starting change-list TextSyncCoordinator for model {ModelId}: Monaco throttle {ThrottleMs} ms",
            localModel.Id,
            throttle.TotalMilliseconds);

        var localChanges = localModel.WhenChanged
            .Select(_ => (SyncWorkItem)new LocalSyncWorkItem());

        var monacoBatches = remoteChanges
            .Buffer(remoteChanges.Throttle(throttle, throttleScheduler))
            .Where(batch => batch.Count > 0)
            .Select(batch => (SyncWorkItem)new MonacoBatchSyncWorkItem(batch));

        var syncItems = Observable.Merge(localChanges, monacoBatches);
        anchors.Add(syncItems.SubscribeAsync(HandleSyncWorkItemAsync, HandlePipelineError));
    }

    internal TextSyncCoordinator(
        ITextModel localModel,
        IObservable<MonacoTextModelChange> remoteChanges,
        Func<string, int?, CancellationToken, ValueTask<bool>> pushLocalTextToRemoteAsync,
        ILogger logger,
        TimeSpan? monacoChangeThrottle = null,
        IScheduler? monacoThrottleScheduler = null,
        SourceText? initialSynchronizedLocalText = null)
        : this(
            localModel,
            remoteChanges,
            () => initialSynchronizedLocalText ?? SourceText.From(string.Empty),
            async (_, cancellationToken) =>
            {
                var textToPush = await localModel.GetTextAsync(cancellationToken);
                await pushLocalTextToRemoteAsync(textToPush.ToString(), null, cancellationToken);
            },
            logger,
            monacoChangeThrottle,
            monacoThrottleScheduler)
    {
    }

    public void Dispose()
    {
        if (anchors.IsDisposed)
        {
            return;
        }

        anchors.Dispose();
    }

    private async Task HandleSyncWorkItemAsync(SyncWorkItem workItem, CancellationToken cancellationToken)
    {
        switch (workItem)
        {
            case LocalSyncWorkItem:
                await ApplyLocalChangeAsync(cancellationToken);
                break;
            case MonacoBatchSyncWorkItem monacoBatchSyncWorkItem:
                await ApplyMonacoBatchAsync(monacoBatchSyncWorkItem.Changes, cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(workItem), $"Unsupported sync item type: {workItem.GetType()}");
        }
    }

    private async Task ApplyLocalChangeAsync(CancellationToken cancellationToken)
    {
        var localText = await localModel.GetTextAsync(cancellationToken);
        var remoteText = getRemoteTextSnapshot();

        if (localText.ContentEquals(remoteText))
        {
            logger.LogDebug(
                "Skipping local->remote update for model {ModelId}: same content (lengths local={LocalLength}, remote={RemoteLength})",
                localModel.Id,
                localText.Length,
                remoteText.Length);
            return;
        }

        var localToRemoteChanges = RoslynChangesAdaptor.PrepareDiff(remoteText, localText);
        if (localToRemoteChanges.Count <= 0)
        {
            logger.LogDebug(
                "Skipping local->remote update for model {ModelId}: no diffs (lengths local={LocalLength}, remote={RemoteLength})",
                localModel.Id,
                localText.Length,
                remoteText.Length);
            return;
        }

        logger.LogDebug(
            "Applying local->remote diffs for model {ModelId}: {OperationCount} operation(s), lengths remote={RemoteLength}, local={LocalLength}",
            localModel.Id,
            localToRemoteChanges.Count,
            remoteText.Length,
            localText.Length);

        await applyLocalChangesToRemoteAsync(localToRemoteChanges, cancellationToken);

        logger.LogDebug(
            "Applied local->remote diffs for model {ModelId}: {OperationCount} operation(s)",
            localModel.Id,
            localToRemoteChanges.Count);
    }

    private async Task ApplyMonacoBatchAsync(IList<MonacoTextModelChange> batch, CancellationToken cancellationToken)
    {
        if (batch.Count <= 0)
        {
            return;
        }

        var firstVersion = batch[0].VersionId;
        var lastVersion = batch[batch.Count - 1].VersionId;
        var remoteText = batch[batch.Count - 1].NewText;

        var localText = await localModel.GetTextAsync(cancellationToken);
        if (localText.ContentEquals(remoteText))
        {
            logger.LogDebug(
                "Skipping remote->local update for model {ModelId}: batch {BatchSize}, versions {FirstVersion}->{LastVersion}, same text",
                localModel.Id,
                batch.Count,
                firstVersion,
                lastVersion);
            return;
        }
        var remoteToLocalChanges = remoteText.GetTextChanges(localText);
        if (remoteToLocalChanges.Count <= 0)
        {
            logger.LogDebug(
                "Skipping remote->local update for model {ModelId}: batch {BatchSize}, versions {FirstVersion}->{LastVersion}, no diffs",
                localModel.Id,
                batch.Count,
                firstVersion,
                lastVersion);
            return;
        }

        var updatedLocalText = MonacoRoslynAdapter.ApplyChanges(localText, remoteToLocalChanges);

        logger.LogDebug(
            "Applying remote->local diffs for model {ModelId}: batch {BatchSize}, versions {FirstVersion}->{LastVersion}, {ChangeCount} change(s), lengths local={LocalLength}, remote={RemoteLength}",
            localModel.Id,
            batch.Count,
            firstVersion,
            lastVersion,
            remoteToLocalChanges.Count,
            localText.Length,
            remoteText.Length);

        await localModel.SetTextAsync(updatedLocalText, cancellationToken);

        logger.LogDebug(
            "Applied remote->local diffs for model {ModelId}: {ChangeCount} change(s), resulting local length {LocalLength}",
            localModel.Id,
            remoteToLocalChanges.Count,
            updatedLocalText.Length);
    }

    private void HandlePipelineError(Exception exception)
    {
        logger.LogError(exception, "Text synchronization pipeline failed for model {ModelId}", localModel.Id);
    }

    private abstract record SyncWorkItem;

    private sealed record LocalSyncWorkItem : SyncWorkItem;

    private sealed record MonacoBatchSyncWorkItem(IList<MonacoTextModelChange> Changes) : SyncWorkItem;
}
