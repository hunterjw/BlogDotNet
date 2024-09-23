namespace BlogDotNet.DataServices.Abstractions.Interfaces;

public interface IFileScannerService
{
    /// <summary>
    /// Scan all files
    /// </summary>
    Task ScanAllFiles();
}
