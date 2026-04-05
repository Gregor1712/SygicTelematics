using Alert.Application.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Alert.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAlertEmailAsync(string alertType, string message, Guid vehicleId, DateTime timestamp)
    {
        var smtpHost = _config["Email:SmtpHost"];
        var smtpPort = int.Parse(_config["Email:SmtpPort"] ?? "587");
        var smtpUser = _config["Email:SmtpUser"];
        var smtpPass = _config["Email:SmtpPass"];
        var from = _config["Email:From"] ?? smtpUser;
        var to = _config["Email:To"];

        if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(to))
        {
            Console.WriteLine("[Email] SMTP not configured, skipping email notification.");
            return;
        }

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(from));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = $"[Vehicle Alert] {alertType} - Vehicle {vehicleId.ToString()[..8]}";

        email.Body = new TextPart("html")
        {
            Text = $"""
                <h2 style="color: #d32f2f;">Vehicle Alert: {alertType}</h2>
                <table style="border-collapse: collapse; font-family: Arial;">
                    <tr><td style="padding: 8px; font-weight: bold;">Vehicle ID:</td><td style="padding: 8px;">{vehicleId}</td></tr>
                    <tr><td style="padding: 8px; font-weight: bold;">Alert Type:</td><td style="padding: 8px;">{alertType}</td></tr>
                    <tr><td style="padding: 8px; font-weight: bold;">Message:</td><td style="padding: 8px;">{message}</td></tr>
                    <tr><td style="padding: 8px; font-weight: bold;">Time:</td><td style="padding: 8px;">{timestamp:yyyy-MM-dd HH:mm:ss} UTC</td></tr>
                </table>
                <p style="color: #666; margin-top: 20px;">This is an automated alert from Sygic Telematics.</p>
                """
        };

        using var smtp = new SmtpClient();
        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
        await smtp.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

        if (!string.IsNullOrEmpty(smtpUser))
            await smtp.AuthenticateAsync(smtpUser, smtpPass);

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

        Console.WriteLine($"[Email] Alert notification sent to {to}");
    }
}
