﻿using BlogDotNet.Database.Models;
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

    /// <summary>
    /// Blog posts
    /// </summary>
    public DbSet<BlogPost> BlogPosts { get; set; }
}
