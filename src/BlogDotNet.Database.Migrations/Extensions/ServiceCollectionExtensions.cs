using BlogDotNet.Database.Migrations.Migrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace BlogDotNet.Database.Migrations.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add BlogDotNet migrations to a service collection
    /// </summary>
    /// <param name="services">Service collection to add migrations to</param>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>Updated service collection</returns>
    public static IServiceCollection AddBlogDotNetMigrations(
        this IServiceCollection services, 
        string? connectionString)
    {
        return services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(CreateBlogDotNetSchema).Assembly).For.Migrations())
            .AddScoped<MigrationService>();
    }
}
