using BuildingBlocks.Email.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BuildingBlocks.Email;

/// <summary>
/// An email sender implementation using MailKit.
/// </summary>
public class MailKitEmailSender(
    IOptions<EmailOptions> config,
    ILogger<MailKitEmailSender> logger) : IEmailSender
{
    private readonly EmailOptions _config = config.Value;

    /// <summary>
    /// Sends an email asynchronously using MailKit.
    /// </summary>
    /// <param name="emailObject">The email object containing the email details.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SendAsync(EmailObject emailObject)
    {
        try
        {
            if (_config.MimeKitOptions == null)
            {
                throw new Exception("MimeKitOptions is empty.");
            }

            // Create the email message
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailObject.SenderEmail ?? _config.From);
            email.To.Add(MailboxAddress.Parse(emailObject.ReceiverEmail));
            email.Subject = emailObject.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = emailObject.MailBody
            };
            email.Body = builder.ToMessageBody();

            // Connect to the SMTP server and send the email
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _config.MimeKitOptions.Host,
                _config.MimeKitOptions.Port,
                SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(
                _config.MimeKitOptions.UserName,
                _config.MimeKitOptions.Password);
            smtp.DeliveryStatusNotificationType = DeliveryStatusNotificationType.Full;
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            logger.LogInformation(
                "Email sent. From: {From}, To: {To}, Subject: {Subject}, Content: {Content}",
                _config.From,
                emailObject.ReceiverEmail,
                emailObject.Subject,
                emailObject.MailBody
            );
        }
        catch (Exception ex)
        {
            logger.LogError("An error occurred while sending email: {Message}", ex.Message);
            throw;
        }
    }
}