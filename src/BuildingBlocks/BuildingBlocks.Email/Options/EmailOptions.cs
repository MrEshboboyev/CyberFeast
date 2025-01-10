namespace BuildingBlocks.Email.Options;

/// <summary>
/// Defines options for configuring email settings.
/// </summary>
public class EmailOptions
{
    /// <summary>
    /// Gets or sets the MailKit options.
    /// </summary>
    public MailKitOptions? MimeKitOptions { get; set; }

    /// <summary>
    /// Gets or sets the SendGrid options.
    /// </summary>
    public SendGridOptions? SendGridOptions { get; set; }

    /// <summary>
    /// Gets or sets the sender email address.
    /// </summary>
    public string? From { get; set; }

    /// <summary>
    /// Gets or sets the display name for the sender.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether email services are enabled.
    /// </summary>
    public bool Enable { get; set; }
}