namespace eShop.Shared.Interfaces.Communication;

public interface IPhoneService
{
    Task SendSmsAsync(string phoneNumber, string message);
}