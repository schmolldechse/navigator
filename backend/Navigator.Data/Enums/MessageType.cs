using NpgsqlTypes;

namespace Navigator.Data.Enums;

public enum MessageType
{
    [PgName("ATTRIBUTE")]
    Attribute,

    [PgName("DISRUPTION")]
    Disruption,

    [PgName("NOTE")]
    Note,

    [PgName("RIS_CAUSE")]
    RisCause,

    [PgName("RIS_QUALITY_DEVIATION")]
    RisQualityDeviation
}
