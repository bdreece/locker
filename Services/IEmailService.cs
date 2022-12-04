using Locker.Models.Entities;

namespace Locker.Services;

public interface IEmailService
{
    Task SendConfirmationEmail(User user, string context, string token);
    Task SendPasswordResetEmail(User user, string context, string token);
}