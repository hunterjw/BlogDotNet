using BlogDotNet.DataServices.Abstractions.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlogDotNet.BlazorWebApp.Components.Layout;

public partial class NavMenu
{
    [Inject]
    public BlogOptions? BlogSettings { get; set; }

    [Inject]
    public IBlogPostService? BlogPostService { get; set; }

    public bool HasAboutPage { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (BlogPostService != null)
        {
            HasAboutPage = await BlogPostService.HasAboutPage();
        }
    }
}
