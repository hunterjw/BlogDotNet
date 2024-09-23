namespace BlogDotNet.DataServices.Abstractions.Models;

public class BlogPost
{
    /// <summary>
    /// Post ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Post title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Post description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Post publication date
    /// </summary>
    public DateTimeOffset? PubDate { get; set; }



    /// <summary>
    /// Post slug
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Post file path
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Content file path
    /// </summary>
    public string? ContentFilePath { get; set; }

    /// <summary>
    /// Content in markdown
    /// </summary>
    public string? ContentMd { get; set; }

    /// <summary>
    /// Content in HTML
    /// </summary>
    public string? ContentHtml { get; set; }
}
