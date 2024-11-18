using AutoMapper;
using BlogDotNet.Database;
using BlogDotNet.DataServices.Abstractions.Interfaces;
using BlogDotNet.DataServices.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Db = BlogDotNet.Database.Models;

namespace BlogDotNet.DataServices;

/// <summary>
/// Blog post service
/// </summary>
/// <param name="mapper">Mapper</param>
/// <param name="blogDotNetContext">Database context</param>
public class BlogPostService(
    IMapper mapper, 
    BlogDotNetContext blogDotNetContext) 
    : IBlogPostService
{
    private readonly IMapper _mapper = mapper;
    private readonly BlogDotNetContext _blogDotNetContext = blogDotNetContext;

    /// <inheritdoc/>
    public async Task<BlogPost> AddBlogPost(BlogPost blogPost)
    {
        blogPost.Id = Guid.NewGuid();
        blogPost.PubDate = blogPost.PubDate?.ToUniversalTime();

        Db.BlogPost mapped = _mapper.Map<Db.BlogPost>(blogPost);
        await _blogDotNetContext.BlogPosts.AddAsync(mapped);
        await _blogDotNetContext.SaveChangesAsync();

        return _mapper.Map<BlogPost>(mapped);
    }

    /// <inheritdoc/>
    public async Task<Listing<BlogPost>> GetBlogPosts(int take = 25, Guid? after = null, Guid? before = null)
    {
        List<Db.RankedBlogPost>? blogPosts = null;

        IQueryable<Db.RankedBlogPost> query = _blogDotNetContext.RankedBlogPosts.Where(_ => _.Slug != "about");

        // Pagination
        if (after != null)
        {
            Db.RankedBlogPost afterPost = await query.FirstOrDefaultAsync(_ => _.Id == after) ??
                throw new ArgumentException($"Cannot find post with ID {after}", nameof(after));
            blogPosts =
            [
                .. await query
                    .Where(_ => _.Rank > afterPost.Rank)
                    .OrderBy(_ => _.Rank)
                    .Take(take)
                    .ToListAsync()
            ];
        }
        else if (before != null)
        {
            Db.RankedBlogPost beforePost = await query.FirstOrDefaultAsync(_ => _.Id == before) ??
                throw new ArgumentException($"Cannot find post with ID {before}", nameof(before));
            blogPosts =
            [
                .. await query
                    .Where(_ => _.Rank < beforePost.Rank)
                    .OrderByDescending(_ => _.Rank)
                    .Take(take)
                    .ToListAsync()
            ];
        }
        else
        {
            // default
            blogPosts ??=
            [
                .. await query
                    .OrderBy(_ => _.Rank)
                    .Take(take)
                    .ToListAsync()
            ];
        }

        List<Db.RankedBlogPost> results =
        [
            .. blogPosts.OrderBy(_ => _.Rank)
        ];
        bool showBefore = false;
        Db.RankedBlogPost? firstItem = results.FirstOrDefault();
        if (firstItem != null)
        {
            showBefore = await query.AnyAsync(_ => _.Rank < firstItem.Rank);
        }
        bool showAfter = false;
        Db.RankedBlogPost? lastItem = results.LastOrDefault();
        if (lastItem != null)
        {
            showAfter = await query.AnyAsync(_ => _.Rank > lastItem.Rank);
        }
        return new Listing<BlogPost>
        {
            After = showAfter ? results.LastOrDefault()?.Id : null,
            Before = showBefore ? results.FirstOrDefault()?.Id : null,
            Items = _mapper.Map<List<BlogPost>>(results)
        };
    }

    /// <inheritdoc/>
    public async Task<List<BlogPost>> GetBlogPosts()
    {
        List<Db.BlogPost> results = await _blogDotNetContext.BlogPosts.ToListAsync();

        return _mapper.Map<List<BlogPost>>(results);
    }

    /// <inheritdoc/>
    public async Task<BlogPost?> GetBlogPostBySlug(string slug)
    {
        Db.BlogPost? result = await _blogDotNetContext.BlogPosts.FirstOrDefaultAsync(_ => _.Slug == slug);
        return _mapper.Map<BlogPost>(result);
    }

    /// <inheritdoc/>
    public async Task<BlogPost?> GetBlogPostByFilePath(string filePath)
    {
        Db.BlogPost? result = await _blogDotNetContext.BlogPosts.FirstOrDefaultAsync(_ => _.FilePath == filePath);
        return _mapper.Map<BlogPost>(result);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<BlogPost>> GetBlogPostsByContentFilePath(string contentFilePath)
    {
        List<Db.BlogPost> result = await _blogDotNetContext.BlogPosts
            .Where(_ => _.ContentFilePath == contentFilePath)
            .ToListAsync();
        return _mapper.Map<List<BlogPost>>(result);
    }

    /// <inheritdoc/>
    public async Task<BlogPost?> UpdateBlogPost(BlogPost blogPost)
    {
        blogPost.PubDate = blogPost.PubDate?.ToUniversalTime();

        Db.BlogPost? toUpdate = await _blogDotNetContext.BlogPosts.FirstOrDefaultAsync(_ => _.Id == blogPost.Id);
        if (toUpdate == null)
        {
            return null;
        }

        Db.BlogPost updated = _mapper.Map(blogPost, toUpdate);
        _blogDotNetContext.BlogPosts.Update(updated);
        await _blogDotNetContext.SaveChangesAsync();

        return _mapper.Map<BlogPost>(updated);
    }

    /// <inheritdoc/>
    public async Task DeleteBlogPost(BlogPost blogPost)
    {
        Db.BlogPost? toRemove = await _blogDotNetContext.BlogPosts.FirstOrDefaultAsync(_ => _.Id == blogPost.Id);
        if (toRemove == null)
        {
            return;
        }

        _blogDotNetContext.Remove(toRemove);
        await _blogDotNetContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public Task<bool> HasAboutPage()
    {
        return Task.FromResult(_blogDotNetContext.BlogPosts.Any(_ => _.Slug == "about"));
    }
}
