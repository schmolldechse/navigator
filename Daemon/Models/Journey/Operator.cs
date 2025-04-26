using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

namespace Daemon.Models.Journey;

public class Operator
{
    [BsonElement("id")] public string? Id { get; set; }

    [BsonElement("name")] public string? Name { get; set; }

    public static Operator FromJson(JToken? jsonObject)
    {
        if (jsonObject == null)
            throw new ArgumentNullException(nameof(jsonObject), "JSON object for Operator cannot be null");

        return new Operator()
        {
            Id = jsonObject["id"]?.ToString(),
            Name = jsonObject["name"]?.ToString()
        };
    }
}