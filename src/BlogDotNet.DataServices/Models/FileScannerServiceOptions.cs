namespace BlogDotNet.DataServices.Models;

/// <summary>
/// Options for <see cref="FileScannerService"/>
/// </summary>
public class FileScannerServiceOptions
{
    /// <summary>
    /// Base file path to search for files in
    /// </summary>
    public string? BasePath { get; set; }
}
