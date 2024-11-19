using System.ComponentModel.DataAnnotations.Schema;

namespace BlogDotNet.Database.Models;

public class RankedBlogPost
{
    /// <summary>
    /// Blog post rank, lower = newer
    /// </summary>
    [Column("rank")]
    public int Rank { get; set; }

    /// <summary>
    /// Post ID
    /// </summary>
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Post title
    /// </summary>
    [Column("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Post description
    /// </summary>
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Post publication date
    /// </summary>
    [Column("pub_date")]
    public DateTimeOffset PubDate { get; set; }

    /// <summary>
    /// Post slug
    /// </summary>
    [Column("slug")]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Post file path
    /// </summary>
    [Column("file_path")]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Content file path
    /// </summary>
    [Column("content_file_path")]
    public string? ContentFilePath { get; set; }

    /// <summary>
    /// Content in markdown
    /// </summary>
    [Column("content_md")]
    public string? ContentMd { get; set; }

    /// <summary>
    /// Content in HTML
    /// </summary>
    [Column("content_html")]
    public string? ContentHtml { get; set; }

    /// <summary>
    /// If the post is pinned or not
    /// </summary>
    [Column("pinned")]
    public bool Pinned { get; set; } = false;

    /// <summary>
    /// If the post is hidden or not
    /// </summary>
    [Column("hidden")]
    public bool Hidden { get; set; } = false;
}
