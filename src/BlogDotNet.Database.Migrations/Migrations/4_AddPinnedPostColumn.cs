using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDotNet.Database.Migrations.Migrations;

[Migration(4, "Add `pinned` column to `blog_post` table")]
public class AddPinnedPostColumn : Migration
{
    public override void Down()
    {
        Execute.EmbeddedScript("BlogDotNet.Database.Migrations.Scripts.3_ViewRankedBlogPost_V1_Up.sql");

        Delete
            .Column("pinned")
            .FromTable("blog_post")
            .InSchema("blogdotnet");
    }

    public override void Up()
    {
        Create
            .Column("pinned")
                .OnTable("blog_post")
                .InSchema("blogdotnet")
            .AsBoolean()
            .NotNullable()
            .WithDefaultValue(false);

        Execute.EmbeddedScript("BlogDotNet.Database.Migrations.Scripts.4_ViewRankedBlogPost_V2_Up.sql");
    }
}
