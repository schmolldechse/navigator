using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Daemon.Models;

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

public class TransportProductMapper
{
    private static readonly Dictionary<TransportProducts, string[]> Possibilities = new()
    {
        { TransportProducts.Hochgeschwindigkeitszuege, new[] { "HIGH_SPEED_TRAIN", "nationalExpress" } },
        { TransportProducts.IntercityUndEurocityZuege, new[] { "INTERCITY_TRAIN", "national" } },
        { TransportProducts.InterregioUndSchnellzuege, new[] { "INTER_REGIONAL_TRAIN", "regionalExpress" } },
        { TransportProducts.NahverkehrsonstigeZuege, new[] { "REGIONAL_TRAIN", "regional" } },
        { TransportProducts.Sbahnen, new[] { "CITY_TRAIN", "suburban" } },
        { TransportProducts.Busse, new[] { "BUS" } },
        { TransportProducts.Schiffe, new[] { "FERRY" } },
        { TransportProducts.UBahn, new[] { "SUBWAY" } },
        { TransportProducts.Strassenbahn, new[] { "TRAM" } },
        { TransportProducts.AnrufpflichtigeVerkehre, new[] { "SHUTTLE", "taxi" } },
        { TransportProducts.Unknown, new string[] { } }
    };

    public static TransportProducts GetTransportProduct(string? transport)
    {
        if (string.IsNullOrEmpty(transport))
            return TransportProducts.Unknown;
        return Possibilities
            .Where(entry => entry.Value.Contains(transport))
            .Select(entry => entry.Key)
            .FirstOrDefault(TransportProducts.Unknown);
    }
}