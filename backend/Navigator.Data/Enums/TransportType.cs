using Navigator.Data.Models.Ris;
using NpgsqlTypes;

namespace Navigator.Data.Enums;

public enum TransportType
{
    [PgName("UNKNOWN")]
    Unknown,

    [PgName("HIGH_SPEED_TRAIN")]
    HighSpeedTrain,

    [PgName("INTERCITY_TRAIN")]
    IntercityTrain,

    [PgName("INTER_REGIONAL_TRAIN")]
    InterRegionalTrain,

    [PgName("REGIONAL_TRAIN")]
    RegionalTrain,

    [PgName("CITY_TRAIN")]
    CityTrain,

    [PgName("SUBWAY")]
    Subway,

    [PgName("TRAM")]
    Tram,

    [PgName("BUS")]
    Bus,

    [PgName("FERRY")]
    Ferry,

    [PgName("FLIGHT")]
    Flight,

    [PgName("CAR")]
    Car,

    [PgName("TAXI")]
    Taxi,

    [PgName("SHUTTLE")]
    Shuttle,

    [PgName("BIKE")]
    Bike,

    [PgName("SCOOTER")]
    Scooter,

    [PgName("WALK")]
    Walk
}

public static class TransportTypeConverter
{
    public static Navigator.Data.Enums.TransportType BoardsTransportToNavigatorTransport(RisBoards.TransportType transportType) => transportType switch
    {
        RisBoards.TransportType.HIGH_SPEED_TRAIN => Navigator.Data.Enums.TransportType.HighSpeedTrain,
        RisBoards.TransportType.INTERCITY_TRAIN => Navigator.Data.Enums.TransportType.IntercityTrain,
        RisBoards.TransportType.INTER_REGIONAL_TRAIN => Navigator.Data.Enums.TransportType.InterRegionalTrain,
        RisBoards.TransportType.REGIONAL_TRAIN => Navigator.Data.Enums.TransportType.RegionalTrain,
        RisBoards.TransportType.CITY_TRAIN => Navigator.Data.Enums.TransportType.CityTrain,
        RisBoards.TransportType.SUBWAY => Navigator.Data.Enums.TransportType.Subway,
        RisBoards.TransportType.TRAM => Navigator.Data.Enums.TransportType.Tram,
        RisBoards.TransportType.BUS => Navigator.Data.Enums.TransportType.Bus,
        RisBoards.TransportType.FERRY => Navigator.Data.Enums.TransportType.Ferry,
        RisBoards.TransportType.FLIGHT => Navigator.Data.Enums.TransportType.Flight,
        RisBoards.TransportType.CAR => Navigator.Data.Enums.TransportType.Car,
        RisBoards.TransportType.TAXI => Navigator.Data.Enums.TransportType.Taxi,
        RisBoards.TransportType.SHUTTLE => Navigator.Data.Enums.TransportType.Shuttle,
        RisBoards.TransportType.BIKE => Navigator.Data.Enums.TransportType.Bike,
        RisBoards.TransportType.SCOOTER => Navigator.Data.Enums.TransportType.Scooter,
        RisBoards.TransportType.WALK => Navigator.Data.Enums.TransportType.Walk,
        _ => Navigator.Data.Enums.TransportType.Unknown,
    };

    public static Navigator.Data.Enums.TransportType? StringTransportToNavigatorTransport(string? transportType)
    {
        if (string.IsNullOrEmpty(transportType)) return null;
        return transportType switch
        {
            "HIGH_SPEED_TRAIN" => Navigator.Data.Enums.TransportType.HighSpeedTrain,
            "HOCHGESCHWINDIGKEITSZUEGE" => Navigator.Data.Enums.TransportType.HighSpeedTrain,
            "INTERCITY_TRAIN" => Navigator.Data.Enums.TransportType.IntercityTrain,
            "INTERCITYUNDEUROCITYZUEGE" => Navigator.Data.Enums.TransportType.IntercityTrain,
            "INTER_REGIONAL_TRAIN" => Navigator.Data.Enums.TransportType.InterRegionalTrain,
            "INTERREGIOUNDSCHNELLZUEGE" => Navigator.Data.Enums.TransportType.InterRegionalTrain,
            "REGIONAL_TRAIN" => Navigator.Data.Enums.TransportType.RegionalTrain,
            "NAHVERKEHRSONSTIGEZUEGE" => Navigator.Data.Enums.TransportType.RegionalTrain,
            "CITY_TRAIN" => Navigator.Data.Enums.TransportType.CityTrain,
            "SBAHNEN" => Navigator.Data.Enums.TransportType.CityTrain,
            "SUBWAY" => Navigator.Data.Enums.TransportType.Subway,
            "UBAHN" => Navigator.Data.Enums.TransportType.Subway,
            "TRAM" => Navigator.Data.Enums.TransportType.Tram,
            "STRASSENBAHN" => Navigator.Data.Enums.TransportType.Tram,
            "BUS" => Navigator.Data.Enums.TransportType.Bus,
            "BUSSE" => Navigator.Data.Enums.TransportType.Bus,
            "FERRY" => Navigator.Data.Enums.TransportType.Ferry,
            "SCHIFFE" => Navigator.Data.Enums.TransportType.Ferry,
            "FLIGHT" => Navigator.Data.Enums.TransportType.Flight,
            "CAR" => Navigator.Data.Enums.TransportType.Car,
            "TAXI" => Navigator.Data.Enums.TransportType.Taxi,
            "SHUTTLE" => Navigator.Data.Enums.TransportType.Shuttle,
            "ANRUFPFLICHTIGEVERKEHRE" => Navigator.Data.Enums.TransportType.Shuttle,
            "BIKE" => Navigator.Data.Enums.TransportType.Bike,
            "SCOOTER" => Navigator.Data.Enums.TransportType.Scooter,
            "WALK" => Navigator.Data.Enums.TransportType.Walk,
            _ => Navigator.Data.Enums.TransportType.Unknown,
        };
    }
}