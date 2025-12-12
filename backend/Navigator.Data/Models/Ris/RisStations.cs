namespace Navigator.Data.Models.Ris;

public class RisStations
{
    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Accessibility : __ICanIterate
    {
        public Accessibility() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Accessibility(AccessibilityStatus audibleSignalsAvailable, AccessibilityStatus automaticDoor, AccessibilityStatus boardingAid, AccessibilityStatus passengerInformationDisplay, AccessibilityStatus platformSign, AccessibilityStatus stairsMarking, AccessibilityStatus standardPlatformHeight, AccessibilityStatus stepFreeAccess, AccessibilityStatus tactileGuidingStrips, AccessibilityStatus tactileHandrailLabel, AccessibilityStatus tactilePlatformAccess)
        {
            AudibleSignalsAvailable = audibleSignalsAvailable;
            AutomaticDoor = automaticDoor;
            BoardingAid = boardingAid;
            PassengerInformationDisplay = passengerInformationDisplay;
            PlatformSign = platformSign;
            StairsMarking = stairsMarking;
            StandardPlatformHeight = standardPlatformHeight;
            StepFreeAccess = stepFreeAccess;
            TactileGuidingStrips = tactileGuidingStrips;
            TactileHandrailLabel = tactileHandrailLabel;
            TactilePlatformAccess = tactilePlatformAccess;
        }

        public required AccessibilityStatus AudibleSignalsAvailable { get; set; }
        public required AccessibilityStatus AutomaticDoor { get; set; }
        public required AccessibilityStatus BoardingAid { get; set; }
        public required AccessibilityStatus PassengerInformationDisplay { get; set; }
        public required AccessibilityStatus PlatformSign { get; set; }
        public required AccessibilityStatus StairsMarking { get; set; }
        public required AccessibilityStatus StandardPlatformHeight { get; set; }
        public required AccessibilityStatus StepFreeAccess { get; set; }
        public required AccessibilityStatus TactileGuidingStrips { get; set; }
        public required AccessibilityStatus TactileHandrailLabel { get; set; }
        public required AccessibilityStatus TactilePlatformAccess { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("audibleSignalsAvailable", AudibleSignalsAvailable switch
            {
                AccessibilityStatus.AVAILABLE => "AVAILABLE",
                AccessibilityStatus.NOT_AVAILABLE => "NOT_AVAILABLE",
                AccessibilityStatus.PARTIAL => "PARTIAL",
                AccessibilityStatus.NOT_APPLICABLE => "NOT_APPLICABLE",
                AccessibilityStatus.UNKNOWN => "UNKNOWN",
                _ => null
            });
            yield return ("automaticDoor", AutomaticDoor switch
            {
                AccessibilityStatus.AVAILABLE => "AVAILABLE",
                AccessibilityStatus.NOT_AVAILABLE => "NOT_AVAILABLE",
                AccessibilityStatus.PARTIAL => "PARTIAL",
                AccessibilityStatus.NOT_APPLICABLE => "NOT_APPLICABLE",
                AccessibilityStatus.UNKNOWN => "UNKNOWN",
                _ => null
            });
            yield return ("boardingAid", BoardingAid switch
            {
                AccessibilityStatus.AVAILABLE => "AVAILABLE",
                AccessibilityStatus.NOT_AVAILABLE => "NOT_AVAILABLE",
                AccessibilityStatus.PARTIAL => "PARTIAL",
                AccessibilityStatus.NOT_APPLICABLE => "NOT_APPLICABLE",
                AccessibilityStatus.UNKNOWN => "UNKNOWN",
                _ => null
            });
            yield return ("passengerInformationDisplay", PassengerInformationDisplay switch
            {
                AccessibilityStatus.AVAILABLE => "AVAILABLE",
                AccessibilityStatus.NOT_AVAILABLE => "NOT_AVAILABLE",
                AccessibilityStatus.PARTIAL => "PARTIAL",
                AccessibilityStatus.NOT_APPLICABLE => "NOT_APPLICABLE",
                AccessibilityStatus.UNKNOWN => "UNKNOWN",
                _ => null
            });
            yield return ("platformSign", PlatformSign switch
            {
                AccessibilityStatus.AVAILABLE => "AVAILABLE",
                AccessibilityStatus.NOT_AVAILABLE => "NOT_AVAILABLE",
                AccessibilityStatus.PARTIAL => "PARTIAL",
                AccessibilityStatus.NOT_APPLICABLE => "NOT_APPLICABLE",
                AccessibilityStatus.UNKNOWN => "UNKNOWN",
                _ => null
            });
            yield return ("stairsMarking", StairsMarking switch
            {
                AccessibilityStatus.AVAILABLE => "AVAILABLE",
                AccessibilityStatus.NOT_AVAILABLE => "NOT_AVAILABLE",
                AccessibilityStatus.PARTIAL => "PARTIAL",
                AccessibilityStatus.NOT_APPLICABLE => "NOT_APPLICABLE",
                AccessibilityStatus.UNKNOWN => "UNKNOWN",
                _ => null
            });
            yield return ("standardPlatformHeight", StandardPlatformHeight switch
            {
                AccessibilityStatus.AVAILABLE => "AVAILABLE",
                AccessibilityStatus.NOT_AVAILABLE => "NOT_AVAILABLE",
                AccessibilityStatus.PARTIAL => "PARTIAL",
                AccessibilityStatus.NOT_APPLICABLE => "NOT_APPLICABLE",
                AccessibilityStatus.UNKNOWN => "UNKNOWN",
                _ => null
            });
            yield return ("stepFreeAccess", StepFreeAccess switch
            {
                AccessibilityStatus.AVAILABLE => "AVAILABLE",
                AccessibilityStatus.NOT_AVAILABLE => "NOT_AVAILABLE",
                AccessibilityStatus.PARTIAL => "PARTIAL",
                AccessibilityStatus.NOT_APPLICABLE => "NOT_APPLICABLE",
                AccessibilityStatus.UNKNOWN => "UNKNOWN",
                _ => null
            });
            yield return ("tactileGuidingStrips", TactileGuidingStrips switch
            {
                AccessibilityStatus.AVAILABLE => "AVAILABLE",
                AccessibilityStatus.NOT_AVAILABLE => "NOT_AVAILABLE",
                AccessibilityStatus.PARTIAL => "PARTIAL",
                AccessibilityStatus.NOT_APPLICABLE => "NOT_APPLICABLE",
                AccessibilityStatus.UNKNOWN => "UNKNOWN",
                _ => null
            });
            yield return ("tactileHandrailLabel", TactileHandrailLabel switch
            {
                AccessibilityStatus.AVAILABLE => "AVAILABLE",
                AccessibilityStatus.NOT_AVAILABLE => "NOT_AVAILABLE",
                AccessibilityStatus.PARTIAL => "PARTIAL",
                AccessibilityStatus.NOT_APPLICABLE => "NOT_APPLICABLE",
                AccessibilityStatus.UNKNOWN => "UNKNOWN",
                _ => null
            });
            yield return ("tactilePlatformAccess", TactilePlatformAccess switch
            {
                AccessibilityStatus.AVAILABLE => "AVAILABLE",
                AccessibilityStatus.NOT_AVAILABLE => "NOT_AVAILABLE",
                AccessibilityStatus.PARTIAL => "PARTIAL",
                AccessibilityStatus.NOT_APPLICABLE => "NOT_APPLICABLE",
                AccessibilityStatus.UNKNOWN => "UNKNOWN",
                _ => null
            });
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(AccessibilityStatusEnumConverter))]
    public enum AccessibilityStatus
    {
        AVAILABLE,
        NOT_AVAILABLE,
        PARTIAL,
        NOT_APPLICABLE,
        UNKNOWN,
    }

    public static class AccessibilityStatusFastEnum
    {
        public static string ToString(AccessibilityStatus value) => value switch
        {
            AccessibilityStatus.AVAILABLE => "AVAILABLE",
            AccessibilityStatus.NOT_AVAILABLE => "NOT_AVAILABLE",
            AccessibilityStatus.PARTIAL => "PARTIAL",
            AccessibilityStatus.NOT_APPLICABLE => "NOT_APPLICABLE",
            AccessibilityStatus.UNKNOWN => "UNKNOWN",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static AccessibilityStatus FromString(string? value) => value switch
        {
            "AVAILABLE" => AccessibilityStatus.AVAILABLE,
            "NOT_AVAILABLE" => AccessibilityStatus.NOT_AVAILABLE,
            "PARTIAL" => AccessibilityStatus.PARTIAL,
            "NOT_APPLICABLE" => AccessibilityStatus.NOT_APPLICABLE,
            "UNKNOWN" => AccessibilityStatus.UNKNOWN,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class AccessibilityStatusEnumConverter : System.Text.Json.Serialization.JsonConverter<AccessibilityStatus>
    {
        public override AccessibilityStatus Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (AccessibilityStatus)reader.GetInt32();
            }

            return AccessibilityStatusFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, AccessibilityStatus value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(AccessibilityStatusFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class AddressWithWeb : __ICanIterate
    {
        public AddressWithWeb() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public AddressWithWeb(string additionalInformation, string city, string country, string houseNumber, string postalCode, string state, string street, string website)
        {
            AdditionalInformation = additionalInformation;
            City = city;
            Country = country;
            HouseNumber = houseNumber;
            PostalCode = postalCode;
            State = state;
            Street = street;
            Website = website;
        }

        public required string AdditionalInformation { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public required string HouseNumber { get; set; }
        public required string PostalCode { get; set; }
        public required string State { get; set; }
        public required string Street { get; set; }
        public required string Website { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("additionalInformation", AdditionalInformation);
            yield return ("city", City);
            yield return ("country", Country);
            yield return ("houseNumber", HouseNumber);
            yield return ("postalCode", PostalCode);
            yield return ("state", State);
            yield return ("street", Street);
            yield return ("website", Website);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class ConnectingTime : __ICanIterate
    {
        public ConnectingTime() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public ConnectingTime(string fromEvaNumber, string fromPlatform, string fromSector, bool identicalPhysicalPlatform, ConnectingTimeSource source, System.Collections.Generic.List<ConnectionTime> times, string toEvaNumber, string toPlatform, string toSector)
        {
            FromEvaNumber = fromEvaNumber;
            FromPlatform = fromPlatform;
            FromSector = fromSector;
            IdenticalPhysicalPlatform = identicalPhysicalPlatform;
            Source = source;
            Times = times;
            ToEvaNumber = toEvaNumber;
            ToPlatform = toPlatform;
            ToSector = toSector;
        }

        public required string FromEvaNumber { get; set; }
        public required string FromPlatform { get; set; }
        public required string FromSector { get; set; }
        public required bool IdenticalPhysicalPlatform { get; set; }
        public required ConnectingTimeSource Source { get; set; }
        public required System.Collections.Generic.List<ConnectionTime> Times { get; set; }
        public required string ToEvaNumber { get; set; }
        public required string ToPlatform { get; set; }
        public required string ToSector { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("fromEvaNumber", FromEvaNumber);
            yield return ("fromPlatform", FromPlatform);
            yield return ("fromSector", FromSector);
            yield return ("identicalPhysicalPlatform", IdenticalPhysicalPlatform);
            yield return ("source", Source switch
            {
                ConnectingTimeSource.RIL420 => "RIL420",
                ConnectingTimeSource.EFZ => "EFZ",
                ConnectingTimeSource.INDOOR_ROUTING => "INDOOR_ROUTING",
                _ => null
            });
            yield return ("times", Times);
            yield return ("toEvaNumber", ToEvaNumber);
            yield return ("toPlatform", ToPlatform);
            yield return ("toSector", ToSector);
        }
    }


    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(ConnectingTimeGroupEnumConverter))]
    public enum ConnectingTimeGroup
    {
        STATION,
        SALES,
        ALL,
    }

    public static class ConnectingTimeGroupFastEnum
    {
        public static string ToString(ConnectingTimeGroup value) => value switch
        {
            ConnectingTimeGroup.STATION => "STATION",
            ConnectingTimeGroup.SALES => "SALES",
            ConnectingTimeGroup.ALL => "ALL",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static ConnectingTimeGroup FromString(string? value) => value switch
        {
            "STATION" => ConnectingTimeGroup.STATION,
            "SALES" => ConnectingTimeGroup.SALES,
            "ALL" => ConnectingTimeGroup.ALL,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class ConnectingTimeGroupEnumConverter : System.Text.Json.Serialization.JsonConverter<ConnectingTimeGroup>
    {
        public override ConnectingTimeGroup Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (ConnectingTimeGroup)reader.GetInt32();
            }

            return ConnectingTimeGroupFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, ConnectingTimeGroup value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(ConnectingTimeGroupFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class ConnectingTimesBatch : __ICanIterate
    {
        public ConnectingTimesBatch() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public ConnectingTimesBatch(System.Collections.Generic.List<ConnectingTime> connectingTimesList)
        {
            ConnectingTimesList = connectingTimesList;
        }

        public required System.Collections.Generic.List<ConnectingTime> ConnectingTimesList { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("connectingTimesList", ConnectingTimesList);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(ConnectingTimeSourceEnumConverter))]
    public enum ConnectingTimeSource
    {
        RIL420,
        EFZ,
        INDOOR_ROUTING,
    }

    public static class ConnectingTimeSourceFastEnum
    {
        public static string ToString(ConnectingTimeSource value) => value switch
        {
            ConnectingTimeSource.RIL420 => "RIL420",
            ConnectingTimeSource.EFZ => "EFZ",
            ConnectingTimeSource.INDOOR_ROUTING => "INDOOR_ROUTING",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static ConnectingTimeSource FromString(string? value) => value switch
        {
            "RIL420" => ConnectingTimeSource.RIL420,
            "EFZ" => ConnectingTimeSource.EFZ,
            "INDOOR_ROUTING" => ConnectingTimeSource.INDOOR_ROUTING,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class ConnectingTimeSourceEnumConverter : System.Text.Json.Serialization.JsonConverter<ConnectingTimeSource>
    {
        public override ConnectingTimeSource Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (ConnectingTimeSource)reader.GetInt32();
            }

            return ConnectingTimeSourceFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, ConnectingTimeSource value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(ConnectingTimeSourceFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class ConnectingTimesSingle : __ICanIterate
    {
        public ConnectingTimesSingle() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public ConnectingTimesSingle(ConnectingTime connectingTime, System.Collections.Generic.List<ConnectionTimeFallback> fallbackTimes)
        {
            ConnectingTime = connectingTime;
            FallbackTimes = fallbackTimes;
        }

        public required ConnectingTime ConnectingTime { get; set; }
        public required System.Collections.Generic.List<ConnectionTimeFallback> FallbackTimes { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("connectingTime", ConnectingTime);
            yield return ("fallbackTimes", FallbackTimes);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class ConnectionTime : __ICanIterate
    {
        public ConnectionTime() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public ConnectionTime(double distance, string duration, PersonaType persona)
        {
            Distance = distance;
            Duration = duration;
            Persona = persona;
        }

        public required double Distance { get; set; }
        public required string Duration { get; set; }
        public required PersonaType Persona { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("distance", Distance);
            yield return ("duration", Duration);
            yield return ("persona", Persona switch
            {
                PersonaType.HANDICAPPED => "HANDICAPPED",
                PersonaType.OCCASIONAL_TRAVELLER => "OCCASIONAL_TRAVELLER",
                PersonaType.FREQUENT_TRAVELLER => "FREQUENT_TRAVELLER",
                _ => null
            });
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class ConnectionTimeFallback : __ICanIterate
    {
        public ConnectionTimeFallback() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public ConnectionTimeFallback(string duration, PersonaType persona, ConnectingTimeSource source)
        {
            Duration = duration;
            Persona = persona;
            Source = source;
        }

        public required string Duration { get; set; }
        public required PersonaType Persona { get; set; }
        public required ConnectingTimeSource Source { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("duration", Duration);
            yield return ("persona", Persona switch
            {
                PersonaType.HANDICAPPED => "HANDICAPPED",
                PersonaType.OCCASIONAL_TRAVELLER => "OCCASIONAL_TRAVELLER",
                PersonaType.FREQUENT_TRAVELLER => "FREQUENT_TRAVELLER",
                _ => null
            });
            yield return ("source", Source switch
            {
                ConnectingTimeSource.RIL420 => "RIL420",
                ConnectingTimeSource.EFZ => "EFZ",
                ConnectingTimeSource.INDOOR_ROUTING => "INDOOR_ROUTING",
                _ => null
            });
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Contact : __ICanIterate
    {
        public Contact() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Contact(string contactPerson, string email, string furtherDetails, System.Collections.Generic.List<PhoneNumber> phoneNumbers, string url)
        {
            ContactPerson = contactPerson;
            Email = email;
            FurtherDetails = furtherDetails;
            PhoneNumbers = phoneNumbers;
            Url = url;
        }

        public required string ContactPerson { get; set; }
        public required string Email { get; set; }
        public required string FurtherDetails { get; set; }
        public required System.Collections.Generic.List<PhoneNumber> PhoneNumbers { get; set; }
        public required string Url { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("contactPerson", ContactPerson);
            yield return ("email", Email);
            yield return ("furtherDetails", FurtherDetails);
            yield return ("phoneNumbers", PhoneNumbers);
            yield return ("url", Url);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Coordinate2D : __ICanIterate
    {
        public Coordinate2D() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Coordinate2D(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public required double Latitude { get; set; }
        public required double Longitude { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("latitude", Latitude);
            yield return ("longitude", Longitude);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class EquipmentLocker : __ICanIterate
    {
        public EquipmentLocker() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public EquipmentLocker(string equipmentID, System.Collections.Generic.List<Locker> lockers, string stationID)
        {
            EquipmentID = equipmentID;
            Lockers = lockers;
            StationID = stationID;
        }

        public required string EquipmentID { get; set; }
        public required System.Collections.Generic.List<Locker> Lockers { get; set; }
        public required string StationID { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("equipmentID", EquipmentID);
            yield return ("lockers", Lockers);
            yield return ("stationID", StationID);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(EquipmentLockerKeyTypeEnumConverter))]
    public enum EquipmentLockerKeyType
    {
        EVA,
        STATION_ID,
    }

    public static class EquipmentLockerKeyTypeFastEnum
    {
        public static string ToString(EquipmentLockerKeyType value) => value switch
        {
            EquipmentLockerKeyType.EVA => "EVA",
            EquipmentLockerKeyType.STATION_ID => "STATION_ID",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static EquipmentLockerKeyType FromString(string? value) => value switch
        {
            "EVA" => EquipmentLockerKeyType.EVA,
            "STATION_ID" => EquipmentLockerKeyType.STATION_ID,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class EquipmentLockerKeyTypeEnumConverter : System.Text.Json.Serialization.JsonConverter<EquipmentLockerKeyType>
    {
        public override EquipmentLockerKeyType Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (EquipmentLockerKeyType)reader.GetInt32();
            }

            return EquipmentLockerKeyTypeFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, EquipmentLockerKeyType value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(EquipmentLockerKeyTypeFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class EquipmentLockers : __ICanIterate
    {
        public EquipmentLockers() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public EquipmentLockers(System.Collections.Generic.List<EquipmentLocker> lockerList)
        {
            LockerList = lockerList;
        }

        public required System.Collections.Generic.List<EquipmentLocker> LockerList { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("lockerList", LockerList);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class ErrorDetail : __ICanIterate
    {
        public ErrorDetail() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public ErrorDetail(string detail, string errorCode, string field, string title)
        {
            Detail = detail;
            ErrorCode = errorCode;
            Field = field;
            Title = title;
        }

        public required string Detail { get; set; }
        public required string ErrorCode { get; set; }
        public required string Field { get; set; }
        public required string Title { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("detail", Detail);
            yield return ("errorCode", ErrorCode);
            yield return ("field", Field);
            yield return ("title", Title);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class ErrorResponse : __ICanIterate
    {
        public ErrorResponse() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public ErrorResponse(string detail, string errorCode, System.Collections.Generic.List<ErrorDetail> errors, string instanceId, string status, string title)
        {
            Detail = detail;
            ErrorCode = errorCode;
            Errors = errors;
            InstanceId = instanceId;
            Status = status;
            Title = title;
        }

        public required string Detail { get; set; }
        public required string ErrorCode { get; set; }
        public required System.Collections.Generic.List<ErrorDetail> Errors { get; set; }
        public required string InstanceId { get; set; }
        public required string Status { get; set; }
        public required string Title { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("detail", Detail);
            yield return ("errorCode", ErrorCode);
            yield return ("errors", Errors);
            yield return ("instanceId", InstanceId);
            yield return ("status", Status);
            yield return ("title", Title);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Escalator : __ICanIterate
    {
        public Escalator() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Escalator(System.Collections.Generic.List<EscalatorAndLiftPlatforms> associatedPlatforms, string description, string operatorName, Coordinate2D position, State state)
        {
            AssociatedPlatforms = associatedPlatforms;
            Description = description;
            OperatorName = operatorName;
            Position = position;
            State = state;
        }

        public required System.Collections.Generic.List<EscalatorAndLiftPlatforms> AssociatedPlatforms { get; set; }
        public required string Description { get; set; }
        public required string OperatorName { get; set; }
        public required Coordinate2D Position { get; set; }
        public required State State { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("associatedPlatforms", AssociatedPlatforms);
            yield return ("description", Description);
            yield return ("operatorName", OperatorName);
            yield return ("position", Position);
            yield return ("state", State);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class EscalatorAndLiftPlatforms : __ICanIterate
    {
        public EscalatorAndLiftPlatforms() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public EscalatorAndLiftPlatforms(string evaNumber, System.Collections.Generic.List<string> platforms)
        {
            EvaNumber = evaNumber;
            Platforms = platforms;
        }

        public required string EvaNumber { get; set; }
        public required System.Collections.Generic.List<string> Platforms { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("evaNumber", EvaNumber);
            yield return ("platforms", Platforms);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Lift : __ICanIterate
    {
        public Lift() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Lift(System.Collections.Generic.List<EscalatorAndLiftPlatforms> associatedPlatforms, string description, string operatorName, Coordinate2D position, State state)
        {
            AssociatedPlatforms = associatedPlatforms;
            Description = description;
            OperatorName = operatorName;
            Position = position;
            State = state;
        }

        public required System.Collections.Generic.List<EscalatorAndLiftPlatforms> AssociatedPlatforms { get; set; }
        public required string Description { get; set; }
        public required string OperatorName { get; set; }
        public required Coordinate2D Position { get; set; }
        public required State State { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("associatedPlatforms", AssociatedPlatforms);
            yield return ("description", Description);
            yield return ("operatorName", OperatorName);
            yield return ("position", Position);
            yield return ("state", State);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(LocaleEnumConverter))]
    public enum Locale
    {
        CS,
        DA,
        DE,
        EN,
        ES,
        FR,
        IT,
        NL,
        PL,
    }

    public static class LocaleFastEnum
    {
        public static string ToString(Locale value) => value switch
        {
            Locale.CS => "CS",
            Locale.DA => "DA",
            Locale.DE => "DE",
            Locale.EN => "EN",
            Locale.ES => "ES",
            Locale.FR => "FR",
            Locale.IT => "IT",
            Locale.NL => "NL",
            Locale.PL => "PL",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static Locale FromString(string? value) => value switch
        {
            "CS" => Locale.CS,
            "DA" => Locale.DA,
            "DE" => Locale.DE,
            "EN" => Locale.EN,
            "ES" => Locale.ES,
            "FR" => Locale.FR,
            "IT" => Locale.IT,
            "NL" => Locale.NL,
            "PL" => Locale.PL,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class LocaleEnumConverter : System.Text.Json.Serialization.JsonConverter<Locale>
    {
        public override Locale Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (Locale)reader.GetInt32();
            }

            return LocaleFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, Locale value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(LocaleFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class LocalService : __ICanIterate
    {
        public LocalService() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public LocalService(AddressWithWeb address, Contact contact, string description, string externalID, string localServiceID, string name, string openingHours, System.Collections.Generic.List<string> paymentMethods, Coordinate2D position, string stationID, string type, System.DateTime validFrom, System.DateTime validTo)
        {
            Address = address;
            Contact = contact;
            Description = description;
            ExternalID = externalID;
            LocalServiceID = localServiceID;
            Name = name;
            OpeningHours = openingHours;
            PaymentMethods = paymentMethods;
            Position = position;
            StationID = stationID;
            Type = type;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }

        public required AddressWithWeb Address { get; set; }
        public required Contact Contact { get; set; }
        public required string Description { get; set; }
        public required string ExternalID { get; set; }
        public required string LocalServiceID { get; set; }
        public required string Name { get; set; }
        public required string OpeningHours { get; set; }
        public required System.Collections.Generic.List<string> PaymentMethods { get; set; }
        public required Coordinate2D Position { get; set; }
        public required string StationID { get; set; }
        public required string Type { get; set; }
        public required System.DateTime ValidFrom { get; set; }
        public required System.DateTime ValidTo { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("address", Address);
            yield return ("contact", Contact);
            yield return ("description", Description);
            yield return ("externalID", ExternalID);
            yield return ("localServiceID", LocalServiceID);
            yield return ("name", Name);
            yield return ("openingHours", OpeningHours);
            yield return ("paymentMethods", PaymentMethods);
            yield return ("position", Position);
            yield return ("stationID", StationID);
            yield return ("type", Type);
            yield return ("validFrom", ValidFrom);
            yield return ("validTo", ValidTo);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(LocalServiceKeyTypeEnumConverter))]
    public enum LocalServiceKeyType
    {
        EVA,
        STATION_ID,
    }

    public static class LocalServiceKeyTypeFastEnum
    {
        public static string ToString(LocalServiceKeyType value) => value switch
        {
            LocalServiceKeyType.EVA => "EVA",
            LocalServiceKeyType.STATION_ID => "STATION_ID",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static LocalServiceKeyType FromString(string? value) => value switch
        {
            "EVA" => LocalServiceKeyType.EVA,
            "STATION_ID" => LocalServiceKeyType.STATION_ID,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class LocalServiceKeyTypeEnumConverter : System.Text.Json.Serialization.JsonConverter<LocalServiceKeyType>
    {
        public override LocalServiceKeyType Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (LocalServiceKeyType)reader.GetInt32();
            }

            return LocalServiceKeyTypeFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, LocalServiceKeyType value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(LocalServiceKeyTypeFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class LocalServices : __ICanIterate
    {
        public LocalServices() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public LocalServices(System.Collections.Generic.List<LocalService> localServices)
        {
            LocalServicesList = localServices;
        }

        public required System.Collections.Generic.List<LocalService> LocalServicesList { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("localServices", LocalServicesList);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class LocalServicesCursoring : __ICanIterate
    {
        public LocalServicesCursoring() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public LocalServicesCursoring(string after, string before, System.Collections.Generic.List<LocalService> localServices, int total)
        {
            After = after;
            Before = before;
            LocalServices = localServices;
            Total = total;
        }

        public required string After { get; set; }
        public required string Before { get; set; }
        public required System.Collections.Generic.List<LocalService> LocalServices { get; set; }
        public required int Total { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("after", After);
            yield return ("before", Before);
            yield return ("localServices", LocalServices);
            yield return ("total", Total);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Locker : __ICanIterate
    {
        public Locker() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Locker(int amount, LockerDimension dimension, LockerFee fee, string maxLeaseDuration, System.Collections.Generic.List<string> paymentTypes, string size)
        {
            Amount = amount;
            Dimension = dimension;
            Fee = fee;
            MaxLeaseDuration = maxLeaseDuration;
            PaymentTypes = paymentTypes;
            Size = size;
        }

        public required int Amount { get; set; }
        public required LockerDimension Dimension { get; set; }
        public required LockerFee Fee { get; set; }
        public required string MaxLeaseDuration { get; set; }
        public required System.Collections.Generic.List<string> PaymentTypes { get; set; }
        public required string Size { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("amount", Amount);
            yield return ("dimension", Dimension);
            yield return ("fee", Fee);
            yield return ("maxLeaseDuration", MaxLeaseDuration);
            yield return ("paymentTypes", PaymentTypes);
            yield return ("size", Size);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class LockerDimension : __ICanIterate
    {
        public LockerDimension() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public LockerDimension(int depth, int height, int width)
        {
            Depth = depth;
            Height = height;
            Width = width;
        }

        public required int Depth { get; set; }
        public required int Height { get; set; }
        public required int Width { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("depth", Depth);
            yield return ("height", Height);
            yield return ("width", Width);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class LockerFee : __ICanIterate
    {
        public LockerFee() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public LockerFee(int fee, string feePeriod)
        {
            Fee = fee;
            FeePeriod = feePeriod;
        }

        public required int Fee { get; set; }
        public required string FeePeriod { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("fee", Fee);
            yield return ("feePeriod", FeePeriod);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class LockerRack : __ICanIterate
    {
        public LockerRack() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public LockerRack(System.Collections.Generic.List<Locker> lockers)
        {
            Lockers = lockers;
        }

        public required System.Collections.Generic.List<Locker> Lockers { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("lockers", Lockers);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Operational : __ICanIterate
    {
        public Operational() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Operational(System.Collections.Generic.List<string> networkPlatforms, System.Collections.Generic.List<string> optics, double orientation, System.Collections.Generic.List<ReferencePoint> referencePoints)
        {
            NetworkPlatforms = networkPlatforms;
            Optics = optics;
            Orientation = orientation;
            ReferencePoints = referencePoints;
        }

        public required System.Collections.Generic.List<string> NetworkPlatforms { get; set; }
        public required System.Collections.Generic.List<string> Optics { get; set; }
        public required double Orientation { get; set; }
        public required System.Collections.Generic.List<ReferencePoint> ReferencePoints { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("networkPlatforms", NetworkPlatforms);
            yield return ("optics", Optics);
            yield return ("orientation", Orientation);
            yield return ("referencePoints", ReferencePoints);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(PersonaTypeEnumConverter))]
    public enum PersonaType
    {
        HANDICAPPED,
        OCCASIONAL_TRAVELLER,
        FREQUENT_TRAVELLER,
    }

    public static class PersonaTypeFastEnum
    {
        public static string ToString(PersonaType value) => value switch
        {
            PersonaType.HANDICAPPED => "HANDICAPPED",
            PersonaType.OCCASIONAL_TRAVELLER => "OCCASIONAL_TRAVELLER",
            PersonaType.FREQUENT_TRAVELLER => "FREQUENT_TRAVELLER",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static PersonaType FromString(string? value) => value switch
        {
            "HANDICAPPED" => PersonaType.HANDICAPPED,
            "OCCASIONAL_TRAVELLER" => PersonaType.OCCASIONAL_TRAVELLER,
            "FREQUENT_TRAVELLER" => PersonaType.FREQUENT_TRAVELLER,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class PersonaTypeEnumConverter : System.Text.Json.Serialization.JsonConverter<PersonaType>
    {
        public override PersonaType Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (PersonaType)reader.GetInt32();
            }

            return PersonaTypeFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, PersonaType value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(PersonaTypeFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class PhoneNumber : __ICanIterate
    {
        public PhoneNumber() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public PhoneNumber(string number, PhoneNumberType type)
        {
            Number = number;
            Type = type;
        }

        public required string Number { get; set; }
        public required PhoneNumberType Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("number", Number);
            yield return ("type", Type switch
            {
                PhoneNumberType.BUSINESS => "BUSINESS",
                PhoneNumberType.MOBILE => "MOBILE",
                PhoneNumberType.FAX => "FAX",
                _ => null
            });
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(PhoneNumberTypeEnumConverter))]
    public enum PhoneNumberType
    {
        BUSINESS,
        MOBILE,
        FAX,
    }

    public static class PhoneNumberTypeFastEnum
    {
        public static string ToString(PhoneNumberType value) => value switch
        {
            PhoneNumberType.BUSINESS => "BUSINESS",
            PhoneNumberType.MOBILE => "MOBILE",
            PhoneNumberType.FAX => "FAX",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static PhoneNumberType FromString(string? value) => value switch
        {
            "BUSINESS" => PhoneNumberType.BUSINESS,
            "MOBILE" => PhoneNumberType.MOBILE,
            "FAX" => PhoneNumberType.FAX,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class PhoneNumberTypeEnumConverter : System.Text.Json.Serialization.JsonConverter<PhoneNumberType>
    {
        public override PhoneNumberType Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (PhoneNumberType)reader.GetInt32();
            }

            return PhoneNumberTypeFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, PhoneNumberType value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(PhoneNumberTypeFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Platform : __ICanIterate
    {
        public Platform() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Platform(Accessibility accessibility, double end, bool headPlatform, System.Collections.Generic.List<PlatformHeight> heights, string ifopt, double length, System.Collections.Generic.List<string> linkedPlatforms, string name, Operational operational, string parentPlatform, System.Collections.Generic.List<Sector> sectors, double start)
        {
            Accessibility = accessibility;
            End = end;
            HeadPlatform = headPlatform;
            Heights = heights;
            Ifopt = ifopt;
            Length = length;
            LinkedPlatforms = linkedPlatforms;
            Name = name;
            Operational = operational;
            ParentPlatform = parentPlatform;
            Sectors = sectors;
            Start = start;
        }

        public required Accessibility Accessibility { get; set; }
        public required double End { get; set; }
        public required bool HeadPlatform { get; set; }
        public required System.Collections.Generic.List<PlatformHeight> Heights { get; set; }
        public required string Ifopt { get; set; }
        public required double Length { get; set; }
        public required System.Collections.Generic.List<string> LinkedPlatforms { get; set; }
        public required string Name { get; set; }
        public required Operational Operational { get; set; }
        public required string ParentPlatform { get; set; }
        public required System.Collections.Generic.List<Sector> Sectors { get; set; }
        public required double Start { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("accessibility", Accessibility);
            yield return ("end", End);
            yield return ("headPlatform", HeadPlatform);
            yield return ("heights", Heights);
            yield return ("ifopt", Ifopt);
            yield return ("length", Length);
            yield return ("linkedPlatforms", LinkedPlatforms);
            yield return ("name", Name);
            yield return ("operational", Operational);
            yield return ("parentPlatform", ParentPlatform);
            yield return ("sectors", Sectors);
            yield return ("start", Start);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class PlatformHeight : __ICanIterate
    {
        public PlatformHeight() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public PlatformHeight(double end, double height, double start)
        {
            End = end;
            Height = height;
            Start = start;
        }

        public required double End { get; set; }
        public required double Height { get; set; }
        public required double Start { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("end", End);
            yield return ("height", Height);
            yield return ("start", Start);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Platforms : __ICanIterate
    {
        public Platforms() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Platforms(System.Collections.Generic.List<Platform> platforms)
        {
            PlatformsList = platforms;
        }

        public required System.Collections.Generic.List<Platform> PlatformsList { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("platforms", PlatformsList);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class ReferencePoint : __ICanIterate
    {
        public ReferencePoint() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public ReferencePoint(double maxLength, string name, double position, bool readableFromOrigin, ReferencePointType referencePointType, string uuid)
        {
            MaxLength = maxLength;
            Name = name;
            Position = position;
            ReadableFromOrigin = readableFromOrigin;
            ReferencePointType = referencePointType;
            Uuid = uuid;
        }

        public required double MaxLength { get; set; }
        public required string Name { get; set; }
        public required double Position { get; set; }
        public required bool ReadableFromOrigin { get; set; }
        public required ReferencePointType ReferencePointType { get; set; }
        public required string Uuid { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("maxLength", MaxLength);
            yield return ("name", Name);
            yield return ("position", Position);
            yield return ("readableFromOrigin", ReadableFromOrigin);
            yield return ("referencePointType", ReferencePointType switch
            {
                ReferencePointType.STOP_SIGNAL => "STOP_SIGNAL",
                ReferencePointType.STOP_BOARD => "STOP_BOARD",
                _ => null
            });
            yield return ("uuid", Uuid);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(ReferencePointTypeEnumConverter))]
    public enum ReferencePointType
    {
        STOP_SIGNAL,
        STOP_BOARD,
    }

    public static class ReferencePointTypeFastEnum
    {
        public static string ToString(ReferencePointType value) => value switch
        {
            ReferencePointType.STOP_SIGNAL => "STOP_SIGNAL",
            ReferencePointType.STOP_BOARD => "STOP_BOARD",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static ReferencePointType FromString(string? value) => value switch
        {
            "STOP_SIGNAL" => ReferencePointType.STOP_SIGNAL,
            "STOP_BOARD" => ReferencePointType.STOP_BOARD,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class ReferencePointTypeEnumConverter : System.Text.Json.Serialization.JsonConverter<ReferencePointType>
    {
        public override ReferencePointType Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (ReferencePointType)reader.GetInt32();
            }

            return ReferencePointTypeFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, ReferencePointType value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(ReferencePointTypeFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class ReplacementTransportStop : __ICanIterate
    {
        public ReplacementTransportStop() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public ReplacementTransportStop(System.Collections.Generic.Dictionary<string, object> names, Coordinate2D position, string replacementTransportStopID)
        {
            Names = names;
            Position = position;
            ReplacementTransportStopID = replacementTransportStopID;
        }

        public required System.Collections.Generic.Dictionary<string, object> Names { get; set; }
        public required Coordinate2D Position { get; set; }
        public required string ReplacementTransportStopID { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("names", Names);
            yield return ("position", Position);
            yield return ("replacementTransportStopID", ReplacementTransportStopID);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class ReplacementTransportStopResult : __ICanIterate
    {
        public ReplacementTransportStopResult() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public ReplacementTransportStopResult(System.Collections.Generic.List<ReplacementTransportStop> replacementTransportStops)
        {
            ReplacementTransportStops = replacementTransportStops;
        }

        public required System.Collections.Generic.List<ReplacementTransportStop> ReplacementTransportStops { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("replacementTransportStops", ReplacementTransportStops);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Sector : __ICanIterate
    {
        public Sector() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Sector(double cubePosition, bool cubeSignage, double end, string name, double start)
        {
            CubePosition = cubePosition;
            CubeSignage = cubeSignage;
            End = end;
            Name = name;
            Start = start;
        }

        public required double CubePosition { get; set; }
        public required bool CubeSignage { get; set; }
        public required double End { get; set; }
        public required string Name { get; set; }
        public required double Start { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("cubePosition", CubePosition);
            yield return ("cubeSignage", CubeSignage);
            yield return ("end", End);
            yield return ("name", Name);
            yield return ("start", Start);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class State : __ICanIterate
    {
        public State() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public State(string explanation, System.DateTime recommissioningDate, string type)
        {
            Explanation = explanation;
            RecommissioningDate = recommissioningDate;
            Type = type;
        }

        public required string Explanation { get; set; }
        public required System.DateTime RecommissioningDate { get; set; }
        public required string Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("explanation", Explanation);
            yield return ("recommissioningDate", RecommissioningDate);
            yield return ("type", Type);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Station : __ICanIterate
    {
        public Station() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Station(AddressWithWeb address, System.Collections.Generic.List<string> availableLocalServices, System.Collections.Generic.List<TransportType> availableTransports, string countryCode, System.Collections.Generic.Dictionary<string, string> metropolis, bool mobilityServiceStaffOnSite, string municipalityKey, System.Collections.Generic.Dictionary<string, object> names, string openingHours, StationOwner owner, Coordinate2D position, StationRoofingType roofing, string state, StationCategory stationCategory, string stationID, string timeZone, System.Collections.Generic.List<string> transportAssociations, System.DateTime validFrom, System.DateTime validTo)
        {
            Address = address;
            AvailableLocalServices = availableLocalServices;
            AvailableTransports = availableTransports;
            CountryCode = countryCode;
            Metropolis = metropolis;
            MobilityServiceStaffOnSite = mobilityServiceStaffOnSite;
            MunicipalityKey = municipalityKey;
            Names = names;
            OpeningHours = openingHours;
            Owner = owner;
            Position = position;
            Roofing = roofing;
            State = state;
            StationCategory = stationCategory;
            StationID = stationID;
            TimeZone = timeZone;
            TransportAssociations = transportAssociations;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }

        public required AddressWithWeb Address { get; set; }
        public required System.Collections.Generic.List<string> AvailableLocalServices { get; set; }
        public required System.Collections.Generic.List<TransportType> AvailableTransports { get; set; }
        public required string CountryCode { get; set; }
        public required System.Collections.Generic.Dictionary<string, string> Metropolis { get; set; }
        public required bool MobilityServiceStaffOnSite { get; set; }
        public required string MunicipalityKey { get; set; }
        public required System.Collections.Generic.Dictionary<string, object> Names { get; set; }
        public required string OpeningHours { get; set; }
        public required StationOwner Owner { get; set; }
        public required Coordinate2D Position { get; set; }
        public required StationRoofingType Roofing { get; set; }
        public required string State { get; set; }
        public required StationCategory StationCategory { get; set; }
        public required string StationID { get; set; }
        public required string TimeZone { get; set; }
        public required System.Collections.Generic.List<string> TransportAssociations { get; set; }
        public required System.DateTime ValidFrom { get; set; }
        public required System.DateTime ValidTo { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("address", Address);
            yield return ("availableLocalServices", AvailableLocalServices);
            yield return ("availableTransports", AvailableTransports);
            yield return ("countryCode", CountryCode);
            yield return ("metropolis", Metropolis);
            yield return ("mobilityServiceStaffOnSite", MobilityServiceStaffOnSite);
            yield return ("municipalityKey", MunicipalityKey);
            yield return ("names", Names);
            yield return ("openingHours", OpeningHours);
            yield return ("owner", Owner);
            yield return ("position", Position);
            yield return ("roofing", Roofing switch
            {
                StationRoofingType.COVERED => "COVERED",
                StationRoofingType.PARTIALLY_COVERED => "PARTIALLY_COVERED",
                StationRoofingType.NOT_COVERED => "NOT_COVERED",
                _ => null
            });
            yield return ("state", State);
            yield return ("stationCategory", StationCategory switch
            {
                StationCategory.CATEGORY_1 => "CATEGORY_1",
                StationCategory.CATEGORY_2 => "CATEGORY_2",
                StationCategory.CATEGORY_3 => "CATEGORY_3",
                StationCategory.CATEGORY_4 => "CATEGORY_4",
                StationCategory.CATEGORY_5 => "CATEGORY_5",
                StationCategory.CATEGORY_6 => "CATEGORY_6",
                StationCategory.CATEGORY_7 => "CATEGORY_7",
                _ => null
            });
            yield return ("stationID", StationID);
            yield return ("timeZone", TimeZone);
            yield return ("transportAssociations", TransportAssociations);
            yield return ("validFrom", ValidFrom);
            yield return ("validTo", ValidTo);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(StationCategoryEnumConverter))]
    public enum StationCategory
    {
        CATEGORY_1,
        CATEGORY_2,
        CATEGORY_3,
        CATEGORY_4,
        CATEGORY_5,
        CATEGORY_6,
        CATEGORY_7,
    }

    public static class StationCategoryFastEnum
    {
        public static string ToString(StationCategory value) => value switch
        {
            StationCategory.CATEGORY_1 => "CATEGORY_1",
            StationCategory.CATEGORY_2 => "CATEGORY_2",
            StationCategory.CATEGORY_3 => "CATEGORY_3",
            StationCategory.CATEGORY_4 => "CATEGORY_4",
            StationCategory.CATEGORY_5 => "CATEGORY_5",
            StationCategory.CATEGORY_6 => "CATEGORY_6",
            StationCategory.CATEGORY_7 => "CATEGORY_7",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static StationCategory FromString(string? value) => value switch
        {
            "CATEGORY_1" => StationCategory.CATEGORY_1,
            "CATEGORY_2" => StationCategory.CATEGORY_2,
            "CATEGORY_3" => StationCategory.CATEGORY_3,
            "CATEGORY_4" => StationCategory.CATEGORY_4,
            "CATEGORY_5" => StationCategory.CATEGORY_5,
            "CATEGORY_6" => StationCategory.CATEGORY_6,
            "CATEGORY_7" => StationCategory.CATEGORY_7,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class StationCategoryEnumConverter : System.Text.Json.Serialization.JsonConverter<StationCategory>
    {
        public override StationCategory Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (StationCategory)reader.GetInt32();
            }

            return StationCategoryFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, StationCategory value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(StationCategoryFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StationEquipment : __ICanIterate
    {
        public StationEquipment() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StationEquipment(string equipmentID, Escalator escalator, Lift lift, LockerRack lockerRack, string stationID, string type)
        {
            EquipmentID = equipmentID;
            Escalator = escalator;
            Lift = lift;
            LockerRack = lockerRack;
            StationID = stationID;
            Type = type;
        }

        public required string EquipmentID { get; set; }
        public required Escalator Escalator { get; set; }
        public required Lift Lift { get; set; }
        public required LockerRack LockerRack { get; set; }
        public required string StationID { get; set; }
        public required string Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("equipmentID", EquipmentID);
            yield return ("escalator", Escalator);
            yield return ("lift", Lift);
            yield return ("lockerRack", LockerRack);
            yield return ("stationID", StationID);
            yield return ("type", Type);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StationEquipments : __ICanIterate
    {
        public StationEquipments() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StationEquipments(System.Collections.Generic.List<StationEquipment> stationEquipments)
        {
            StationEquipmentsList = stationEquipments;
        }

        public required System.Collections.Generic.List<StationEquipment> StationEquipmentsList { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("stationEquipments", StationEquipmentsList);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StationName : __ICanIterate
    {
        public StationName() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StationName(string name)
        {
            Name = name;
        }

        public required string Name { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("name", Name);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StationOrganisationalUnit : __ICanIterate
    {
        public StationOrganisationalUnit() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StationOrganisationalUnit(int id, string name, string nameShort)
        {
            Id = id;
            Name = name;
            NameShort = nameShort;
        }

        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string NameShort { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("id", Id);
            yield return ("name", Name);
            yield return ("nameShort", NameShort);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StationOwner : __ICanIterate
    {
        public StationOwner() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StationOwner(string name, StationOrganisationalUnit organisationalUnit)
        {
            Name = name;
            OrganisationalUnit = organisationalUnit;
        }

        public required string Name { get; set; }
        public required StationOrganisationalUnit OrganisationalUnit { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("name", Name);
            yield return ("organisationalUnit", OrganisationalUnit);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(StationRoofingTypeEnumConverter))]
    public enum StationRoofingType
    {
        COVERED,
        PARTIALLY_COVERED,
        NOT_COVERED,
    }

    public static class StationRoofingTypeFastEnum
    {
        public static string ToString(StationRoofingType value) => value switch
        {
            StationRoofingType.COVERED => "COVERED",
            StationRoofingType.PARTIALLY_COVERED => "PARTIALLY_COVERED",
            StationRoofingType.NOT_COVERED => "NOT_COVERED",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static StationRoofingType FromString(string? value) => value switch
        {
            "COVERED" => StationRoofingType.COVERED,
            "PARTIALLY_COVERED" => StationRoofingType.PARTIALLY_COVERED,
            "NOT_COVERED" => StationRoofingType.NOT_COVERED,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class StationRoofingTypeEnumConverter : System.Text.Json.Serialization.JsonConverter<StationRoofingType>
    {
        public override StationRoofingType Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (StationRoofingType)reader.GetInt32();
            }

            return StationRoofingTypeFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, StationRoofingType value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(StationRoofingTypeFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Stations : __ICanIterate
    {
        public Stations() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Stations(System.Collections.Generic.List<Station> stations)
        {
            StationsList = stations;
        }

        public required System.Collections.Generic.List<Station> StationsList { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("stations", StationsList);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StationsPageable : __ICanIterate
    {
        public StationsPageable() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StationsPageable(int limit, int offset, System.Collections.Generic.List<Station> stations, int total)
        {
            Limit = limit;
            Offset = offset;
            Stations = stations;
            Total = total;
        }

        public required int Limit { get; set; }
        public required int Offset { get; set; }
        public required System.Collections.Generic.List<Station> Stations { get; set; }
        public required int Total { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("limit", Limit);
            yield return ("offset", Offset);
            yield return ("stations", Stations);
            yield return ("total", Total);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StationTransportCompanies : __ICanIterate
    {
        public StationTransportCompanies() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StationTransportCompanies(System.Collections.Generic.List<StationTransportCompany> transportCompanies)
        {
            TransportCompanies = transportCompanies;
        }

        public required System.Collections.Generic.List<StationTransportCompany> TransportCompanies { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("transportCompanies", TransportCompanies);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StationTransportCompany : __ICanIterate
    {
        public StationTransportCompany() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StationTransportCompany(string email, string name, string phoneNumber, string shortName, string website, string websiteDigitalAssistant)
        {
            Email = email;
            Name = name;
            PhoneNumber = phoneNumber;
            ShortName = shortName;
            Website = website;
            WebsiteDigitalAssistant = websiteDigitalAssistant;
        }

        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string PhoneNumber { get; set; }
        public required string ShortName { get; set; }
        public required string Website { get; set; }
        public required string WebsiteDigitalAssistant { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("email", Email);
            yield return ("name", Name);
            yield return ("phoneNumber", PhoneNumber);
            yield return ("shortName", ShortName);
            yield return ("website", Website);
            yield return ("websiteDigitalAssistant", WebsiteDigitalAssistant);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopPlace : __ICanIterate
    {
        public StopPlace() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlace(System.Collections.Generic.List<TransportType> availablePhysicalTransports, System.Collections.Generic.List<TransportType> availableTransports, string countryCode, string evaNumber, System.Collections.Generic.Dictionary<string, string> metropolis, string municipalityKey, System.Collections.Generic.Dictionary<string, object> names, Coordinate2D position, string postalCode, bool replacementTransportsAvailable, string state, string stationID, string timeZone, System.Collections.Generic.List<string> transportAssociations)
        {
            AvailablePhysicalTransports = availablePhysicalTransports;
            AvailableTransports = availableTransports;
            CountryCode = countryCode;
            EvaNumber = evaNumber;
            Metropolis = metropolis;
            MunicipalityKey = municipalityKey;
            Names = names;
            Position = position;
            PostalCode = postalCode;
            ReplacementTransportsAvailable = replacementTransportsAvailable;
            State = state;
            StationID = stationID;
            TimeZone = timeZone;
            TransportAssociations = transportAssociations;
        }

        public required System.Collections.Generic.List<TransportType> AvailablePhysicalTransports { get; set; }
        public required System.Collections.Generic.List<TransportType> AvailableTransports { get; set; }
        public required string CountryCode { get; set; }
        public required string EvaNumber { get; set; }
        public required System.Collections.Generic.Dictionary<string, string> Metropolis { get; set; }
        public required string MunicipalityKey { get; set; }
        public required System.Collections.Generic.Dictionary<string, object> Names { get; set; }
        public required Coordinate2D Position { get; set; }
        public required string PostalCode { get; set; }
        public required bool ReplacementTransportsAvailable { get; set; }
        public required string State { get; set; }
        public required string StationID { get; set; }
        public required string TimeZone { get; set; }
        public required System.Collections.Generic.List<string> TransportAssociations { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("availablePhysicalTransports", AvailablePhysicalTransports);
            yield return ("availableTransports", AvailableTransports);
            yield return ("countryCode", CountryCode);
            yield return ("evaNumber", EvaNumber);
            yield return ("metropolis", Metropolis);
            yield return ("municipalityKey", MunicipalityKey);
            yield return ("names", Names);
            yield return ("position", Position);
            yield return ("postalCode", PostalCode);
            yield return ("replacementTransportsAvailable", ReplacementTransportsAvailable);
            yield return ("state", State);
            yield return ("stationID", StationID);
            yield return ("timeZone", TimeZone);
            yield return ("transportAssociations", TransportAssociations);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopPlaceGroup : __ICanIterate
    {
        public StopPlaceGroup() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlaceGroup(string groupIdentifier, System.Collections.Generic.List<string> members, StopPlaceGroupType type)
        {
            GroupIdentifier = groupIdentifier;
            Members = members;
            Type = type;
        }

        public required string GroupIdentifier { get; set; }
        public required System.Collections.Generic.List<string> Members { get; set; }
        public required StopPlaceGroupType Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("groupIdentifier", GroupIdentifier);
            yield return ("members", Members);
            yield return ("type", Type switch
            {
                StopPlaceGroupType.STATION => "STATION",
                StopPlaceGroupType.SALES => "SALES",
                StopPlaceGroupType.METROPOLITAN_AREA => "METROPOLITAN_AREA",
                _ => null
            });
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopPlaceGroups : __ICanIterate
    {
        public StopPlaceGroups() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlaceGroups(System.Collections.Generic.List<StopPlaceGroup> groups)
        {
            Groups = groups;
        }

        public required System.Collections.Generic.List<StopPlaceGroup> Groups { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("groups", Groups);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(StopPlaceGroupTypeEnumConverter))]
    public enum StopPlaceGroupType
    {
        STATION,
        SALES,
        METROPOLITAN_AREA,
    }

    public static class StopPlaceGroupTypeFastEnum
    {
        public static string ToString(StopPlaceGroupType value) => value switch
        {
            StopPlaceGroupType.STATION => "STATION",
            StopPlaceGroupType.SALES => "SALES",
            StopPlaceGroupType.METROPOLITAN_AREA => "METROPOLITAN_AREA",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static StopPlaceGroupType FromString(string? value) => value switch
        {
            "STATION" => StopPlaceGroupType.STATION,
            "SALES" => StopPlaceGroupType.SALES,
            "METROPOLITAN_AREA" => StopPlaceGroupType.METROPOLITAN_AREA,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class StopPlaceGroupTypeEnumConverter : System.Text.Json.Serialization.JsonConverter<StopPlaceGroupType>
    {
        public override StopPlaceGroupType Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (StopPlaceGroupType)reader.GetInt32();
            }

            return StopPlaceGroupTypeFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, StopPlaceGroupType value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(StopPlaceGroupTypeFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopPlaceKey : __ICanIterate
    {
        public StopPlaceKey() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlaceKey(string key, StopPlaceKeyType type)
        {
            Key = key;
            Type = type;
        }

        public required string Key { get; set; }
        public required StopPlaceKeyType Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("key", Key);
            yield return ("type", Type switch
            {
                StopPlaceKeyType.IFOPT => "IFOPT",
                StopPlaceKeyType.EVA => "EVA",
                StopPlaceKeyType.RL100 => "RL100",
                StopPlaceKeyType.RL100_ALTERNATIVE => "RL100_ALTERNATIVE",
                StopPlaceKeyType.EPA => "EPA",
                StopPlaceKeyType.STADA => "STADA",
                StopPlaceKeyType.IBNR => "IBNR",
                StopPlaceKeyType.EBHF => "EBHF",
                StopPlaceKeyType.UIC => "UIC",
                StopPlaceKeyType.PLC => "PLC",
                _ => null
            });
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(StopPlaceKeyFilterEnumConverter))]
    public enum StopPlaceKeyFilter
    {
        IFOPT,
        EVA,
        RL100,
        EPA,
        STADA,
        IBNR,
        EBHF,
        UIC,
        PLC,
    }

    public static class StopPlaceKeyFilterFastEnum
    {
        public static string ToString(StopPlaceKeyFilter value) => value switch
        {
            StopPlaceKeyFilter.IFOPT => "IFOPT",
            StopPlaceKeyFilter.EVA => "EVA",
            StopPlaceKeyFilter.RL100 => "RL100",
            StopPlaceKeyFilter.EPA => "EPA",
            StopPlaceKeyFilter.STADA => "STADA",
            StopPlaceKeyFilter.IBNR => "IBNR",
            StopPlaceKeyFilter.EBHF => "EBHF",
            StopPlaceKeyFilter.UIC => "UIC",
            StopPlaceKeyFilter.PLC => "PLC",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static StopPlaceKeyFilter FromString(string? value) => value switch
        {
            "IFOPT" => StopPlaceKeyFilter.IFOPT,
            "EVA" => StopPlaceKeyFilter.EVA,
            "RL100" => StopPlaceKeyFilter.RL100,
            "EPA" => StopPlaceKeyFilter.EPA,
            "STADA" => StopPlaceKeyFilter.STADA,
            "IBNR" => StopPlaceKeyFilter.IBNR,
            "EBHF" => StopPlaceKeyFilter.EBHF,
            "UIC" => StopPlaceKeyFilter.UIC,
            "PLC" => StopPlaceKeyFilter.PLC,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class StopPlaceKeyFilterEnumConverter : System.Text.Json.Serialization.JsonConverter<StopPlaceKeyFilter>
    {
        public override StopPlaceKeyFilter Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (StopPlaceKeyFilter)reader.GetInt32();
            }

            return StopPlaceKeyFilterFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, StopPlaceKeyFilter value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(StopPlaceKeyFilterFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopPlaceKeys : __ICanIterate
    {
        public StopPlaceKeys() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlaceKeys(System.Collections.Generic.List<StopPlaceKey> keys)
        {
            Keys = keys;
        }

        public required System.Collections.Generic.List<StopPlaceKey> Keys { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("keys", Keys);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(StopPlaceKeyTypeEnumConverter))]
    public enum StopPlaceKeyType
    {
        IFOPT,
        EVA,
        RL100,
        RL100_ALTERNATIVE,
        EPA,
        STADA,
        IBNR,
        EBHF,
        UIC,
        PLC,
    }

    public static class StopPlaceKeyTypeFastEnum
    {
        public static string ToString(StopPlaceKeyType value) => value switch
        {
            StopPlaceKeyType.IFOPT => "IFOPT",
            StopPlaceKeyType.EVA => "EVA",
            StopPlaceKeyType.RL100 => "RL100",
            StopPlaceKeyType.RL100_ALTERNATIVE => "RL100_ALTERNATIVE",
            StopPlaceKeyType.EPA => "EPA",
            StopPlaceKeyType.STADA => "STADA",
            StopPlaceKeyType.IBNR => "IBNR",
            StopPlaceKeyType.EBHF => "EBHF",
            StopPlaceKeyType.UIC => "UIC",
            StopPlaceKeyType.PLC => "PLC",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static StopPlaceKeyType FromString(string? value) => value switch
        {
            "IFOPT" => StopPlaceKeyType.IFOPT,
            "EVA" => StopPlaceKeyType.EVA,
            "RL100" => StopPlaceKeyType.RL100,
            "RL100_ALTERNATIVE" => StopPlaceKeyType.RL100_ALTERNATIVE,
            "EPA" => StopPlaceKeyType.EPA,
            "STADA" => StopPlaceKeyType.STADA,
            "IBNR" => StopPlaceKeyType.IBNR,
            "EBHF" => StopPlaceKeyType.EBHF,
            "UIC" => StopPlaceKeyType.UIC,
            "PLC" => StopPlaceKeyType.PLC,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class StopPlaceKeyTypeEnumConverter : System.Text.Json.Serialization.JsonConverter<StopPlaceKeyType>
    {
        public override StopPlaceKeyType Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (StopPlaceKeyType)reader.GetInt32();
            }

            return StopPlaceKeyTypeFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, StopPlaceKeyType value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(StopPlaceKeyTypeFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopPlaceMultiGroup : __ICanIterate
    {
        public StopPlaceMultiGroup() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlaceMultiGroup(string groupIdentifier, System.Collections.Generic.List<string> members, string type)
        {
            GroupIdentifier = groupIdentifier;
            Members = members;
            Type = type;
        }

        public required string GroupIdentifier { get; set; }
        public required System.Collections.Generic.List<string> Members { get; set; }
        public required string Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("groupIdentifier", GroupIdentifier);
            yield return ("members", Members);
            yield return ("type", Type);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopPlaceMultiGroups : __ICanIterate
    {
        public StopPlaceMultiGroups() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlaceMultiGroups(System.Collections.Generic.List<StopPlaceMultiGroup> groups)
        {
            Groups = groups;
        }

        public required System.Collections.Generic.List<StopPlaceMultiGroup> Groups { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("groups", Groups);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopPlaceName : __ICanIterate
    {
        public StopPlaceName() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlaceName(string nameLocal, string nameLong, string nameShort, string speechLong, string speechShort, string symbol, System.Collections.Generic.List<string> synonyms)
        {
            NameLocal = nameLocal;
            NameLong = nameLong;
            NameShort = nameShort;
            SpeechLong = speechLong;
            SpeechShort = speechShort;
            Symbol = symbol;
            Synonyms = synonyms;
        }

        public required string NameLocal { get; set; }
        public required string NameLong { get; set; }
        public required string NameShort { get; set; }
        public required string SpeechLong { get; set; }
        public required string SpeechShort { get; set; }
        public required string Symbol { get; set; }
        public required System.Collections.Generic.List<string> Synonyms { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("nameLocal", NameLocal);
            yield return ("nameLong", NameLong);
            yield return ("nameShort", NameShort);
            yield return ("speechLong", SpeechLong);
            yield return ("speechShort", SpeechShort);
            yield return ("symbol", Symbol);
            yield return ("synonyms", Synonyms);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopPlaces : __ICanIterate
    {
        public StopPlaces() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlaces(System.Collections.Generic.List<StopPlace> stopPlaces)
        {
            StopPlacesList = stopPlaces;
        }

        public required System.Collections.Generic.List<StopPlace> StopPlacesList { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("stopPlaces", StopPlacesList);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopPlacesByKeysEntry : __ICanIterate
    {
        public StopPlacesByKeysEntry() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlacesByKeysEntry(string key)
        {
            Key = key;
        }

        public required string Key { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("key", Key);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopPlacesByKeysRequest : __ICanIterate
    {
        public StopPlacesByKeysRequest() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlacesByKeysRequest(string keyType, System.Collections.Generic.List<StopPlacesByKeysEntry> keys)
        {
            KeyType = keyType;
            Keys = keys;
        }

        public required string KeyType { get; set; }
        public required System.Collections.Generic.List<StopPlacesByKeysEntry> Keys { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("keyType", KeyType);
            yield return ("keys", Keys);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(StopPlaceSearchGroupByKeyEnumConverter))]
    public enum StopPlaceSearchGroupByKey
    {
        STATION,
        SALES,
        NONE,
    }

    public static class StopPlaceSearchGroupByKeyFastEnum
    {
        public static string ToString(StopPlaceSearchGroupByKey value) => value switch
        {
            StopPlaceSearchGroupByKey.STATION => "STATION",
            StopPlaceSearchGroupByKey.SALES => "SALES",
            StopPlaceSearchGroupByKey.NONE => "NONE",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static StopPlaceSearchGroupByKey FromString(string? value) => value switch
        {
            "STATION" => StopPlaceSearchGroupByKey.STATION,
            "SALES" => StopPlaceSearchGroupByKey.SALES,
            "NONE" => StopPlaceSearchGroupByKey.NONE,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class StopPlaceSearchGroupByKeyEnumConverter : System.Text.Json.Serialization.JsonConverter<StopPlaceSearchGroupByKey>
    {
        public override StopPlaceSearchGroupByKey Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (StopPlaceSearchGroupByKey)reader.GetInt32();
            }

            return StopPlaceSearchGroupByKeyFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, StopPlaceSearchGroupByKey value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(StopPlaceSearchGroupByKeyFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopPlaceSearchResult : __ICanIterate
    {
        public StopPlaceSearchResult() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlaceSearchResult(System.Collections.Generic.List<TransportType> availableTransports, string evaNumber, System.Collections.Generic.List<string> groupMembers, System.Collections.Generic.Dictionary<string, object> names, Coordinate2D position, bool replacementTransportsAvailable, string stationID)
        {
            AvailableTransports = availableTransports;
            EvaNumber = evaNumber;
            GroupMembers = groupMembers;
            Names = names;
            Position = position;
            ReplacementTransportsAvailable = replacementTransportsAvailable;
            StationID = stationID;
        }

        public required System.Collections.Generic.List<TransportType> AvailableTransports { get; set; }
        public required string EvaNumber { get; set; }
        public required System.Collections.Generic.List<string> GroupMembers { get; set; }
        public required System.Collections.Generic.Dictionary<string, object> Names { get; set; }
        public required Coordinate2D Position { get; set; }
        public required bool ReplacementTransportsAvailable { get; set; }
        public required string StationID { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("availableTransports", AvailableTransports);
            yield return ("evaNumber", EvaNumber);
            yield return ("groupMembers", GroupMembers);
            yield return ("names", Names);
            yield return ("position", Position);
            yield return ("replacementTransportsAvailable", ReplacementTransportsAvailable);
            yield return ("stationID", StationID);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopPlaceSearchResults : __ICanIterate
    {
        public StopPlaceSearchResults() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlaceSearchResults(System.Collections.Generic.List<StopPlaceSearchResult> stopPlaces)
        {
            StopPlaces = stopPlaces;
        }

        public required System.Collections.Generic.List<StopPlaceSearchResult> StopPlaces { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("stopPlaces", StopPlaces);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(StopPlaceSortKeyEnumConverter))]
    public enum StopPlaceSortKey
    {
        RELEVANCE,
        QUERY_MATCH,
    }

    public static class StopPlaceSortKeyFastEnum
    {
        public static string ToString(StopPlaceSortKey value) => value switch
        {
            StopPlaceSortKey.RELEVANCE => "RELEVANCE",
            StopPlaceSortKey.QUERY_MATCH => "QUERY_MATCH",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static StopPlaceSortKey FromString(string? value) => value switch
        {
            "RELEVANCE" => StopPlaceSortKey.RELEVANCE,
            "QUERY_MATCH" => StopPlaceSortKey.QUERY_MATCH,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class StopPlaceSortKeyEnumConverter : System.Text.Json.Serialization.JsonConverter<StopPlaceSortKey>
    {
        public override StopPlaceSortKey Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (StopPlaceSortKey)reader.GetInt32();
            }

            return StopPlaceSortKeyFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, StopPlaceSortKey value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(StopPlaceSortKeyFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(TransportTypeEnumConverter))]
    public enum TransportType
    {
        HIGH_SPEED_TRAIN,
        INTERCITY_TRAIN,
        INTER_REGIONAL_TRAIN,
        REGIONAL_TRAIN,
        CITY_TRAIN,
        SUBWAY,
        TRAM,
        BUS,
        FERRY,
        FLIGHT,
        CAR,
        TAXI,
        SHUTTLE,
        BIKE,
        SCOOTER,
        WALK,
        UNKNOWN,
    }

    public static class TransportTypeFastEnum
    {
        public static string ToString(TransportType value) => value switch
        {
            TransportType.HIGH_SPEED_TRAIN => "HIGH_SPEED_TRAIN",
            TransportType.INTERCITY_TRAIN => "INTERCITY_TRAIN",
            TransportType.INTER_REGIONAL_TRAIN => "INTER_REGIONAL_TRAIN",
            TransportType.REGIONAL_TRAIN => "REGIONAL_TRAIN",
            TransportType.CITY_TRAIN => "CITY_TRAIN",
            TransportType.SUBWAY => "SUBWAY",
            TransportType.TRAM => "TRAM",
            TransportType.BUS => "BUS",
            TransportType.FERRY => "FERRY",
            TransportType.FLIGHT => "FLIGHT",
            TransportType.CAR => "CAR",
            TransportType.TAXI => "TAXI",
            TransportType.SHUTTLE => "SHUTTLE",
            TransportType.BIKE => "BIKE",
            TransportType.SCOOTER => "SCOOTER",
            TransportType.WALK => "WALK",
            TransportType.UNKNOWN => "UNKNOWN",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static TransportType FromString(string? value) => value switch
        {
            "HIGH_SPEED_TRAIN" => TransportType.HIGH_SPEED_TRAIN,
            "INTERCITY_TRAIN" => TransportType.INTERCITY_TRAIN,
            "INTER_REGIONAL_TRAIN" => TransportType.INTER_REGIONAL_TRAIN,
            "REGIONAL_TRAIN" => TransportType.REGIONAL_TRAIN,
            "CITY_TRAIN" => TransportType.CITY_TRAIN,
            "SUBWAY" => TransportType.SUBWAY,
            "TRAM" => TransportType.TRAM,
            "BUS" => TransportType.BUS,
            "FERRY" => TransportType.FERRY,
            "FLIGHT" => TransportType.FLIGHT,
            "CAR" => TransportType.CAR,
            "TAXI" => TransportType.TAXI,
            "SHUTTLE" => TransportType.SHUTTLE,
            "BIKE" => TransportType.BIKE,
            "SCOOTER" => TransportType.SCOOTER,
            "WALK" => TransportType.WALK,
            "UNKNOWN" => TransportType.UNKNOWN,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class TransportTypeEnumConverter : System.Text.Json.Serialization.JsonConverter<TransportType>
    {
        public override TransportType Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (TransportType)reader.GetInt32();
            }

            return TransportTypeFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, TransportType value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(TransportTypeFastEnum.ToString(value));
        }
    }
}
