using BlogDotNet.Database.Migrations;
using BlogDotNet.Database.Migrations.Extensions;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", true)
    .AddEnvironmentVariables()
    .Build();

using ServiceProvider serviceProvider = CreateServices(config);
using IServiceScope scope = serviceProvider.CreateScope();

scope.ServiceProvider.GetRequiredService<MigrationService>().RunMigrations();

static ServiceProvider CreateServices(IConfigurationRoot config)
{
    return new ServiceCollection()
        .AddSingleton(config
            .GetRequiredSection("BlogDotNet")
            .GetRequiredSection("Migrations")
            .Get<MigrationServiceOptions>()
            ?? new MigrationServiceOptions())
        .AddBlogDotNetMigrations(config.GetConnectionString("BlogDotNet"))
        .AddLogging(lb => lb
            .AddFluentMigratorConsole())
        .BuildServiceProvider(false);
}