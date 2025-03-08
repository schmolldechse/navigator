using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Daemon.Models;

[BsonIgnoreExtraElements]
public class StationDocument : Station
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("queryingEnabled")]
    public bool? QueryingEnabled { get; set; }

    [BsonElement("lastQueried")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastQueried { get; set; }
}

[BsonIgnoreExtraElements]
public class Station
{
    [Required] [BsonElement("name")] public string Name { get; set; }

    [Required]
    [BsonElement("evaNumber")]
    public int EvaNumber { get; set; }

    [BsonElement("locationId")] public string? LocationId { get; set; }

    [BsonElement("coordinates")] public Coordinates? Coordinates { get; set; }

    [BsonElement("products")]
    [BsonRepresentation(BsonType.String)]
    public List<TransportProducts>? Products { get; set; }
}

public class Coordinates
{
    [BsonElement("latitude")] public double Latitude { get; set; }

    [BsonElement("longitude")] public double Longitude { get; set; }
}

public enum TransportProducts
{
    [BsonElement("HOCHGESCHWINDIGKEITSZUEGE")] [BsonRepresentation(BsonType.String)]
    Hochgeschwindigkeitszuege,

    [BsonElement("INTERCITYUNDEUROCITYZUEGE")] [BsonRepresentation(BsonType.String)]
    IntercityUndEurocityZuege,

    [BsonElement("INTERREGIOUNDSCHNELLZUEGE")] [BsonRepresentation(BsonType.String)]
    InterregioUndSchnellzuege,

    [BsonElement("NAHVERKEHRSONSTIGEZUEGE")] [BsonRepresentation(BsonType.String)]
    NahverkehrsonstigeZuege,

    [BsonElement("SBAHNEN")] [BsonRepresentation(BsonType.String)]
    Sbahnen,

    [BsonElement("BUSSE")] [BsonRepresentation(BsonType.String)]
    Busse,

    [BsonElement("SCHIFFE")] [BsonRepresentation(BsonType.String)]
    Schiffe,

    [BsonElement("UBAHN")] [BsonRepresentation(BsonType.String)]
    UBahn,

    [BsonElement("STRASSENBAHN")] [BsonRepresentation(BsonType.String)]
    Strassenbahn,

    [BsonElement("ANRUFPFLICHTIGEVERKEHRE")] [BsonRepresentation(BsonType.String)]
    AnrufpflichtigeVerkehre,

    [BsonElement("UNKNOWN")] [BsonRepresentation(BsonType.String)]
    Unknown
}