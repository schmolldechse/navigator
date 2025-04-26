using Daemon.Models.Journey;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

namespace Daemon.Models.Station;

public class Stop : Station
{
    [BsonElement("arrival")] public Time? Arrival { get; set; }

    [BsonElement("departure")] public Time? Departure { get; set; }

    [BsonElement("cancelled")] public bool Cancelled { get; set; }

    [BsonElement("separation")] public bool? Separation { get; set; }

    [BsonElement("messages")] public List<Message>? Messages { get; set; }

    public static Stop FromJson(JToken? jsonObject, bool? parseFull = false)
    {
        if (jsonObject == null)
            throw new ArgumentNullException(nameof(jsonObject), "JSON object for Stop cannot be null");

        return new Stop()
        {
            EvaNumber = jsonObject["id"]?.ToObject<int>() ?? jsonObject["stop"]?["id"]?.ToObject<int>() ??
                throw new ArgumentException("Missing 'id' field in Stop"),
            Name = jsonObject["name"]?.ToString() ?? jsonObject["stop"]?["name"]?.ToString() ??
                throw new ArgumentException("Missing 'name' field in Stop"),
            Cancelled = jsonObject["cancelled"]?.ToObject<bool>() ?? false,
            Departure = parseFull == true ? Time.FromJson(jsonObject, true) : null,
            Arrival = parseFull == true ? Time.FromJson(jsonObject, false) : null,
            Messages = parseFull == true
                ? jsonObject["remarks"]?.Select(Message.FromJson)?.ToList()
                : null,
        };
    }
}