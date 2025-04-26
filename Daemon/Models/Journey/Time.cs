using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

namespace Daemon.Models.Journey;

public class Time
{
    [BsonElement("plannedTime")] public DateTime? PlannedTime { get; set; }

    [BsonElement("actualTime")] public DateTime? ActualTime { get; set; }

    [BsonElement("delay")] public int? Delay { get; set; }

    [BsonElement("plannedPlatform")] public string? PlannedPlatform { get; set; }

    [BsonElement("actualPlatform")] public string? ActualPlatform { get; set; }

    public static Time FromJson(JToken? jsonObject, bool isDeparture)
    {
        if (jsonObject == null) throw new ArgumentNullException(nameof(jsonObject), "JSON object for Time cannot be null");
        
        var keys = new
        {
            PlannedTime = isDeparture ? "plannedDeparture" : "plannedArrival",
            ActualTime = isDeparture ? "departure" : "arrival",
            Delay = isDeparture ? "departureDelay" : "arrivalDelay",
            PlannedPlatform = isDeparture ? "plannedDeparturePlatform" : "plannedArrivalPlatform",
            ActualPlatform = isDeparture ? "departurePlatform" : "arrivalPlatform"
        };
        
        return new Time()
        {
            PlannedTime = !string.IsNullOrEmpty(jsonObject[keys.PlannedTime]?.ToString()) ? DateTime.Parse(jsonObject[keys.PlannedTime]!.ToString()).ToLocalTime() : null,
            ActualTime = !string.IsNullOrEmpty(jsonObject[keys.ActualTime]?.ToString()) ? DateTime.Parse(jsonObject[keys.ActualTime]!.ToString()).ToLocalTime() : null,
            Delay = jsonObject[keys.Delay]?.ToObject<int?>() ?? 0,
            PlannedPlatform = jsonObject[keys.PlannedPlatform]?.ToString() ?? null,
            ActualPlatform = jsonObject[keys.ActualPlatform]?.ToString() ?? null,
        };
    }
}