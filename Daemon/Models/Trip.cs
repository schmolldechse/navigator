using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Daemon.Models;

[BsonIgnoreExtraElements]
public class Trip
{
    [Required]
    [BsonId]
    [BsonElement("ris_journeyId")]
    public string RIS_JourneyId { get; set; }

    [BsonElement("origin")] public Stop Origin { get; set; }

    [BsonElement("destination")] public Stop Destination { get; set; }

    [BsonElement("lineInformation")] public LineInformation LineInformation { get; set; }

    [BsonElement("viaStops")] public List<Stop> ViaStops { get; set; }

    [BsonElement("messages")] public List<Message> Messages { get; set; }
}

public class Stop : Station
{
    [BsonElement("arrival")] public Time? Arrival { get; set; }

    [BsonElement("departure")] public Time? Departure { get; set; }

    [BsonElement("cancelled")] public bool Cancelled { get; set; }

    [BsonElement("additional")] public bool? Additional { get; set; }

    [BsonElement("separation")] public bool? Separation { get; set; }

    [BsonElement("messages")]
    public List<Message>? Messages { get; set; }
}

public class Time
{
    [BsonElement("plannedTime")] public DateTime PlannedTime { get; set; }

    [BsonElement("actualTime")] public DateTime? ActualTime { get; set; }

    [BsonElement("delay")] public int? Delay { get; set; }

    [BsonElement("plannedPlatform")] public string PlannedPlatform { get; set; }

    [BsonElement("actualPlatform")] public string? ActualPlatform { get; set; }
}

public class Message
{
    [BsonElement("code")] public int Code { get; set; }

    [BsonElement("summary")] public string Summary { get; set; }

    [BsonElement("text")] public string Text { get; set; }
}

public class LineInformation
{
    [BsonElement("type")] public TransportProducts Type { get; set; }

    [BsonElement("product")] public string Product { get; set; }

    [BsonElement("lineName")] public string LineName { get; set; }

    [BsonElement("fahrtNr")] public string FahrtNr { get; set; }

    [BsonElement("operator")] public Operator Operator { get; set; }
}

public class Operator
{
    [BsonElement("id")] public string Id { get; set; }

    [BsonElement("name")] public string Name { get; set; }
}