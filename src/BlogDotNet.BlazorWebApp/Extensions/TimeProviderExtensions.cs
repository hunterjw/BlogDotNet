namespace BlogDotNet.BlazorWebApp.Extensions;

public static class TimeProviderExtensions
{
    /// <summary>
    /// Convert <paramref name="dateTime"/> to a local <see cref="DateTime"/>
    /// </summary>
    /// <param name="timeProvider">Time provider</param>
    /// <param name="dateTime"><see cref="DateTime"/> to convert</param>
    /// <returns>Updated <see cref="DateTime"/></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static DateTime ToLocalDateTime(this TimeProvider timeProvider, DateTime dateTime)
    {
        return dateTime.Kind switch
        {
            DateTimeKind.Unspecified => throw new InvalidOperationException("Unable to convert unspecified DateTime to local time"),
            DateTimeKind.Local => dateTime,
            _ => DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeProvider.LocalTimeZone), DateTimeKind.Local),
        };
    }

    /// <summary>
    /// Convert <paramref name="dateTime"/> to a local <see cref="DateTimeOffset"/>
    /// </summary>
    /// <param name="timeProvider">Time provider</param>
    /// <param name="dateTime"><see cref="DateTimeOffset"/> to convert</param>
    /// <returns>Updated <see cref="DateTimeOffset"/></returns>
    public static DateTime ToLocalDateTime(this TimeProvider timeProvider, DateTimeOffset dateTime)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(dateTime.UtcDateTime, timeProvider.LocalTimeZone);
        local = DateTime.SpecifyKind(local, DateTimeKind.Local);
        return local;
    }
}
