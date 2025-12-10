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
