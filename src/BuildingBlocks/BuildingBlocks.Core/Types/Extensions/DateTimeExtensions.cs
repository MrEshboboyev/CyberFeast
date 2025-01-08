namespace BuildingBlocks.Core.Types.Extensions;

/// <summary>
/// Provides extension methods for <see cref="DateTime"/> to convert to and from Unix time.
/// </summary>
public static class DateTimeExtensions
{
    private static readonly DateTime Epoch = new DateTime(
        1970,
        1,
        1,
        0,
        0,
        0,
        DateTimeKind.Utc);

    /// <summary>
    /// Converts the <see cref="DateTime"/> to Unix time in seconds.
    /// </summary>
    /// <param name="datetime">The <see cref="DateTime"/> to convert.</param>
    /// <returns>The Unix time in seconds.</returns>
    public static long ToUnixTimeSecond(this DateTime datetime)
    {
        var unixTime = (datetime.ToUniversalTime() - Epoch).TotalSeconds;
        return (long)unixTime;
    }

    /// <summary>
    /// Converts a nullable Unix time in seconds to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="unixTime">The Unix time in seconds.</param>
    /// <returns>The corresponding <see cref="DateTime"/>.</returns>
    public static DateTime ToDateTime(this long? unixTime) =>
        Epoch.AddSeconds(unixTime ?? ToUnixTimeSecond(DateTime.Now)).ToLocalTime();

    /// <summary>
    /// Converts the <see cref="DateTime"/> to Unix time in milliseconds.
    /// </summary>
    /// <param name="dateTime">The <see cref="DateTime"/> to convert.</param>
    /// <returns>The Unix time in milliseconds.</returns>
    public static long ToUnixTimeMilliseconds(this DateTime dateTime)
    {
        return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
    }
}