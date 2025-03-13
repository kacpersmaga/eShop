namespace eShop.Shared.Abstractions.Interfaces.Communication;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
}