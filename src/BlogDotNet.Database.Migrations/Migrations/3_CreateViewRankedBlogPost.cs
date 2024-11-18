using FluentMigrator;

namespace BlogDotNet.Database.Migrations.Migrations;

[Migration(3, "Create the view `view_ranked_blog_post`")]
public class CreateRankedBlogPostView : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP VIEW IF EXISTS blogdotnet.view_ranked_blog_post");
    }

    public override void Up()
    {
        Execute.EmbeddedScript("BlogDotNet.Database.Migrations.Scripts.3_ViewRankedBlogPost_V1_Up.sql");
    }
}
