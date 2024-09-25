using System.Globalization;

namespace BlogDotNet.BlazorWebApp;

/// <summary>
/// Browser time provider
/// </summary>
internal sealed class BrowserTimeProvider : TimeProvider
{
    private TimeZoneInfo? _browserLocalTimeZone;
    private CultureInfo? _browserLocalCultureInfo;

    // Notify when the local time zone changes
    public event EventHandler? LocalTimeZoneChanged;

    public override TimeZoneInfo LocalTimeZone
        => _browserLocalTimeZone ?? base.LocalTimeZone;

    public CultureInfo LocalCultureInfo 
        => _browserLocalCultureInfo ?? CultureInfo.InvariantCulture;

    internal bool IsLocalTimeZoneSet => _browserLocalTimeZone != null;

    internal bool IsLocalCultureInfoSet => _browserLocalCultureInfo != null;

    // Set the local time zone
    public void SetBrowserTimeZone(string timeZone, string locale)
    {
        if (!TimeZoneInfo.TryFindSystemTimeZoneById(timeZone, out var timeZoneInfo))
        {
            timeZoneInfo = null;
        }

        CultureInfo cultureInfo = CultureInfo.GetCultureInfo(locale);

        if (timeZoneInfo != LocalTimeZone || cultureInfo != LocalCultureInfo)
        {
            _browserLocalTimeZone = timeZoneInfo;
            _browserLocalCultureInfo = cultureInfo;
            LocalTimeZoneChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
