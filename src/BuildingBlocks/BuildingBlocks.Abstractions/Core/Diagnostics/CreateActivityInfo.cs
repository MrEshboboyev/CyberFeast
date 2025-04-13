using System.Diagnostics;

namespace BuildingBlocks.Abstractions.Core.Diagnostics;

/// <summary>
/// Represents information required to create a diagnostic <see cref="Activity"/>.
/// This class is typically used to encapsulate metadata such as name, tags, and parent context
/// when starting a new tracing activity.
/// </summary>
public class CreateActivityInfo
{
    /// <summary>
    /// Gets or sets the name of the activity. This will be used as the identifier for the tracing operation.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the set of tags (key-value pairs) associated with the activity.
    /// These tags provide additional contextual metadata for diagnostics and tracing tools.
    /// </summary>
    public IDictionary<string, object?> Tags { get; set; } = new Dictionary<string, object?>();

    /// <summary>
    /// Gets or sets the parent activity's ID, if known. Used to correlate this activity with its parent in distributed tracing scenarios.
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the parent <see cref="ActivityContext"/> used in propagation scenarios.
    /// This is often used in distributed systems to maintain context across service boundaries.
    /// </summary>
    public ActivityContext? Parent { get; set; }

    /// <summary>
    /// Gets or sets the kind of activity, such as Internal, Server, Client, Producer, or Consumer.
    /// This defines how the activity fits into a trace.
    /// Default is <see cref="ActivityKind.Internal"/>.
    /// </summary>
    public required ActivityKind ActivityKind = ActivityKind.Internal;
}
