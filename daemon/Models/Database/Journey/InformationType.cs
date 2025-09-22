using NpgsqlTypes;

namespace daemon.Models.Database.Journey;

public enum InformationType
{
    [PgName("DISRUPTION")]
    DISRUPTION,
    [PgName("JOURNEY_ATTRIBUTE")]
    JOURNEY_ATTRIBUTE,
    [PgName("RIS_QUALITY_DEVIATION")]
    RIS_QUALITY_DEVIATION,
    [PgName("RIS_CAUSE_REASON")]
    RIS_CAUSE_REASON,
    [PgName("MESSAGE")]
    MESSAGE
}