using BuildingBlocks.Core.Extensions;

namespace BuildingBlocks.Email;

/// <summary>
/// Defines the contract for an email sender service.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Sends an email message asynchronously.
    /// </summary>
    /// <param name="emailObject">The email message to send.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendAsync(EmailObject emailObject);
}

/// <summary>
/// Represents an email message.
/// </summary>
public class EmailObject
{
    /// <summary>
    /// Initializes a new instance of the EmailObject class.
    /// </summary>
    /// <param name="receiverEmail">The receiver email address.</param>
    /// <param name="subject">The email subject.</param>
    /// <param name="mailBody">The email body/content.</param>
    public EmailObject(string receiverEmail, string subject, string mailBody)
    {
        ReceiverEmail = receiverEmail.NotBeNullOrWhiteSpace();
        Subject = subject.NotBeNullOrWhiteSpace();
        MailBody = mailBody.NotBeNull();
    }

    /// <summary>
    /// Initializes a new instance of the EmailObject class.
    /// </summary>
    /// <param name="receiverEmail">The receiver email address.</param>
    /// <param name="senderEmail">The sender email address.</param>
    /// <param name="subject">The email subject.</param>
    /// <param name="mailBody">The email body/content.</param>
    public EmailObject(string receiverEmail, string senderEmail, string subject, string mailBody)
    {
        ReceiverEmail = receiverEmail.NotBeNullOrWhiteSpace();
        Subject = subject.NotBeNullOrWhiteSpace();
        MailBody = mailBody.NotBeNull();
        SenderEmail = senderEmail.NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// Gets the receiver email address.
    /// </summary>
    public string ReceiverEmail { get; }

    /// <summary>
    /// Gets the sender email address.
    /// </summary>
    public string SenderEmail { get; } = default!;

    /// <summary>
    /// Gets the email subject.
    /// </summary>
    public string Subject { get; }

    /// <summary>
    /// Gets the email body/content.
    /// </summary>
    public string MailBody { get; }
}