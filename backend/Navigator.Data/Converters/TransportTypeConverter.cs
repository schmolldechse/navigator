using Navigator.Data.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Navigator.Data.Converters;

public class TransportTypeConverter : JsonConverter<TransportType>
{
    public override TransportType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        return str switch
        {
            "HIGH_SPEED_TRAIN" => TransportType.HighSpeedTrain,
            "INTERCITY_TRAIN" => TransportType.IntercityTrain,
            "INTER_REGIONAL_TRAIN" => TransportType.InterRegionalTrain,
            "REGIONAL_TRAIN" => TransportType.RegionalTrain,
            "CITY_TRAIN" => TransportType.CityTrain,
            "SUBWAY" => TransportType.Subway,
            "TRAM" => TransportType.Tram,
            "BUS" => TransportType.Bus,
            "FERRY" => TransportType.Ferry,
            "FLIGHT" => TransportType.Flight,
            "CAR" => TransportType.Car,
            "TAXI" => TransportType.Taxi,
            "SHUTTLE" => TransportType.Shuttle,
            "BIKE" => TransportType.Bike,
            "SCOOTER" => TransportType.Scooter,
            "WALK" => TransportType.Walk,
            _ => TransportType.Unknown,
        };
    }

    public override void Write(Utf8JsonWriter writer, TransportType value, JsonSerializerOptions options)
    {
        var str = value switch
        {
            TransportType.HighSpeedTrain => "HIGH_SPEED_TRAIN",
            TransportType.IntercityTrain => "INTERCITY_TRAIN",
            TransportType.InterRegionalTrain => "INTER_REGIONAL_TRAIN",
            TransportType.RegionalTrain => "REGIONAL_TRAIN",
            TransportType.CityTrain => "CITY_TRAIN",
            TransportType.Subway => "SUBWAY",
            TransportType.Tram => "TRAM",
            TransportType.Bus => "BUS",
            TransportType.Ferry => "FERRY",
            TransportType.Flight => "FLIGHT",
            TransportType.Car => "CAR",
            TransportType.Taxi => "TAXI",
            TransportType.Shuttle => "SHUTTLE",
            TransportType.Bike => "BIKE",
            TransportType.Scooter => "SCOOTER",
            TransportType.Walk => "WALK",
            _ => "UNKNOWN",
        };
        writer.WriteStringValue(str);
    }
}
