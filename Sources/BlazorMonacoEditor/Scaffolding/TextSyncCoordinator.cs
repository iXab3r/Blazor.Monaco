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
    private readonly CompositeDisposable anchors = new();
    private readonly ITextModel localModel;
    private readonly ILogger logger;
    private readonly Func<SourceText> getRemoteTextSnapshot;
    private readonly Func<IReadOnlyList<IdentifiedSingleEditOperation>, CancellationToken, ValueTask> applyLocalChangesToRemoteAsync;

    public TextSyncCoordinator(
        ITextModel localModel,
        MonacoTextModelFacade remoteModel,
        ILogger logger)
        : this(
            localModel,
            remoteModel?.WhenContentChanged ?? throw new ArgumentNullException(nameof(remoteModel)),
            () => remoteModel.Text,
            (changes, _) => remoteModel.ApplyChanges(changes),
            logger)
    {
    }

    internal TextSyncCoordinator(
        ITextModel localModel,
        IObservable<MonacoTextModelChange> remoteChanges,
        Func<SourceText> getRemoteTextSnapshot,
        Func<IReadOnlyList<IdentifiedSingleEditOperation>, CancellationToken, ValueTask> applyLocalChangesToRemoteAsync,
        ILogger logger)
    {
        this.localModel = localModel ?? throw new ArgumentNullException(nameof(localModel));
        this.getRemoteTextSnapshot = getRemoteTextSnapshot ?? throw new ArgumentNullException(nameof(getRemoteTextSnapshot));
        this.applyLocalChangesToRemoteAsync = applyLocalChangesToRemoteAsync ?? throw new ArgumentNullException(nameof(applyLocalChangesToRemoteAsync));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (remoteChanges == null)
        {
            throw new ArgumentNullException(nameof(remoteChanges));
        }

        logger.LogDebug(
            "Starting change-list TextSyncCoordinator for model {ModelId}",
            localModel.Id);

        var localChanges = localModel.WhenChanged
            .Select(_ => (SyncWorkItem)new LocalSyncWorkItem());

        var monacoChanges = remoteChanges
            .Select(change => (SyncWorkItem)new MonacoSyncWorkItem(change));

        var syncItems = Observable.Merge(localChanges, monacoChanges);
        anchors.Add(syncItems.SubscribeAsync(HandleSyncWorkItemAsync, HandlePipelineError));
    }

    internal TextSyncCoordinator(
        ITextModel localModel,
        IObservable<MonacoTextModelChange> remoteChanges,
        Func<string, int?, CancellationToken, ValueTask<bool>> pushLocalTextToRemoteAsync,
        ILogger logger,
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
            logger)
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
            case MonacoSyncWorkItem monacoSyncWorkItem:
                await ApplyMonacoChangeAsync(monacoSyncWorkItem.Change, cancellationToken);
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

    private async Task ApplyMonacoChangeAsync(MonacoTextModelChange change, CancellationToken cancellationToken)
    {
        var remoteText = change.NewText;

        var localText = await localModel.GetTextAsync(cancellationToken);
        if (localText.ContentEquals(remoteText))
        {
            logger.LogDebug(
                "Skipping remote->local update for model {ModelId}: version {VersionId}, same text",
                localModel.Id,
                change.VersionId);
            return;
        }
        var remoteToLocalChanges = remoteText.GetTextChanges(localText);
        if (remoteToLocalChanges.Count <= 0)
        {
            logger.LogDebug(
                "Skipping remote->local update for model {ModelId}: version {VersionId}, no diffs",
                localModel.Id,
                change.VersionId);
            return;
        }

        var updatedLocalText = MonacoRoslynAdapter.ApplyChanges(localText, remoteToLocalChanges);

        logger.LogDebug(
            "Applying remote->local diffs for model {ModelId}: version {VersionId}, {ChangeCount} change(s), lengths local={LocalLength}, remote={RemoteLength}",
            localModel.Id,
            change.VersionId,
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

    private sealed record MonacoSyncWorkItem(MonacoTextModelChange Change) : SyncWorkItem;
}
