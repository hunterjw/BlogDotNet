namespace BlogDotNet.DataServices.Models;

/// <summary>
/// Options for <see cref="FileWatcherService"/>
/// </summary>
public class FileWatcherServiceOptions
{
    /// <summary>
    /// Base file path to watch for file changes in
    /// </summary>
    public string? BasePath { get; set; }
}
