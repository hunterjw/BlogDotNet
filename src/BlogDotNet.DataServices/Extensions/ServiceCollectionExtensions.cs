using BlogDotNet.DataServices.Abstractions.Interfaces;
using BlogDotNet.DataServices.Mapping;
using Markdig;
using Microsoft.Extensions.DependencyInjection;

namespace BlogDotNet.DataServices.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add BlogDotNet services to the service collection
    /// </summary>
    /// <param name="services">Service collection to update</param>
    /// <returns>Updated service collection</returns>
    public static IServiceCollection AddBlogDotNetServices(this IServiceCollection services)
    {
        return services
            .AddTransient<IFileScannerService, FileScannerService>()
            .AddTransient<IBlogPostService, BlogPostService>()
            .AddTransient(_ => new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseBootstrap()
                .Build())
            .AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            })
            .AddScoped<IFileWatcherService, FileWatcherService>();
    }
}
