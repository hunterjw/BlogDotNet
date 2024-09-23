using FluentMigrator.Runner;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace BlogDotNet.Database.Migrations;

/// <summary>
/// Migration service
/// </summary>
/// <param name="options">Options</param>
/// <param name="logger">Logger</param>
/// <param name="runner">Migration runner</param>
public class MigrationService(
    MigrationServiceOptions options, 
    ILogger<MigrationService> logger, 
    IMigrationRunner runner)
{
    private readonly MigrationServiceOptions _options = options;
    private readonly IMigrationRunner _runner = runner;
    private readonly ILogger<MigrationService> _logger = logger;

    /// <summary>
    /// Esure the database exists prior to running migrations
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    private void EnsureDatabase()
    {
        _logger.LogInformation("Verifying database exists...");

        string? adminConnectionString = _options.PostgresAdminConnectionString;
        string? database = _options.Database;

        if (string.IsNullOrWhiteSpace(adminConnectionString))
        {
            throw new ArgumentException("Admin connection string required to run migrations");
        }
        if (string.IsNullOrWhiteSpace(database))
        {
            throw new ArgumentException("Database name required to run migrations");
        }

        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(adminConnectionString);

        using NpgsqlCommand checkIfExistsCommand = dataSource.CreateCommand("SELECT datname FROM postgres.pg_catalog.pg_database WHERE datname = @name");
        checkIfExistsCommand.Parameters.Add("name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = database;

        object? checkIfExistsResult = checkIfExistsCommand.ExecuteScalar();

        if (checkIfExistsResult != null && (checkIfExistsResult.ToString()?.Equals(database) ?? false))
        {
            _logger.LogInformation("Database exists!");
            return;
        }

        _logger.LogInformation("Creating database...");

        using NpgsqlCommand createDatabaseCommand = dataSource.CreateCommand(@$"CREATE DATABASE ""{database}""");

        createDatabaseCommand.ExecuteNonQuery();

        _logger.LogInformation("Database created!");
    }

    /// <summary>
    /// Update the database
    /// </summary>
    /// <param name="version">Version to upgrade to</param>
    private void UpdateDatabase(long? version = null)
    {
        if (version.HasValue)
        {
            _runner.MigrateUp(version.Value);
        }
        else
        {
            _runner.MigrateUp();
        }
    }

    /// <summary>
    /// Downgrade the database
    /// </summary>
    /// <param name="version">Version to downgrade to</param>
    private void DowngradeDatabase(long version)
    {
        _runner.MigrateDown(version);
    }

    /// <summary>
    /// Run the database migrations
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public void RunMigrations()
    {
        MigrationDirection dir = _options.Direction ?? MigrationDirection.Up;
        long? version = _options.Version;

        string versionLabel = version.HasValue ? version.Value.ToString() : "latest";
        _logger.LogInformation("Starting migrations...");
        _logger.LogInformation("Direction: {dir}", dir);
        _logger.LogInformation("Version: {versionLabel}", versionLabel);

        EnsureDatabase();

        switch (dir)
        {
            case MigrationDirection.Up:
                UpdateDatabase(version);
                break;
            case MigrationDirection.Down:
                if (!version.HasValue)
                {
                    throw new ArgumentException("Verison required for database downgrade");
                }
                DowngradeDatabase(version.Value);
                break;
            default:
                throw new ArgumentException($"Unexpected mirgration direction {dir}");
        }

        _logger.LogInformation("Migrations done!");
    }
}
