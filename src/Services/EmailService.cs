using SendGrid;
using SendGrid.Helpers.Mail;
using Serilog;
using ILogger = Serilog.ILogger;

using Locker.Models.Entities;

namespace Locker.Services;

public class EmailService : IEmailService
{
    private static readonly EmailAddress from = new EmailAddress("brianreece889@gmail.com");

    private readonly ILogger _logger = Log.Logger.ForContext<EmailService>();
    private readonly ISendGridClient _sendGrid;

    public EmailService(ISendGridClient sendGrid) =>
        _sendGrid = sendGrid;

    public async Task SendConfirmationEmail(User user, string context, string token)
    {
        var to = new EmailAddress(user.Email);
        var subject = $"{context} - Confirm your email address";
        var text = "Confirm your email address";
        var content = @$"<a href=""https://locker.bdreece.dev/todo?token={token}"">Confirm</a>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, text, content);
        var response = await _sendGrid.SendEmailAsync(msg);
        if (!response.IsSuccessStatusCode)
            throw new Exception(response.ToString());
    }

    public async Task SendPasswordResetEmail(User user, string context, string token)
    {
        var to = new EmailAddress(user.Email);
        var subject = $"{context} - Reset your password";
        var text = "Confirm your email address";
        var content = @$"<a href=""https://locker.bdreece.dev/todo?token={token}"">Confirm</a>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, text, content);
        var response = await _sendGrid.SendEmailAsync(msg);
        if (!response.IsSuccessStatusCode)
            throw new Exception(response.ToString());
    }

}