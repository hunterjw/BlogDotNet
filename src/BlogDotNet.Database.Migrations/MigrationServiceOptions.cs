namespace BlogDotNet.Database.Migrations;

/// <summary>
/// Options for <see cref="MigrationService"/>
/// </summary>
public class MigrationServiceOptions
{
    /// <summary>
    /// Postgres admin connection string
    /// </summary>
    public string? PostgresAdminConnectionString { get; set; }

    /// <summary>
    /// Database name
    /// </summary>
    public string? Database { get; set; }

    /// <summary>
    /// Migration direction
    /// </summary>
    public MigrationDirection? Direction { get; set; }

    /// <summary>
    /// Migration version
    /// </summary>
    public long? Version { get; set; }
}
