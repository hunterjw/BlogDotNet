using BlogDotNet.DataServices.Abstractions.Interfaces;
using BlogDotNet.DataServices.Models;
using Microsoft.Extensions.Logging;
using Polly.Retry;
using Polly;

namespace BlogDotNet.DataServices;

/// <summary>
/// File watcher service
/// </summary>
public class FileWatcherService : IFileWatcherService
{
    private readonly ILogger<FileWatcherService> _logger;
    private readonly IFileScannerService _fileScannerService;
    private readonly string? _basePath;

    private readonly BackgroundQueue<FileSystemEventArgs> _backgroundQueue;
    private readonly ResiliencePipeline _resiliencePipeline = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
            MaxRetryAttempts = int.MaxValue,
            Delay = TimeSpan.FromSeconds(1),
        })
        .Build();

    private FileSystemWatcher? _fileSystemWatcher;

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
        _basePath = options.BasePath;

        _fileSystemWatcher = BuildWatcher(options.BasePath);

        _backgroundQueue = new BackgroundQueue<FileSystemEventArgs>(logger, HandleFileSystemEvent);
    }

    /// <inheritdoc/>
    public Task StartWatchingForFileChanges()
    {
        StartWatchingForFileChangesInternal();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopWatchingForFileChanges()
    {
        StopWatchingForFileChangesInternal();
        return Task.CompletedTask;
    }

    private FileSystemWatcher BuildWatcher(string? basePath)
    {
        FileSystemWatcher watcher = new();
        if (!string.IsNullOrWhiteSpace(basePath))
        {
            watcher.Path = basePath;
        }
        watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
        watcher.Changed += OnChanged;
        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;
        watcher.Renamed += OnRenamed;
        watcher.Error += OnError;
        watcher.Filter = "*.*";
        watcher.IncludeSubdirectories = true;

        return watcher;
    }

    private void StartWatchingForFileChangesInternal()
    {
        if (_fileSystemWatcher != null)
        {
            _fileSystemWatcher.EnableRaisingEvents = true;
            _logger.LogInformation("Watching for file changes");
        }
    }
    public void StopWatchingForFileChangesInternal()
    {
        if (_fileSystemWatcher != null)
        {
            _logger.LogInformation("Stopping file watching");
            _fileSystemWatcher.EnableRaisingEvents = false;
        }
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

    private void OnError(object sender, ErrorEventArgs e)
    {
        _logger.LogError(e.GetException(), "File watcher error");

        StopWatchingForFileChangesInternal();
        _fileSystemWatcher = null;

        _resiliencePipeline.Execute(() =>
        {
            _fileSystemWatcher = BuildWatcher(_basePath);
            StartWatchingForFileChangesInternal();

            _logger.LogInformation("Recovered from file watcher error");
        });

        _ = Task.Run(async () =>
        {
            _logger.LogInformation("Full scan required to pick up missed file changes");

            _logger.LogInformation("Waiting 5 seconds for file system to settle down...");
            await Task.Delay(5000);

            _logger.LogInformation("Rescanning all files...");
            await _fileScannerService.ScanAllFiles();

            _logger.LogInformation("Scan complete!");
        });
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
