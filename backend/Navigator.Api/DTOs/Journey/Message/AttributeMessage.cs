using Navigator.Data.Enums;

namespace Navigator.Api.DTOs.Journey.Message;

public class AttributeMessage : JourneyMessage
{
    internal override MessageType Type => MessageType.Attribute;
}