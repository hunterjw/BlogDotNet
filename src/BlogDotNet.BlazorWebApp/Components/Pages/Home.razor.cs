using BlogDotNet.DataServices.Abstractions.Interfaces;
using BlogDotNet.DataServices.Abstractions.Models;
using Microsoft.AspNetCore.Components;

namespace BlogDotNet.BlazorWebApp.Components.Pages;

public partial class Home
{
    [Inject]
    public IBlogPostService? BlogPostService { get; set; }

    [SupplyParameterFromQuery(Name = "take")]
    public int? Take { get; set; }

    [SupplyParameterFromQuery(Name = "after")]
    public Guid? After { get; set; }

    [SupplyParameterFromQuery(Name = "before")]
    public Guid? Before { get; set; }

    public Listing<BlogPost>? BlogPosts { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (BlogPostService == null)
        {
            return;
        }

        BlogPosts = await BlogPostService.GetBlogPosts(take: Take ?? 25, after: After, before: Before);
    }
}
