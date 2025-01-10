using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Email.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BuildingBlocks.Email;

/// <summary>
/// An email sender implementation using SendGrid.
/// </summary>
public class SendGridEmailSender(
    IOptions<EmailOptions> emailConfig,
    ILogger<SendGridEmailSender> logger)
    : IEmailSender
{
    private readonly EmailOptions _config = emailConfig.Value;

    /// <summary>
    /// Gets an instance of the SendGrid client configured with the API key.
    /// </summary>
    private SendGridClient SendGridClient => new(_config.SendGridOptions?.ApiKey);

    /// <summary>
    /// Sends an email asynchronously using SendGrid.
    /// </summary>
    /// <param name="emailObject">The email object containing the email details.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SendAsync(EmailObject emailObject)
    {
        emailObject.NotBeNull();

        var message = new SendGridMessage
        {
            Subject = emailObject.Subject,
            HtmlContent = emailObject.MailBody
        };

        message.AddTo(new EmailAddress(emailObject.ReceiverEmail));
        message.From = new EmailAddress(emailObject.SenderEmail ?? _config.From);
        message.ReplyTo = new EmailAddress(emailObject.SenderEmail ?? _config.From);

        await SendGridClient.SendEmailAsync(message);

        logger.LogInformation(
            "Email sent. From: {From}, To: {To}, Subject: {Subject}, Content: {Content}",
            _config.From,
            emailObject.ReceiverEmail,
            emailObject.Subject,
            emailObject.MailBody
        );
    }
}