using FluentMigrator;

namespace BlogDotNet.Database.Migrations.Migrations;

[Migration(5, "Add `hidden` column to `blog_post` table")]
public class AddHiddenPostColumn : Migration
{
    public override void Down()
    {
        // Drop the view temporarily before we drop the column
        Execute.Sql("DROP VIEW IF EXISTS blogdotnet.view_ranked_blog_post");

        Delete
            .Column("hidden")
            .FromTable("blog_post")
            .InSchema("blogdotnet");

        // Regenerate the ranked view
        Execute.EmbeddedScript("BlogDotNet.Database.Migrations.Scripts.4_ViewRankedBlogPost_V2_Up.sql");
    }

    public override void Up()
    {
        Create
            .Column("hidden")
                .OnTable("blog_post")
                .InSchema("blogdotnet")
            .AsBoolean()
            .NotNullable()
            .WithDefaultValue(false);

        // Regenerate the ranked view to include the new column
        Execute.EmbeddedScript("BlogDotNet.Database.Migrations.Scripts.4_ViewRankedBlogPost_V2_Up.sql");
    }
}
