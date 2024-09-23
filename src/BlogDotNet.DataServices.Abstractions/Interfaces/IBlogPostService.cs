using BlogDotNet.DataServices.Abstractions.Models;

namespace BlogDotNet.DataServices.Abstractions.Interfaces;

public interface IBlogPostService
{
    /// <summary>
    /// Add a blog post
    /// </summary>
    /// <param name="blogPost">Post to add</param>
    /// <returns>Added <see cref="BlogPost"/></returns>
    Task<BlogPost> AddBlogPost(BlogPost blogPost);

    /// <summary>
    /// Get blog posts
    /// </summary>
    /// <param name="take">Number of posts to get</param>
    /// <param name="after">After post ID</param>
    /// <param name="before">Before post ID</param>
    /// <returns><see cref="Listing{T}"/> of <see cref="BlogPost"/></returns>
    Task<Listing<BlogPost>> GetBlogPosts(int take = 25, Guid? after = null, Guid? before = null);

    /// <summary>
    /// Get all blog posts
    /// </summary>
    /// <returns>List of <see cref="BlogPost"/></returns>
    Task<List<BlogPost>> GetBlogPosts();

    /// <summary>
    /// Get a blog post by its slug
    /// </summary>
    /// <param name="slug">Post slug</param>
    /// <returns><see cref="BlogPost"/> if found, null otherwise</returns>
    Task<BlogPost?> GetBlogPost(string slug);

    /// <summary>
    /// Update a blog post
    /// </summary>
    /// <param name="blogPost">Post to update</param>
    /// <returns>Updated <see cref="BlogPost"/>, null if post is not in database</returns>
    Task<BlogPost?> UpdateBlogPost(BlogPost blogPost);

    /// <summary>
    /// Delete a blog post
    /// </summary>
    /// <param name="blogPost">Post to delete</param>
    Task DeleteBlogPost(BlogPost blogPost);
}
