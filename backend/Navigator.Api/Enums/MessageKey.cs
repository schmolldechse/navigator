namespace Navigator.Api.Enums;

public enum MessageKey
{
    UNPLANNED_INFO,
    GENERAL_WARNING,
    ADDITIONAL_COACHES,
    MISSING_COACHES,
    CHANGED_COACH_SEQUENCE,
    REPLACEMENT_SERVICE,
    ADDITIONAL_STOP,
    WIFI_DISTRIBUTION,
    NO_FIRST_CLASS,
    ACCESSIBILITY_WARNING,
    RESERVATIONS_MISSING,
    RESERVATIONS_REQUIRED,
    NO_FOOD,
    BICYCLE_WARNING,
    BICYCLE_TRANSPORT,
    BICYCLE_TRANSPORT_NOT_POSSIBLE,
    BICYCLE_RESERVATION_REQUIRED,
    TICKET_INFORMATION
}

public static class MessageKeyConverter
{
    public static MessageKey MapToMessageKey(string? code) => code?.ToUpperInvariant() switch
    {
        "1" or "3" or "5" or "6" or "7" or "10" or "12" or "13" or "14" or "15" or "16" or "17" or "18" or "19" or "21" or "24" or "27" or "28" or "32" or "42" or "43" or "44" or "45" or "47" or "48" or "51" or "56" or "62" or "63" or "67" or "68" or "69" or "72" or "84" or "88" or "89" or "94" or "99" or "1000" or "CK" or "EF" or "EH" or "FT" or "HS" or "KA" or "OA" or "OC" or "RG" or "RO" or "RT" or "SI" or "SM" or "ZN" => MessageKey.UNPLANNED_INFO,
        "2" or "8" or "9" or "11" or "22" or "30" or "31" or "33" or "34" or "35" or "36" or "37" or "38" or "39" or "40" or "41" or "49" or "50" or "52" or "53" or "54" or "55" or "58" or "59" or "60" or "61" or "64" or "65" or "66" or "96" or "97" or "98" => MessageKey.GENERAL_WARNING,
        "25" => MessageKey.ADDITIONAL_COACHES,
        "26" or "79" or "82" or "85" => MessageKey.MISSING_COACHES,
        "73" or "74" or "75" or "76" or "80"  => MessageKey.CHANGED_COACH_SEQUENCE,
        "29" or "78" => MessageKey.REPLACEMENT_SERVICE,
        "57" => MessageKey.ADDITIONAL_STOP,
        "70" or "71" => MessageKey.WIFI_DISTRIBUTION,
        "77" => MessageKey.NO_FIRST_CLASS,
        "83" or "93" or "95" or "DC" or "OG" => MessageKey.ACCESSIBILITY_WARNING,
        "86" or "87" => MessageKey.RESERVATIONS_MISSING,
        "RP" => MessageKey.RESERVATIONS_REQUIRED,
        "90" => MessageKey.NO_FOOD,
        "AB" or "KF" or "RF" or "TF" => MessageKey.BICYCLE_TRANSPORT,
        "91" or "NF" => MessageKey.BICYCLE_TRANSPORT_NOT_POSSIBLE,
        "92" or "FB" or "FK" or "FS" or "G" => MessageKey.BICYCLE_WARNING,
        "FF" or "FO" or "FR" => MessageKey.BICYCLE_RESERVATION_REQUIRED,
        "N+" or "NG" or "NJ" => MessageKey.TICKET_INFORMATION,
        _ => MessageKey.UNPLANNED_INFO
    };
}