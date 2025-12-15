using NpgsqlTypes;

namespace Navigator.Data.Enums;

public enum MessageReferenceType
{
    [PgName("LINK")]
    Link,

    [PgName("IMAGE")]
    Image,

    [PgName("ATTACHMENT")]
    Attachment
}
