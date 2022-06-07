using System;
using System.Text.Json.Serialization;

namespace PassBot;

public class SlottiAvailableTime
{
    [JsonPropertyName("start")]
    public DateTime Start { get; set; }

    [JsonPropertyName("end")]
    public DateTime End { get; set; }
    
    [JsonPropertyName("resourceGuid")]
    public string ResourceGuid { get; set; }
    
    [JsonPropertyName("resourceId")]
    public int ResourceId { get; set; }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(Start, End, ResourceGuid);
    }
}
