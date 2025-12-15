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

public static class JourneyTypeConverter
{
    public static Navigator.Data.Enums.JourneyType MapBoardsJourneyTypeOrThrow(RisBoards.JourneyType journeyType) => journeyType switch
    {
        RisBoards.JourneyType.REGULAR => Navigator.Data.Enums.JourneyType.Regular,
        RisBoards.JourneyType.REPLACEMENT => Navigator.Data.Enums.JourneyType.Replacement,
        RisBoards.JourneyType.RELIEF => Navigator.Data.Enums.JourneyType.Relief,
        RisBoards.JourneyType.EXTRA => Navigator.Data.Enums.JourneyType.Extra,
        _ => Navigator.Data.Enums.JourneyType.Regular
    };
}