using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

namespace Daemon.Models.Journey;

public class LineInformation
{
    [BsonElement("type")]
    [BsonRepresentation(BsonType.String)]
    public TransportProducts Type { get; set; }

    [BsonElement("product")] public string Product { get; set; }

    [BsonElement("lineName")] public string LineName { get; set; }

    [BsonElement("fahrtNr")] public string FahrtNr { get; set; }

    [BsonElement("operator")] public Operator Operator { get; set; }

    public static LineInformation FromJson(JToken? jsonObject)
    {
        if (jsonObject == null) throw new ArgumentNullException(nameof(jsonObject), "JSON object for LineInformation cannot be null");
        
        return new LineInformation()
        {
            Type = TransportProductMapper.GetTransportProduct(jsonObject["product"]?.ToString()),
            Product = jsonObject["productName"]?.ToString() ?? throw new ArgumentException("Missing 'productName' field in LineInformation"),
            LineName = jsonObject["name"]?.ToString() ?? throw new ArgumentException("Missing 'name' field in LineInformation"),
            FahrtNr = jsonObject["fahrtNr"]?.ToString() ?? throw new ArgumentException("Missing 'fahrtNr' field in LineInformation"),
            Operator = Operator.FromJson(jsonObject["operator"])
        };
    }
}