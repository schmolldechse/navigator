using NpgsqlTypes;

namespace daemon.Models.Database.Journey;

public enum ScheduleType
{
    [PgName("ARRIVAL")]
    ARRIVAL,
    [PgName("DEPARTURE")]
    DEPARTURE
}