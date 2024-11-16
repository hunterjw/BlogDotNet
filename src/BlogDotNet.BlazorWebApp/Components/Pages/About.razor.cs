using BlogDotNet.DataServices.Abstractions.Interfaces;
using BlogDotNet.DataServices.Abstractions.Models;
using Microsoft.AspNetCore.Components;

namespace BlogDotNet.BlazorWebApp.Components.Pages;

public partial class About
{
    [Inject]
    public IBlogPostService? BlogPostService { get; set; }

    public BlogPost? BlogPost { get; set; }

    public bool NotFound { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (BlogPostService != null)
        {
            BlogPost = await BlogPostService.GetBlogPostBySlug("about");
        }
        NotFound = BlogPost == null;
    }
}
