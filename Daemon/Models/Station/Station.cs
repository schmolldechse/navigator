using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Daemon.Models.Station;

[BsonIgnoreExtraElements]
public class StationDocument : Station
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required ObjectId Id { get; set; }

    [BsonElement("queryingEnabled")] public bool? QueryingEnabled { get; set; }

    [BsonElement("lastQueried")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastQueried { get; set; }
}

[BsonIgnoreExtraElements]
public class Station
{
    [Required] [BsonElement("name")] public required string Name { get; set; }

    [Required] [BsonElement("evaNumber")] public required int EvaNumber { get; set; }

    [BsonElement("products")]
    [BsonRepresentation(BsonType.String)]
    public List<TransportProducts>? Products { get; set; }

    [BsonElement("ril100")]
    [BsonRepresentation(BsonType.String)]
    public List<string>? Ril100Identifier { get; set; }
}