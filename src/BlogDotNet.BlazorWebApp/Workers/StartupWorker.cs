using BlogDotNet.Database.Migrations;
using BlogDotNet.DataServices.Abstractions.Interfaces;

namespace BlogDotNet.BlazorWebApp.Workers;

/// <summary>
/// Worker to perform tasks while the web app starts
/// </summary>
/// <param name="serviceProvider">Service provider</param>
public class StartupWorker(IServiceProvider serviceProvider) : IHostedService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        // Run migrations
        MigrationService migrationsService = scope.ServiceProvider.GetRequiredService<MigrationService>();
        migrationsService.RunMigrations();

        // Scan all files
        IFileScannerService fileScanner = scope.ServiceProvider.GetRequiredService<IFileScannerService>();
        await fileScanner.ScanAllFiles();
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
