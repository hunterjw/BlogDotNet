using BlogDotNet.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogDotNet.Database;

/// <summary>
/// BlogDotNet database context
/// </summary>
/// <param name="databaseSettings">Database settings</param>
public class BlogDotNetContext(DatabaseOptions databaseSettings) : DbContext
{
    private readonly DatabaseOptions _databaseSettings = databaseSettings;

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_databaseSettings.ConnectionString);
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<RankedBlogPost>(
                eb =>
                {
                    eb.HasNoKey();
                    eb.ToView("view_ranked_blog_post", "blogdotnet");
                });
    }

    /// <summary>
    /// Blog posts
    /// </summary>
    public DbSet<BlogPost> BlogPosts { get; set; }

    /// <summary>
    /// Ranked blog posts
    /// </summary>
    public DbSet<RankedBlogPost> RankedBlogPosts { get; set; }
}
