using BlogDotNet.DataServices.Abstractions.Interfaces;
using BlogDotNet.DataServices.Models;
using Microsoft.Extensions.Logging;

namespace BlogDotNet.DataServices;

/// <summary>
/// File watcher service
/// </summary>
public class FileWatcherService : IFileWatcherService
{
    private readonly ILogger<FileWatcherService> _logger;
    private readonly IFileScannerService _fileScannerService;

    private readonly FileSystemWatcher _fileSystemWatcher;
    private readonly BackgroundQueue<FileSystemEventArgs> _backgroundQueue;

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
        _logger = logger;
        _fileScannerService = fileScannerService;

        _fileSystemWatcher = new FileSystemWatcher();
        if (!string.IsNullOrWhiteSpace(options.BasePath))
        {
            _fileSystemWatcher.Path = options.BasePath;
        }
        _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
        _fileSystemWatcher.Changed += OnChanged;
        _fileSystemWatcher.Created += OnCreated;
        _fileSystemWatcher.Deleted += OnDeleted;
        _fileSystemWatcher.Renamed += OnRenamed;
        _fileSystemWatcher.Error += OnError;
        _fileSystemWatcher.Filter = "*.*";
        _fileSystemWatcher.IncludeSubdirectories = true;

        _backgroundQueue = new BackgroundQueue<FileSystemEventArgs>(logger, HandleFileSystemEvent);
    }

    /// <inheritdoc/>
    public Task StartWatchingForFileChanges()
    {
        _fileSystemWatcher.EnableRaisingEvents = true;
        _logger.LogInformation("Watching for file changes");
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopWatchingForFileChanges()
    {
        _logger.LogInformation("Stopping file watching");
        _fileSystemWatcher.EnableRaisingEvents = false;
        return Task.CompletedTask;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        _backgroundQueue.Enqueue(e);
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        _backgroundQueue.Enqueue(e);
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        _backgroundQueue.Enqueue(e);
    }
    
    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        _backgroundQueue.Enqueue(e);
    }

    private void OnError(object sender, ErrorEventArgs e) =>
        PrintException(e.GetException());

    private void PrintException(Exception? ex)
    {
        _logger.LogError(ex, "File watcher error");
    }

    private async Task HandleFileSystemEvent(FileSystemEventArgs e)
    {
        switch (e.ChangeType)
        {
            case WatcherChangeTypes.Created:
                await _fileScannerService.ScanAddedFile(e.FullPath);
                break;
            case WatcherChangeTypes.Changed:
                await _fileScannerService.ScanUpdatedFile(e.FullPath);
                break;
            case WatcherChangeTypes.Renamed:
                RenamedEventArgs re = (RenamedEventArgs)e;
                await _fileScannerService.ScanRenamedFile(re.OldFullPath, re.FullPath);
                break;
            case WatcherChangeTypes.Deleted:
                await _fileScannerService.ScanRemovedFile(e.FullPath);
                break;
            default:
                break;
        }
    }
}
