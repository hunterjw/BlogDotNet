using Markdig;
using Microsoft.AspNetCore.Components;

namespace BlogDotNet.BlazorWebApp.Components.Pages;

public partial class MarkdownTest
{
    [Inject]
    public IWebHostEnvironment? WebHostEnvironment { get; set; }

    public string? PageContent { get; set; } = null;

    protected override async Task OnInitializedAsync()
    {
        if (WebHostEnvironment != null)
        {
            string filePath = Path.Combine(WebHostEnvironment.WebRootPath, "test.md");
            string content = await File.ReadAllTextAsync(filePath);
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseBootstrap()
                .Build();
            PageContent = Markdown.ToHtml(content, pipeline);
        }
    }
}
