using System.ComponentModel.DataAnnotations;
using Daemon.Models.Station;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;
using ZstdSharp.Unsafe;

namespace Daemon.Models.Journey;

[BsonIgnoreExtraElements]
public class Trip
{
    [Required]
    [BsonId]
    [BsonElement("ris_journeyId")]
    public required string RisJourneyId { get; set; }

    [BsonElement("origin")] public Stop Origin { get; set; }

    [BsonElement("destination")] public Stop Destination { get; set; }

    [BsonElement("lineInformation")] public LineInformation LineInformation { get; set; }

    [BsonElement("viaStops")] public List<Stop>? ViaStops { get; set; }

    [BsonElement("messages")] public List<Message>? Messages { get; set; }

    public static Trip FromJson(JToken jsonObject)
    {
        return new Trip()
        {
            RisJourneyId = jsonObject["id"]!.ToString(),
            Origin = Stop.FromJson(jsonObject["origin"]!),
            Destination = Stop.FromJson(jsonObject["destination"]!),
            LineInformation = LineInformation.FromJson(jsonObject["line"]!),
            ViaStops = jsonObject["stopovers"]?.Select(stop => Stop.FromJson(stop, true))?.ToList(),
            Messages = jsonObject["remarks"]?.Select(Message.FromJson)?.ToList(),
        };
    }
}