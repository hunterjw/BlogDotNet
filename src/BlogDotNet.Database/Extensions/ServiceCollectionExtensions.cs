using Microsoft.Extensions.DependencyInjection;

namespace BlogDotNet.Database.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add BlogDotNet database to a service collection
    /// </summary>
    /// <param name="services">Service collection to update</param>
    /// <returns>Updated service collection</returns>
    public static IServiceCollection AddBlogDotNetDatabase(this IServiceCollection services)
    {
        return services.AddDbContext<BlogDotNetContext>();
    }
}
