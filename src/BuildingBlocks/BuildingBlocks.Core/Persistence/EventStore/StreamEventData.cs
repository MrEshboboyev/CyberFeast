namespace BuildingBlocks.Core.Persistence.EventStore
{
    /// <summary>
    /// Represents the data structure for an event in the event stream.
    /// </summary>
    public class StreamEventData
    {
        /// <summary>
        /// Gets or sets the unique event identifier.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the stream identifier.
        /// </summary>
        public string StreamId { get; set; }

        /// <summary>
        /// Gets or sets the event name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the event data as a byte array.
        /// </summary>
        public byte[] Data { get; set; } = null!;

        /// <summary>
        /// Gets or sets the event metadata as a byte array.
        /// </summary>
        public byte[]? Metadata { get; set; }

        /// <summary>
        /// Gets or sets the name of the event type (e.g., "invoice_issued").
        /// </summary>
        public string EventType { get; set; } = null!;

        /// <summary>
        /// Gets or sets the content type of the event (e.g., application/json).
        /// </summary>
        public string ContentType { get; set; } = null!;

        /// <summary>
        /// Gets or sets the timestamp representing when the event occurred.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the position of the event in the global stream.
        /// </summary>
        public int GlobalEventPosition { get; set; }

        /// <summary>
        /// Gets or sets the event number in the stream (also known as StreamPosition).
        /// </summary>
        public int EventNumber { get; set; }
    }
}