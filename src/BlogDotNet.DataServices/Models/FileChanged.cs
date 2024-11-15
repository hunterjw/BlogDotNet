namespace BlogDotNet.DataServices.Models;

/// <summary>
/// File changed information
/// </summary>
internal class FileChanged
{
    /// <summary>
    /// Full path to the changed file
    /// </summary>
    public string? FullPath { get; set; }

    /// <summary>
    /// Type of the file change
    /// </summary>
    public FileChangeType ChangeType { get; set; }
}
