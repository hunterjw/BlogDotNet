using AutoMapper;
using BlogDotNet.DataServices.Abstractions.Interfaces;
using BlogDotNet.DataServices.Abstractions.Models;
using BlogDotNet.DataServices.Models;
using Markdig;
using Newtonsoft.Json;

namespace BlogDotNet.DataServices;

/// <summary>
/// File scanner service
/// </summary>
/// <param name="options">Options</param>
/// <param name="blogPostService">Blog post service</param>
/// <param name="markdownPipeline">Markdown pipeline</param>
/// <param name="mapper">Mapper</param>
public class FileScannerService(
    FileScannerServiceOptions options,
    IBlogPostService blogPostService,
    MarkdownPipeline markdownPipeline,
    IMapper mapper)
    : IFileScannerService
{
    private const string BlogPostFileExtension = ".blogpost.json";

    private readonly FileScannerServiceOptions _options = options;
    private readonly IBlogPostService _blogPostService = blogPostService;
    private readonly MarkdownPipeline _markdownPipeline = markdownPipeline;
    private readonly IMapper _mapper = mapper;

    /// <inheritdoc/>
    public async Task ScanAllFiles()
    {
        if (string.IsNullOrWhiteSpace(_options.BasePath))
        {
            throw new Exception("No base path specified for file scanning");
        }

        List<BlogPost> existingPosts = await _blogPostService.GetBlogPosts();
        List<string> existingsFiles = existingPosts
            .Select(_ => _.FilePath)
            .ToList();

        string[] files = Directory.GetFiles(
            _options.BasePath,
            $"*{BlogPostFileExtension}",
            SearchOption.AllDirectories);

        IEnumerable<BlogPost> toRemove = existingPosts.Where(_ => !files.Contains(Path.Combine(_options.BasePath, _.FilePath)));
        foreach (BlogPost blogPost in toRemove)
        {
            await _blogPostService.DeleteBlogPost(blogPost);
        }

        IEnumerable<BlogPost> toUpdate = existingPosts.Where(_ => files.Contains(Path.Combine(_options.BasePath, _.FilePath)));
        foreach (BlogPost blogPost in toUpdate)
        {
            BlogPost? scanned = await ScanFile(Path.Combine(_options.BasePath, blogPost.FilePath));
            if (scanned == null)
            {
                continue;
            }

            scanned.Id = blogPost.Id;
            BlogPost updated = _mapper.Map(scanned, blogPost);
            await _blogPostService.UpdateBlogPost(updated);
        }

        IEnumerable<string> toAdd = files.Where(_ => !existingsFiles.Contains(Path.GetRelativePath(_options.BasePath, _)));
        foreach (string file in toAdd)
        {
            BlogPost? blogpost = await ScanFile(file);
            if (blogpost != null)
            {
                await _blogPostService.AddBlogPost(blogpost);
            }
        }
    }

    /// <inheritdoc/>
    public async Task ScanAddedFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(_options.BasePath))
        {
            throw new Exception("No base path specified for file scanning");
        }

        if (!filePath.EndsWith(BlogPostFileExtension))
        {
            return;
        }

        BlogPost? blogpost = await ScanFile(filePath);
        if (blogpost != null)
        {
            await _blogPostService.AddBlogPost(blogpost);
        }
    }

    /// <inheritdoc/>
    public async Task ScanUpdatedFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(_options.BasePath))
        {
            throw new Exception("No base path specified for file scanning");
        }

        IEnumerable<BlogPost> blogPosts = await GetBlogPostsByFilePath(filePath);
        foreach (BlogPost blogPost in blogPosts)
        {
            BlogPost? scanned = await ScanFile(Path.Combine(_options.BasePath, blogPost.FilePath));
            if (scanned == null)
            {
                continue;
            }

            scanned.Id = blogPost.Id;
            BlogPost updated = _mapper.Map(scanned, blogPost);
            await _blogPostService.UpdateBlogPost(updated);
        }
    }

    /// <inheritdoc/>
    public async Task ScanRemovedFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(_options.BasePath))
        {
            throw new Exception("No base path specified for file scanning");
        }

        IEnumerable<BlogPost> blogPosts = await GetBlogPostsByFilePath(filePath);
        foreach (BlogPost blogPost in blogPosts)
        {
            if (filePath.EndsWith(BlogPostFileExtension))
            {
                await _blogPostService.DeleteBlogPost(blogPost);
            }
            else
            {
                blogPost.ContentMd = null;
                blogPost.ContentHtml = null;
                await _blogPostService.UpdateBlogPost(blogPost);
            }
        }
    }

    /// <summary>
    /// Get blog posts that are linked to a file path
    /// </summary>
    /// <param name="filePath">File path to search for</param>
    /// <returns>List of <see cref="BlogPost"/></returns>
    private async Task<IEnumerable<BlogPost>> GetBlogPostsByFilePath(string filePath)
    {
        if (filePath == null || _options.BasePath == null)
        {
            return [];
        }

        string relativeFilePath = Path.GetRelativePath(_options.BasePath, filePath);

        if (relativeFilePath.EndsWith(BlogPostFileExtension))
        {
            // Blog post file
            BlogPost? blogPost = await _blogPostService.GetBlogPostByFilePath(relativeFilePath);
            if (blogPost == null)
            {
                return [];
            }

            return [blogPost];
        }
        else
        {
            // Content file path
            return await _blogPostService.GetBlogPostsByContentFilePath(relativeFilePath);
        }
    }

    /// <summary>
    /// Scan a blog post file
    /// </summary>
    /// <param name="filePath">Blog post file path</param>
    /// <returns><see cref="BlogPost"/>, or null if scanning fails</returns>
    private async Task<BlogPost?> ScanFile(string? filePath)
    {
        if (filePath == null || _options.BasePath == null || !File.Exists(filePath))
        {
            return null;
        }

        string fileContents = await File.ReadAllTextAsync(filePath);
        BlogPost? blogPost = JsonConvert.DeserializeObject<BlogPost>(fileContents);

        if (blogPost != null)
        {
            blogPost.FilePath = Path.GetRelativePath(_options.BasePath, filePath);

            if (!string.IsNullOrWhiteSpace(blogPost.ContentFilePath))
            {
                string? directory = Path.GetDirectoryName(filePath);
                if (directory != null)
                {
                    string contentFilePath = Path.Combine(directory, blogPost.ContentFilePath);
                    blogPost.ContentFilePath = Path.GetRelativePath(_options.BasePath, contentFilePath);

                    if (File.Exists(contentFilePath))
                    {
                        blogPost.ContentMd = await File.ReadAllTextAsync(contentFilePath);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(blogPost.ContentMd))
            {
                blogPost.ContentHtml = Markdown.ToHtml(blogPost.ContentMd, _markdownPipeline);
            }
        }

        return blogPost;
    }
}
