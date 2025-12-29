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
    public static Navigator.Data.Enums.TimeType StringToNavigatorTime(string? timeType) => timeType?.ToUpperInvariant() switch
    {
        "SCHEDULE" => Navigator.Data.Enums.TimeType.Schedule,
        "PREVIEW" => Navigator.Data.Enums.TimeType.Preview,
        "REAL" => Navigator.Data.Enums.TimeType.Real,
        _ => Navigator.Data.Enums.TimeType.Schedule,
    };
}