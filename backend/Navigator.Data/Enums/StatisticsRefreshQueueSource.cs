using NpgsqlTypes;

namespace Navigator.Data.Enums;

public enum StatisticsRefreshQueueSource
{
    [PgName("JOURNEY_IMPORT")]
    JourneyImport,

    [PgName("MANUAL")]
    Manual
}
