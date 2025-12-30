using Navigator.Data.Models.Ris;
using NpgsqlTypes;

namespace Navigator.Data.Enums;

public enum TimeType
{
    [PgName("SCHEDULE")]
    Schedule,

    [PgName("PREVIEW")]
    Preview,

    [PgName("REAL")]
    Real
}

public static class TimeTypeConverter
{
    public static TimeType StringToNavigatorTime(string? timeType) => timeType?.ToUpperInvariant() switch
    {
        "SCHEDULE" => TimeType.Schedule,
        "PREVIEW" => TimeType.Preview,
        "REAL" => TimeType.Real,
        _ => TimeType.Schedule,
    };
    
    public static TimeType BoardsTimeTypeToNavigatorTime(RisBoards.TimeType timeType) => timeType switch
    {
        RisBoards.TimeType.SCHEDULE => TimeType.Schedule,
        RisBoards.TimeType.PREVIEW => TimeType.Preview,
        RisBoards.TimeType.REAL => TimeType.Real,
        _ => TimeType.Schedule,
    };
}