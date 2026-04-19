namespace Resort_Hub.Services;

public interface ICustomEmailService
{
    Task SendEmailAsync(string Email, string subject, string htmlMessage);
}
