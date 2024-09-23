namespace BlogDotNet.Database.Migrations;

/// <summary>
/// Migration direction
/// </summary>
public enum MigrationDirection
{
    /// <summary>
    /// Up - apply migrations
    /// </summary>
    Up = 0,

    /// <summary>
    /// Down - revert migrations
    /// </summary>
    Down = 1,
}
