using Microsoft.AspNetCore.Components;

namespace BlogDotNet.BlazorWebApp.Components.Shared;

public partial class CustomPageTitle
{
    [Inject]
    public BlogOptions? BlogSettings { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
