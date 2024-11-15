using BlogDotNet.DataServices.Abstractions.Interfaces;
using BlogDotNet.DataServices.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace BlogDotNet.DataServices;

/// <summary>
/// File watcher service
/// </summary>
public class FileWatcherService : IFileWatcherService
{
    private readonly FileWatcherServiceOptions _options;
    private readonly ILogger<FileWatcherService> _logger;
    private readonly IFileScannerService _fileScannerService;

    private readonly BackgroundQueue<FileChanged> _backgroundQueue;

    private PhysicalFileProvider? _physicalFileProvider;
    private IChangeToken? _changeToken;
    private bool _watchForChanges = false;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options">Options</param>
    /// <param name="logger">Logger</param>
    /// <param name="fileScannerService">File scanner service</param>
    public FileWatcherService(
        FileWatcherServiceOptions options,
        ILogger<FileWatcherService> logger,
        IFileScannerService fileScannerService)
    {
        _options = options;
        _logger = logger;
        _fileScannerService = fileScannerService;

        _backgroundQueue = new BackgroundQueue<FileChanged>(logger, HandleChangedFile);
    }

    /// <inheritdoc/>
    public Task StartWatchingForFileChanges()
    {
        if (string.IsNullOrWhiteSpace(_options.BasePath))
        {
            throw new Exception("Missing base path configuration for file watcher");
        }

        _physicalFileProvider = new PhysicalFileProvider(_options.BasePath)
        {
            UsePollingFileWatcher = true,
            UseActivePolling = true,
        };

        _watchForChanges = true;
        WatchForChanges(GetDirectoryFiles());

        _logger.LogInformation("Watching for file changes");

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopWatchingForFileChanges()
    {
        _logger.LogInformation("Stopping file watching");

        _watchForChanges = false;

        return Task.CompletedTask;
    }

    private void WatchForChanges(IEnumerable<IFileInfo> currentState)
    {
        _changeToken = _physicalFileProvider?.Watch("**.*");
        _changeToken?.RegisterChangeCallback(Notify, currentState);
    }

    private void Notify(object? state)
    {
        _logger.LogInformation($"File change detected");

        if (_watchForChanges)
        {
            List<IFileInfo> currentState = GetDirectoryFiles();
            WatchForChanges(currentState);

            EnqueueChangedFiles(currentState, state as List<IFileInfo> ?? []);
        }
    }

    private List<IFileInfo> GetDirectoryFiles(string subDirectory = "")
    {
        List<IFileInfo> toReturn = [];

        IDirectoryContents? directoryContents = _physicalFileProvider?.GetDirectoryContents(subDirectory);
        toReturn.AddRange(directoryContents?.Where(_ => !_.IsDirectory) ?? []);
        foreach (IFileInfo? directory in directoryContents?.Where(_ => _.IsDirectory) ?? [])
        {
            toReturn.AddRange(GetDirectoryFiles(Path.Combine(subDirectory, directory.Name)));
        }

        return toReturn;
    }

    private void EnqueueChangedFiles(List<IFileInfo> currentState, List<IFileInfo> previousState)
    {
        IEnumerable<IFileInfo> removed = previousState.Where(ps => !currentState.Any(cs => ps.PhysicalPath == cs.PhysicalPath));
        foreach (IFileInfo file in removed)
        {
            _backgroundQueue.Enqueue(new FileChanged
            {
                ChangeType = FileChangeType.Deleted,
                FullPath = file.PhysicalPath,
            });
        }

        IEnumerable<IFileInfo> updated = previousState.Where(previousFile =>
        {
            IFileInfo? currentFile = currentState.FirstOrDefault(_ => _.PhysicalPath == previousFile.PhysicalPath);

            return currentFile != null && (currentFile.Length != previousFile.Length || currentFile.LastModified != previousFile.LastModified);
        });
        foreach (IFileInfo file in updated)
        {
            _backgroundQueue.Enqueue(new FileChanged
            {
                ChangeType = FileChangeType.Updated,
                FullPath = file.PhysicalPath,
            });
        }

        IEnumerable<IFileInfo> added = currentState.Where(cs => !previousState.Any(ps => ps.PhysicalPath == cs.PhysicalPath));
        foreach (IFileInfo file in added)
        {
            _backgroundQueue.Enqueue(new FileChanged
            {
                ChangeType = FileChangeType.Created,
                FullPath = file.PhysicalPath,
            });
        }
    }

    private async Task HandleChangedFile(FileChanged fileChanged)
    {
        if (string.IsNullOrWhiteSpace(fileChanged.FullPath))
        {
            _logger.LogWarning("Blank file changed path");
            return;
        }

        switch (fileChanged.ChangeType)
        {
            case FileChangeType.Created:
                _logger.LogInformation("File created: {fullPath}", fileChanged.FullPath);
                await _fileScannerService.ScanAddedFile(fileChanged.FullPath);
                break;
            case FileChangeType.Updated:
                _logger.LogInformation("File updated: {fullPath}", fileChanged.FullPath);
                await _fileScannerService.ScanUpdatedFile(fileChanged.FullPath);
                break;
            case FileChangeType.Deleted:
                _logger.LogInformation("File deleted: {fullPath}", fileChanged.FullPath);
                await _fileScannerService.ScanRemovedFile(fileChanged.FullPath);
                break;
            default:
                _logger.LogWarning("Unknown file change type {chengeType} for file {fullPath}", fileChanged.ChangeType, fileChanged.FullPath);
                break;
        }
    }
}
