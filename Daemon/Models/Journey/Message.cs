using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

namespace Daemon.Models.Journey;

public class Message
{
    [BsonElement("type")] public string Type { get; set; }

    [BsonElement("summary")] public string Summary { get; set; }

    [BsonElement("text")] public string Text { get; set; }

    public static Message FromJson(JToken? jsonObject)
    {
        if (jsonObject == null)
            throw new ArgumentNullException(nameof(jsonObject), "JSON object for Message cannot be null");

        return new Message()
        {
            Type = jsonObject["type"]?.ToString() ?? throw new ArgumentException("Missing 'type' field in Message"),
            Summary = jsonObject["summary"]?.ToString() ??
                      throw new ArgumentException("Missing 'summary' field in Message"),
            Text = jsonObject["text"]?.ToString() ?? throw new ArgumentException("Missing 'text' field in Message")
        };
    }
}