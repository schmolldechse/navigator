using Navigator.Api.Enums;

namespace Navigator.Api.DTOs.Journey.Message;

public class RisCauseMessage : JourneyMessage
{
    internal override MessageType Type => MessageType.RisCause;
}