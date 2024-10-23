namespace BlogDotNet.DataServices.Abstractions.Interfaces;

public interface IFileScannerService
{
    /// <summary>
    /// Scan all files
    /// </summary>
    Task ScanAllFiles();

    /// <summary>
    /// Scan an added file
    /// </summary>
    /// <param name="filePath">File path</param>
    Task ScanAddedFile(string filePath);

    /// <summary>
    /// Scan an updated file
    /// </summary>
    /// <param name="filePath">File path</param>
    Task ScanUpdatedFile(string filePath);

    /// <summary>
    /// Scan a renamed file
    /// </summary>
    /// <param name="oldFilePath">Old file path</param>
    /// <param name="newFilePath">New file path</param>
    Task ScanRenamedFile(string oldFilePath, string newFilePath);

    /// <summary>
    /// Scan a removed file
    /// </summary>
    /// <param name="filePath">File path</param>
    Task ScanRemovedFile(string filePath);
}
