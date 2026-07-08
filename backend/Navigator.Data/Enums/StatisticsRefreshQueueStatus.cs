using NpgsqlTypes;

namespace Navigator.Data.Enums;

public enum StatisticsRefreshQueueStatus
{
    [PgName("PENDING")]
    Pending,

    [PgName("RUNNING")]
    Running,

    [PgName("SUCCESS")]
    Success,

    [PgName("FAILED")]
    Failed
}
