﻿namespace eShop.Shared.Settings;

public class TwilioSettings
{
    public required string AccountSid { get; set; }
    public required string AuthToken { get; set; }
    public required string FromPhoneNumber { get; set; }
}