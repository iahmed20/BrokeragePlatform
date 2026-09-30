// Services/EmailSender.cs
using System.Net;
using System.Net.Mail;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string htmlBody);
}

public class SmtpOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool EnableSsl { get; set; } = true;
    public string From { get; set; } = "";
}

// Sends real email through any SMTP provider (Gmail app password, SendGrid, Mailtrap, ...)
public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;

    public SmtpEmailSender(SmtpOptions options)
    {
        _options = options;
    }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = string.IsNullOrEmpty(_options.Username)
                ? null
                : new NetworkCredential(_options.Username, _options.Password),
        };

        using var message = new MailMessage(_options.From, to, subject, htmlBody) { IsBodyHtml = true };
        await client.SendMailAsync(message);
    }
}

// Development fallback when no SMTP host is configured: writes the email to the log
public class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string to, string subject, string htmlBody)
    {
        _logger.LogWarning("No SMTP configured - email to {To} not sent.\nSubject: {Subject}\n{Body}", to, subject, htmlBody);
        return Task.CompletedTask;
    }
}
