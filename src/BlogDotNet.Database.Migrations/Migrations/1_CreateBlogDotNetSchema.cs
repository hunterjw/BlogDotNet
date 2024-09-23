using FluentMigrator;

namespace BlogDotNet.Database.Migrations.Migrations;

[Migration(1, "Create the `blogdotnet` schema")]
public class CreateBlogDotNetSchema : Migration
{
    /// <inheritdoc/>
    public override void Down()
    {
        Delete.Schema("blogdotnet");
    }

    /// <inheritdoc/>
    public override void Up()
    {
        Create.Schema("blogdotnet");
    }
}
