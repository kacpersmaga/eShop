namespace eShop.Services.Interfaces;

public interface IPhoneService
{
    Task SendSmsAsync(string phoneNumber, string message);
}