using Navigator.Data.Models.Ris;
using NpgsqlTypes;

namespace Navigator.Data.Enums;

public enum JourneyType
{
    [PgName("REGULAR")]
    Regular,

    [PgName("REPLACEMENT")]
    Replacement,

    [PgName("RELIEF")]
    Relief,

    [PgName("EXTRA")]
    Extra
}
