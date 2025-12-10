using NpgsqlTypes;

namespace Navigator.Data.Enums;

public enum InformationType
{
    [PgName("JOURNEY_ATTRIBUTE")]
    JourneyAttribute,

    [PgName("DISRUPTION")]
    Disruption,

    [PgName("MESSAGE")]
    Message,

    [PgName("RIS_QUALITY_DEVIATION")]
    RisQualityDeviation,

    [PgName("RIS_CAUSE_REASON")]
    RisCauseReason
}
