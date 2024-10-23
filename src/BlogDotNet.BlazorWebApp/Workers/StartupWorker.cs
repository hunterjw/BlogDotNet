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

    private AsyncServiceScope _scope;
    private IFileWatcherService? _fileWatcherService;

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _scope = _serviceProvider.CreateAsyncScope();
        _fileWatcherService = _scope.ServiceProvider.GetRequiredService<IFileWatcherService>();

        // Run migrations
        MigrationService migrationsService = _scope.ServiceProvider.GetRequiredService<MigrationService>();
        migrationsService.RunMigrations();

        // Scan all files
        IFileScannerService fileScannerService = _scope.ServiceProvider.GetRequiredService<IFileScannerService>();
        await fileScannerService.ScanAllFiles();

        // Start watching for file changes
        await _fileWatcherService.StartWatchingForFileChanges();
    }

    /// <inheritdoc/>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_fileWatcherService != null)
        {
            await _fileWatcherService.StopWatchingForFileChanges();
        }
    }
}
