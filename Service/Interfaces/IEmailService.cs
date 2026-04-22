namespace BookStore.Service.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    Task SendPasswordResetEmailAsync(string email, string resetLink);
}