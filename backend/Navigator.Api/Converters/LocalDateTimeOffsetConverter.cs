using System.Text.Json;
using System.Text.Json.Serialization;

namespace Navigator.Api.Converters;

public class LocalDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetDateTimeOffset(out var dateTimeOffset)) return dateTimeOffset;
        throw new JsonException($"Unable to parse '{reader.GetString()}' as a valid DateTimeOffset.");
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
