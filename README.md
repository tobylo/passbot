# Passbot for slotti.fi

## Description

Passbot is a simple automation that polls the slotti.fi API for available times and notifies a user via SMS when a new available time appears.

Hardcoded for looking for available times at the Finnish embassy in Stockholm. Only contains an in memory hashset of "known" times, meaning a restart of the application would re-send "old" notifications to the recepient.

## Example local.settings.json

```json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "TwilioAccountSid": "<replace with twilio account sid>", 
        "TwilioAuthToken": "<replace with twilio auth token>",
        "ToPhoneNumber": "<replace with international phone number to receive the notification, e.g. +461234512345>",
        "FromPhoneNumber": "<replace with your twilio phone number>"
    }
}
```