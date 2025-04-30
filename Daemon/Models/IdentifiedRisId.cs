using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

namespace Daemon.Models;

[BsonIgnoreExtraElements]
public class IdentifiedRisId
{
    [Required]
    [BsonId]
    [BsonElement("risId")]
    public required string RisId { get; set; }

    [BsonElement("discoveredAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DiscoveredAt { get; set; }
    
    [BsonElement("lastSuccessfulQueried")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastSuccessfulQueried { get; set; }

    public static IdentifiedRisId FromJson(JToken jsonObject)
    {
        if (jsonObject == null)
            throw new ArgumentNullException(nameof(jsonObject), "JSON object for IdentifiedRisId cannot be null");
        
        string fullTripId = jsonObject["tripId"]?.ToString() ??  throw new ArgumentException("Missing 'tripId' field in IdentifiedRisId");
        string datePart = fullTripId.Substring(0, 8);

        if (DateTime.TryParseExact(datePart, "yyyyMMdd", null, DateTimeStyles.None, out DateTime _))
            fullTripId = fullTripId.Substring(9);

        DateTime? discoveredAt = null;
        if (jsonObject["plannedWhen"] != null)
            discoveredAt = new DateTime(
                DateOnly.FromDateTime(DateTime.Parse(jsonObject["plannedWhen"]!.ToString()).Date),
                TimeOnly.FromTimeSpan(DateTime.Now.Date.TimeOfDay)
            ).ToLocalTime();
        
        return new IdentifiedRisId()
        {
            RisId = fullTripId,
            DiscoveredAt = discoveredAt
        };
    }
}