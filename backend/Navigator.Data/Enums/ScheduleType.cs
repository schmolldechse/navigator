using NpgsqlTypes;

namespace Navigator.Data.Enums;

public enum ScheduleType
{
    [PgName("ARRIVAL")]
    Arrival,

    [PgName("DEPARTURE")]
    Departure
}
