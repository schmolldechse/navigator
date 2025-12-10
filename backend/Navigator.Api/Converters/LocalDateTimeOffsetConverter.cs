using System.Text.Json;
using System.Text.Json.Serialization;

namespace Navigator.Api.Converters;

public class LocalDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTimeOffset.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        var localOffset = TimeZoneInfo.Local.GetUtcOffset(value.DateTime);
        var localTime = value.ToOffset(localOffset);
        writer.WriteStringValue(localTime.ToString("yyyy-MM-ddTHH:mm:sszzz"));
    }
}
