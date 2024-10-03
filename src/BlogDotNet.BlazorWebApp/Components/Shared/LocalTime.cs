using BlogDotNet.BlazorWebApp.Extensions;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using System.Globalization;
using TimeZoneNames;

namespace BlogDotNet.BlazorWebApp.Components.Shared;

/// <summary>
/// Local time zone display
/// </summary>
public sealed class LocalTime : ComponentBase, IDisposable
{
    [Inject]
    public TimeProvider TimeProvider { get; set; } = default!;

    /// <summary>
    /// DateTimeOffset to display
    /// </summary>
    [Parameter]
    public DateTimeOffset? DateTime { get; set; }

    protected override void OnInitialized()
    {
        if (TimeProvider is BrowserTimeProvider browserTimeProvider)
        {
            browserTimeProvider.LocalTimeZoneChanged += LocalTimeZoneChanged;
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (DateTime != null)
        {
            DateTime localDateTime = TimeProvider.ToLocalDateTime(DateTime.Value);
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo("en-US");
            TimeZoneValues timeZoneNames = TZNames.GetAbbreviationsForTimeZone(TimeProvider.LocalTimeZone.Id, cultureInfo.Name);
            builder.AddContent(0, $"{localDateTime.ToString("g", cultureInfo)} {(TimeProvider.LocalTimeZone.IsDaylightSavingTime(localDateTime) ? timeZoneNames.Daylight : timeZoneNames.Standard)}");
        }
    }

    public void Dispose()
    {
        if (TimeProvider is BrowserTimeProvider browserTimeProvider)
        {
            browserTimeProvider.LocalTimeZoneChanged -= LocalTimeZoneChanged;
        }
    }

    private void LocalTimeZoneChanged(object? sender, EventArgs e)
    {
        _ = InvokeAsync(StateHasChanged);
    }
}