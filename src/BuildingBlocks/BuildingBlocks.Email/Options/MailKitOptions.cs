namespace BuildingBlocks.Email.Options;

/// <summary>
/// Defines the configuration options for MailKit.
/// </summary>
public class MailKitOptions
{
    /// <summary>
    /// Gets or sets the SMTP host.
    /// </summary>
    public required string Host { get; init; }

    /// <summary>
    /// Gets or sets the SMTP port.
    /// </summary>
    public required int Port { get; init; }

    /// <summary>
    /// Gets or sets the SMTP username.
    /// </summary>
    public required string UserName { get; init; }

    /// <summary>
    /// Gets or sets the SMTP password.
    /// </summary>
    public required string Password { get; init; }
}