using MailKit.Security;

namespace MqttAnomalyApi.Services;

using MailKit.Net.Smtp;
using MimeKit;

public class EmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendAnomalyAlert(double temperature)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse("hello@demomailtrap.co"));
        message.To.Add(MailboxAddress.Parse("dop4min34@gmail.com"));
        message.Subject = "Anomali Uyarısı";
        message.Body = new TextPart("plain")
        {
            Text = $"⚠️ Yüksek sıcaklık tespit edildi: {temperature}°C"
        };

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync("live.smtp.mailtrap.io", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("api", "e0a10e8e93f631f5d412f72a80f9c265"); 
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("📧 Mailtrap üzerinden anomali bildirimi gönderildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Mail gönderimi başarısız: {ex.Message}");
        }
    }
}
