using Microsoft.AspNetCore.Components;

namespace BlogDotNet.BlazorWebApp.Components.Layout;

public partial class NavMenu
{
    [Inject]
    public BlogOptions? BlogSettings { get; set; }
}
