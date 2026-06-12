using Navigator.Api.Enums;

namespace Navigator.Api.DTOs.Journey.Message;

public class RisQualityDeviationMessage : JourneyMessage
{
    internal override MessageType Type => MessageType.RisQualityDeviation;
}