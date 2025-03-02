using eShop.Services.Interfaces;
using eShop.Validators.Dtos;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace eShop.Services.Implementations;
public class PhoneService : IPhoneService
{
    private readonly TwilioSettings _twilioSettings;

    public PhoneService(IOptions<TwilioSettings> twilioSettings)
    {
        _twilioSettings = twilioSettings.Value;
        TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);
    }

    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        await MessageResource.CreateAsync(
            body: message,
            from: new PhoneNumber(_twilioSettings.FromPhoneNumber),
            to: new PhoneNumber(phoneNumber)
        );
    }
}
