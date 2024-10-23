namespace BlogDotNet.DataServices.Abstractions.Interfaces;

public interface IFileWatcherService
{
    /// <summary>
    /// Start watching for file changes
    /// </summary>
    Task StartWatchingForFileChanges();

    /// <summary>
    /// Stop watching for file changes
    /// </summary>
    /// <returns></returns>
    Task StopWatchingForFileChanges();
}
