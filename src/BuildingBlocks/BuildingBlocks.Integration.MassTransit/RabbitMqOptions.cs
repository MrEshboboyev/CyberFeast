namespace BuildingBlocks.Integration.MassTransit;

/// <summary>
/// Configures RabbitMQ connection settings.
/// </summary>
public class RabbitMqOptions
{
    /// <summary>
    /// Gets or sets the RabbitMQ host. Defaults to "localhost".
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets the RabbitMQ port. Defaults to 5672.
    /// </summary>
    public ushort Port { get; set; } = 5672;

    /// <summary>
    /// Gets or sets the RabbitMQ username. Defaults to "guest".
    /// </summary>
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// Gets or sets the RabbitMQ password. Defaults to "guest".
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// Gets the RabbitMQ connection string.
    /// </summary>
    public string ConnectionString => $"amqp://{UserName}:{Password}@{Host}:{Port}/";
}