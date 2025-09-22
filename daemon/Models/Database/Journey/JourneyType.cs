using NpgsqlTypes;

namespace daemon.Models.Database.Journey;

public enum JourneyType
{
    [PgName("REGULAR")]
    REGULAR,
    [PgName("REPLACEMENT")]
    REPLACEMENT,
    [PgName("RELIEF")]
    RELIEF,
    [PgName("EXTRA")]
    EXTRA
}