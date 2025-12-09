using Navigator.Data.Converters;
using System.Text.Json.Serialization;

namespace Navigator.Data.Enums;

[JsonConverter(typeof(TransportTypeConverter))]
public enum TransportType
{
    Unknown,
    HighSpeedTrain,
    IntercityTrain,
    InterRegionalTrain,
    RegionalTrain,
    CityTrain,
    Subway,
    Tram,
    Bus,
    Ferry,
    Flight,
    Car,
    Taxi,
    Shuttle,
    Bike,
    Scooter,
    Walk
}
