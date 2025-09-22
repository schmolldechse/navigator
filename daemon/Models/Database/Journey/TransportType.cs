using NpgsqlTypes;

namespace daemon.Models.Database;

public enum TransportType
{
    [PgName("UNKNOWN")]
    UNKNOWN,
    [PgName("HIGH_SPEED_TRAIN")]
    HIGH_SPEED_TRAIN,
    [PgName("INTERCITY_TRAIN")]
    INTERCITY_TRAIN,
    [PgName("INTER_REGIONAL_TRAIN")]
    INTER_REGIONAL_TRAIN,
    [PgName("REGIONAL_TRAIN")]
    REGIONAL_TRAIN,
    [PgName("CITY_TRAIN")]
    CITY_TRAIN,
    [PgName("SUBWAY")]
    SUBWAY,
    [PgName("TRAM")]
    TRAM,
    [PgName("BUS")]
    BUS,
    [PgName("FERRY")]
    FERRY,
    [PgName("FLIGHT")]
    FLIGHT,
    [PgName("CAR")]
    CAR,
    [PgName("TAXI")]
    TAXI,
    [PgName("SHUTTLE")]
    SHUTTLE,
    [PgName("BIKE")]
    BIKE,
    [PgName("SCOOTER")]
    SCOOTER,
    [PgName("WALK")]
    WALK
}