using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace PassBot;
public class SendNotification
{

    public SendNotification()
    {
    }

    [FunctionName("SendNotification")]
    [return: TwilioSms(
        AccountSidSetting = "TwilioAccountSid",
        AuthTokenSetting = "TwilioAuthToken"
    )]
    public CreateMessageOptions Run([QueueTrigger("available-times")] SlottiAvailableDate availableTime, ILogger log)
    {
        log.LogInformation($"PassBot triggered at {DateTime.Now}");
        log.LogInformation($"Available time found for {availableTime.Date}");

        string toPhoneNumber = Environment.GetEnvironmentVariable("ToPhoneNumber", EnvironmentVariableTarget.Process);
        string fromPhoneNumber = Environment.GetEnvironmentVariable("FromPhoneNumber", EnvironmentVariableTarget.Process);
        var message = new CreateMessageOptions(new PhoneNumber(toPhoneNumber))
        {
            From = new PhoneNumber(fromPhoneNumber),
            Body = $"Tillgänglig tid hittad för {availableTime.Date}!\nTillgänglig(a) tider: {string.Join(", ", availableTime.AvailableTimes.Select(time => time.Start.ToShortTimeString()))}\n\nSkynda och boka på https://slotti.fi/booking/suomensuurlahetystotukholma"
        };
        return message;
    }
}