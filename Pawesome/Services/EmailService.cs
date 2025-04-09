using Pawesome.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Pawesome.Services;

/// <summary>
/// Service for sending email notifications
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    /// <summary>
    /// Initializes a new instance of the EmailService
    /// </summary>
    /// <param name="configuration">The application configuration provider</param>
    /// <param name="logger">The logger for recording email operations</param>
    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Sends an email message to the specified recipient
    /// </summary>
    /// <param name="email">The recipient's email address</param>
    /// <param name="subject">The subject line of the email</param>
    /// <param name="htmlMessage">The HTML content of the email body</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <exception cref="Exception">Thrown when the email fails to send</exception>
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var host = _configuration["Email:Host"] ?? "localhost";
        var port = int.Parse(_configuration["Email:Port"] ?? "1025");
        var enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "false");
        var userName = _configuration["Email:UserName"];
        var password = _configuration["Email:Password"];
        var senderEmail = _configuration["Email:SenderEmail"] ?? "noreply@pawesome.local";
        var senderName = _configuration["Email:SenderName"] ?? "Pawesome";

        _logger.LogInformation("Tentative d'envoi d'email à {Email}", email);

        using var client = new SmtpClient(host);
        client.Port = port;
        client.EnableSsl = enableSsl;

        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
        {
            client.Credentials = new NetworkCredential(userName, password);
        }

        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail, senderName),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        mailMessage.To.Add(email);
        await client.SendMailAsync(mailMessage);
    }
}
