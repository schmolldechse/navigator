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