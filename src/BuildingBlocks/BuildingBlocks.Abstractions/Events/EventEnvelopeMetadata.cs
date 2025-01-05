namespace BuildingBlocks.Abstractions.Events;

/// <summary>
/// Defines metadata for event envelopes.
/// </summary>
/// <param name="MessageId">The unique identifier of the message.</param>
/// <param name="CorrelationId">The correlation identifier for related messages.</param>
/// <param name="MessageType">The type of the message.</param>
/// <param name="Name">The name of the message.</param>
/// <param name="CausationId">The identifier of the message that caused this message (optional).</param>
public record EventEnvelopeMetadata(
    Guid MessageId,
    Guid CorrelationId,
    string MessageType,
    string Name,
    Guid? CausationId)
{
    /// <summary>
    /// Gets or initializes a dictionary of additional metadata headers.
    /// </summary>
    public IDictionary<string, object?> Headers { get; init; } =
        new Dictionary<string, object?>();

    /// <summary>
    /// Gets or initializes the creation date of the message.
    /// </summary>
    public DateTime Created { get; init; } = DateTime.Now;

    /// <summary>
    /// Gets or initializes the creation time in Unix time.
    /// </summary>
    public long? CreatedUnixTime { get; init; } = DateTimeHelper.ToUnixTimeSecond(DateTime.Now);

    /// <summary>
    /// Helper class for converting dates to Unix time.
    /// </summary>
    internal static class DateTimeHelper
    {
        private static readonly DateTime Epoch = new(
            1970,
            1,
            1,
            0,
            0,
            0,
            DateTimeKind.Utc);

        /// <summary>
        /// Converts the specified date to Unix time.
        /// </summary>
        /// <param name="datetime">The date to convert.</param>
        /// <returns>The Unix time representation of the specified date.</returns>
        public static long ToUnixTimeSecond(DateTime datetime)
        {
            var unixTime = (datetime.ToUniversalTime() - Epoch).TotalSeconds;
            return (long)unixTime;
        }
    }
}