namespace BuildingBlocks.Email;

/// <summary>
/// Defines the available email providers.
/// </summary>
public enum EmailProvider
{
    /// <summary>
    /// SendGrid email provider.
    /// </summary>
    SendGrid = 1,

    /// <summary>
    /// MailKit email provider.
    /// </summary>
    MimKit = 2
}