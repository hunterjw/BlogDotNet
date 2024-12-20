﻿using BlogDotNet.DataServices.Abstractions.Interfaces;
using BlogDotNet.DataServices.Abstractions.Models;
using Microsoft.AspNetCore.Components;

namespace BlogDotNet.BlazorWebApp.Components.Pages;

public partial class Post
{
    [Inject]
    public IBlogPostService? BlogPostService { get; set; }

    [Parameter]
    public string? Slug { get; set; }

    public BlogPost? BlogPost { get; set; }

    public bool NotFound { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (BlogPostService != null && !string.IsNullOrWhiteSpace(Slug) && Slug != "about")
        {
            BlogPost = await BlogPostService.GetBlogPostBySlug(Slug);
        }
        NotFound = BlogPost == null;
    }
}
