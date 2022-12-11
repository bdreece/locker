/**
 * locker - A multi-tenant GraphQL authentication & authorization server
 * Copyright (C) 2022 Brian Reece

 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.

 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.

 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
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