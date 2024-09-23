using FluentMigrator;

namespace BlogDotNet.Database.Migrations.Migrations;

[Migration(2, "Create the `blog_post` table")]
public class CreateBlogPostTable : Migration
{
    /// <inheritdoc/>
    public override void Down()
    {
        Delete
            .Table("blog_post")
            .InSchema("blogdotnet");
    }

    /// <inheritdoc/>
    public override void Up()
    {
        Create
            .Table("blog_post")
            .InSchema("blogdotnet")
            .WithColumn("id")
                .AsGuid()
                .NotNullable()
                .Unique()
                .PrimaryKey()
            .WithColumn("title")
                .AsString()
                .Nullable()
            .WithColumn("description")
                .AsString()
                .Nullable()
            .WithColumn("pub_date")
                .AsDateTimeOffset()
                .NotNullable()
                .Indexed()
            .WithColumn("slug")
                .AsString()
                .NotNullable()
                .Unique()
            .WithColumn("file_path")
                .AsString()
                .NotNullable()
                .Unique()
            .WithColumn("content_file_path")
                .AsString()
                .Nullable()
            .WithColumn("content_md")
                .AsString()
                .Nullable()
            .WithColumn("content_html")
                .AsString()
                .Nullable();
    }
}
