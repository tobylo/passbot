using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PassBot;

public class SlottiAvailableDate
{
    [JsonPropertyName("date")]
    public string Date { get; set; }
    
    [JsonPropertyName("availableTimes")]
    public List<SlottiAvailableTime> AvailableTimes { get; set; }

    public override int GetHashCode()
    {
        int timesHc = 0;
        if(AvailableTimes is not null)
        {
            foreach(var time in AvailableTimes)
                timesHc ^= time.GetHashCode();
        }
        return System.HashCode.Combine(Date, timesHc);
    }
}