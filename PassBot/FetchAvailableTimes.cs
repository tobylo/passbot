using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace PassBot;

[StorageAccount("AzureWebJobsStorage")]
public class FetchAvailableTimes
{
    private static HttpClient _client = null;
    private static HashSet<Int32> _sentNotifications;

    public FetchAvailableTimes()
    {
        _sentNotifications = _sentNotifications ?? new HashSet<Int32>();
        if (_client is null)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:100.0) Gecko/20100101 Firefox/100.0");
        }
    }

    [FunctionName("FetchAvailableTimes")]
    public async Task Run(
        [TimerTrigger("*/3 * * * * *", RunOnStartup = true)] TimerInfo myTimer,
        [Queue("available-times")] ICollector<SlottiAvailableDate> availableTimesQueue,
        ILogger log)
    {
        log.LogInformation($"PassBot triggered at {DateTime.Now}");

        var responseMessage = await _client.PostAsJsonAsync("https://slotti.fi/booking/suomensuurlahetystotukholma/api/v4/bookings/bookabletimes", new
        {
            start = DateTime.UtcNow.Date.ToString("yyyy-MM-dd"),
            end = "2022-06-25",
            serviceLinks = new List<Object>{
                    new {
                        type = "primary",
                        serviceGuid = "62033cdf-d55e-4f75-a64b-888103726098",
                        durationMinutes = 20
                    }
                },
            onlyForResourceGuid = (object)null,
            onlyForLocationGuid = "d6b675ee-4231-4856-851b-370cd0b8a2ff"
        });

        log.LogInformation($"{(responseMessage.IsSuccessStatusCode ? "Successful" : "Failed")} response from slotti.fi");

        if (responseMessage.IsSuccessStatusCode)
        {
            var streamTask = responseMessage.Content.ReadAsStreamAsync();
            var availableTimes = await JsonSerializer.DeserializeAsync<SlottiAvailableDate[]>(await streamTask);

            if (availableTimes.Length == 0)
            {
                log.LogInformation("Still no available times..");
                return;
            }

            log.LogInformation($"Found {availableTimes.Length} available dates!");

            foreach (var available in availableTimes)
            {
                if (_sentNotifications.Add(available.GetHashCode()))
                {
                    log.LogInformation($"New timeslot found!");
                    availableTimesQueue.Add(available);
                }
                else
                {
                    log.LogInformation("Time already notified about, skipping..");
                }
            }

            return;
        }

        log.LogInformation($"Received bad response from slotti.fi. {responseMessage.ReasonPhrase}");
        return;
    }
}
