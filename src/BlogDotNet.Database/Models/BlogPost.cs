﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogDotNet.Database.Models;

[Table("blog_post", Schema = "blogdotnet")]
public class BlogPost
{
    /// <summary>
    /// Post ID
    /// </summary>
    [Column("id")]
    [Required]
    [Key]
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
    [Required]
    public DateTimeOffset PubDate { get; set; }

    /// <summary>
    /// Post slug
    /// </summary>
    [Column("slug")]
    [Required]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Post file path
    /// </summary>
    [Column("file_path")]
    [Required]
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
}