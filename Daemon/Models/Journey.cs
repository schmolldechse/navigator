using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Daemon.Models;

[BsonIgnoreExtraElements]
public class JourneyDocument
{
    [Required]
    [BsonId]
    [BsonElement("risId")]
    public string risId { get; set; }

    [BsonElement("firstQueried")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? FirstQueried { get; set; }
    
    [BsonElement("lastSuccessfulQueried")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastSuccessfulQueried { get; set; }
}