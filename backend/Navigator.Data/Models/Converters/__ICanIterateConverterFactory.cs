using Navigator.Data.Models.Ris;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Navigator.Data.Models.Converters;

public class __ICanIterateConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => 
        !typeToConvert.IsEnum &&
        typeToConvert.GetInterfaces().Any(i => i == typeof(__ICanIterate));

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(CanIterateConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

public class CanIterateConverter<T> : JsonConverter<T> where T : class, new()
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

        var instance = new T();
        var canIterate = instance as __ICanIterate;

        var propertyMap = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
        foreach (var (name, _) in canIterate!.IterateProperties()) 
        {
            var propertyInfo = typeToConvert.GetProperty(
                ToPascalCase(name),
                BindingFlags.Public | BindingFlags.Instance
            );
            if (propertyInfo != null) propertyMap[name] = propertyInfo;
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return instance;
            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();

            string propertyName = reader.GetString()!;
            reader.Read();

            if (propertyMap.TryGetValue(propertyName, out var propertyInfo))
            {
                var value = JsonSerializer.Deserialize(ref reader, propertyInfo.PropertyType, options);
                propertyInfo.SetValue(instance, value);
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        var canIterate = value as __ICanIterate;
        foreach (var (name, propertyValue) in canIterate!.IterateProperties())
        {
            writer.WritePropertyName(name);
            JsonSerializer.Serialize(writer, propertyValue, options);
        }

        writer.WriteEndObject();
    }

    private string ToPascalCase(string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return char.ToUpper(str[0]) + str.Substring(1);
    }
}
