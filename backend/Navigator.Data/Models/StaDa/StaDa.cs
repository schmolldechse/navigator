using Navigator.Data.Models.Ris;

namespace Navigator.Data.Models.StaDa;

public class StaDa
{
    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Address : __ICanIterate
    {
        public Address() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Address(string city, string houseNumber, string street, string zipcode)
        {
            City = city;
            HouseNumber = houseNumber;
            Street = street;
            Zipcode = zipcode;
        }

        public required string City { get; set; }
        public required string HouseNumber { get; set; }
        public required string Street { get; set; }
        public required string Zipcode { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("city", City);
            yield return ("houseNumber", HouseNumber);
            yield return ("street", Street);
            yield return ("zipcode", Zipcode);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Aufgabentraeger : __ICanIterate
    {
        public Aufgabentraeger() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Aufgabentraeger(string name, string shortName)
        {
            Name = name;
            ShortName = shortName;
        }

        public required string Name { get; set; }
        public required string ShortName { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("name", Name);
            yield return ("shortName", ShortName);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class DoubleSchedule : __ICanIterate
    {
        public DoubleSchedule() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public DoubleSchedule(OpeningHours friday1, OpeningHours friday2, OpeningHours monday1, OpeningHours monday2, OpeningHours saturday1, OpeningHours saturday2, OpeningHours sunday1, OpeningHours sunday2, OpeningHours thursday1, OpeningHours thursday2, OpeningHours tuesday1, OpeningHours tuesday2, OpeningHours wednesday1, OpeningHours wednesday2)
        {
            Friday1 = friday1;
            Friday2 = friday2;
            Monday1 = monday1;
            Monday2 = monday2;
            Saturday1 = saturday1;
            Saturday2 = saturday2;
            Sunday1 = sunday1;
            Sunday2 = sunday2;
            Thursday1 = thursday1;
            Thursday2 = thursday2;
            Tuesday1 = tuesday1;
            Tuesday2 = tuesday2;
            Wednesday1 = wednesday1;
            Wednesday2 = wednesday2;
        }

        public required OpeningHours Friday1 { get; set; }
        public required OpeningHours Friday2 { get; set; }
        public required OpeningHours Monday1 { get; set; }
        public required OpeningHours Monday2 { get; set; }
        public required OpeningHours Saturday1 { get; set; }
        public required OpeningHours Saturday2 { get; set; }
        public required OpeningHours Sunday1 { get; set; }
        public required OpeningHours Sunday2 { get; set; }
        public required OpeningHours Thursday1 { get; set; }
        public required OpeningHours Thursday2 { get; set; }
        public required OpeningHours Tuesday1 { get; set; }
        public required OpeningHours Tuesday2 { get; set; }
        public required OpeningHours Wednesday1 { get; set; }
        public required OpeningHours Wednesday2 { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("friday1", Friday1);
            yield return ("friday2", Friday2);
            yield return ("monday1", Monday1);
            yield return ("monday2", Monday2);
            yield return ("saturday1", Saturday1);
            yield return ("saturday2", Saturday2);
            yield return ("sunday1", Sunday1);
            yield return ("sunday2", Sunday2);
            yield return ("thursday1", Thursday1);
            yield return ("thursday2", Thursday2);
            yield return ("tuesday1", Tuesday1);
            yield return ("tuesday2", Tuesday2);
            yield return ("wednesday1", Wednesday1);
            yield return ("wednesday2", Wednesday2);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Error : __ICanIterate
    {
        public Error() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Error(string errMsg, int errNo)
        {
            ErrMsg = errMsg;
            ErrNo = errNo;
        }

        public required string ErrMsg { get; set; }
        public required int ErrNo { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("errMsg", ErrMsg);
            yield return ("errNo", ErrNo);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class EVANumber : __ICanIterate
    {
        public EVANumber() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public EVANumber(GeographicPoint geographicCoordinates, bool isMain, int number)
        {
            GeographicCoordinates = geographicCoordinates;
            IsMain = isMain;
            Number = number;
        }

        public required GeographicPoint GeographicCoordinates { get; set; }
        public required bool IsMain { get; set; }
        public required int Number { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("geographicCoordinates", GeographicCoordinates);
            yield return ("isMain", IsMain);
            yield return ("number", Number);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class GeographicPoint : __ICanIterate
    {
        public GeographicPoint() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public GeographicPoint(System.Collections.Generic.List<double> coordinates, string type)
        {
            Coordinates = coordinates;
            Type = type;
        }

        public required System.Collections.Generic.List<double> Coordinates { get; set; }
        public required string Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("coordinates", Coordinates);
            yield return ("type", Type);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class MobilityServiceStaff : __ICanIterate
    {
        public MobilityServiceStaff() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public MobilityServiceStaff(DoubleSchedule availability, string meetingPoint, bool serviceOnBehalf, bool staffOnSite)
        {
            Availability = availability;
            MeetingPoint = meetingPoint;
            ServiceOnBehalf = serviceOnBehalf;
            StaffOnSite = staffOnSite;
        }

        public required DoubleSchedule Availability { get; set; }
        public required string MeetingPoint { get; set; }
        public required bool ServiceOnBehalf { get; set; }
        public required bool StaffOnSite { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("availability", Availability);
            yield return ("meetingPoint", MeetingPoint);
            yield return ("serviceOnBehalf", ServiceOnBehalf);
            yield return ("staffOnSite", StaffOnSite);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class OpeningHours : __ICanIterate
    {
        public OpeningHours() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public OpeningHours(string fromTime, string toTime)
        {
            FromTime = fromTime;
            ToTime = toTime;
        }

        public required string FromTime { get; set; }
        public required string ToTime { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("fromTime", FromTime);
            yield return ("toTime", ToTime);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(PartialEnumConverter))]
    public enum Partial
    {
        yes,
        no,
        partial,
    }

    public static class PartialFastEnum
    {
        public static string ToString(Partial value) => value switch
        {
            Partial.yes => "yes",
            Partial.no => "no",
            Partial.partial => "partial",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static Partial FromString(string? value) => value switch
        {
            "yes" => Partial.yes,
            "no" => Partial.no,
            "partial" => Partial.partial,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class PartialEnumConverter : System.Text.Json.Serialization.JsonConverter<Partial>
    {
        public override Partial Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (Partial)reader.GetInt32();
            }

            return PartialFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, Partial value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(PartialFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class RegionalBereich : __ICanIterate
    {
        public RegionalBereich() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public RegionalBereich(string name, int number, string shortName)
        {
            Name = name;
            Number = number;
            ShortName = shortName;
        }

        public required string Name { get; set; }
        public required int Number { get; set; }
        public required string ShortName { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("name", Name);
            yield return ("number", Number);
            yield return ("shortName", ShortName);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class RiL100Identifier : __ICanIterate
    {
        public RiL100Identifier() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public RiL100Identifier(GeographicPoint geographicCoordinates, bool hasSteamPermission, bool isMain, string primaryLocationCode, string rilIdentifier, string steamPermission)
        {
            GeographicCoordinates = geographicCoordinates;
            HasSteamPermission = hasSteamPermission;
            IsMain = isMain;
            PrimaryLocationCode = primaryLocationCode;
            RilIdentifier = rilIdentifier;
            SteamPermission = steamPermission;
        }

        public required GeographicPoint GeographicCoordinates { get; set; }
        public required bool HasSteamPermission { get; set; }
        public required bool IsMain { get; set; }
        public required string PrimaryLocationCode { get; set; }
        public required string RilIdentifier { get; set; }
        public required string SteamPermission { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("geographicCoordinates", GeographicCoordinates);
            yield return ("hasSteamPermission", HasSteamPermission);
            yield return ("isMain", IsMain);
            yield return ("primaryLocationCode", PrimaryLocationCode);
            yield return ("rilIdentifier", RilIdentifier);
            yield return ("steamPermission", SteamPermission);
        }
    }


    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Schedule : __ICanIterate
    {
        public Schedule() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Schedule(object availability)
        {
            Availability = availability;
        }

        public required object Availability { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("availability", Availability);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Station : __ICanIterate
    {
        public Station() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Station(Schedule dBinformation, Aufgabentraeger aufgabentraeger, int category, string countryCode, System.Collections.Generic.List<EVANumber> evaNumbers, string federalState, string federalStateCode, bool hasBicycleParking, bool hasCarRental, bool hasDBLounge, bool hasLocalPublicTransport, bool hasLockerSystem, bool hasLostAndFound, string hasMobilityService, bool hasParking, bool hasPublicFacilities, bool hasRailwayMission, Partial hasSteplessAccess, bool hasTaxiRank, bool hasTravelCenter, bool hasTravelNecessities, bool hasWiFi, string ifopt, Schedule localServiceStaff, Address mailingAddress, MobilityServiceStaff mobilityServiceStaff, string municipalityCode, string name, int number, int priceCategory, RegionalBereich regionalbereich, System.Collections.Generic.List<RiL100Identifier> ril100Identifiers, StationManagement stationManagement, SZentrale szentrale, TimetableOffice timeTableOffice, WirelessLan wirelessLan)
        {
            DBinformation = dBinformation;
            Aufgabentraeger = aufgabentraeger;
            Category = category;
            CountryCode = countryCode;
            EvaNumbers = evaNumbers;
            FederalState = federalState;
            FederalStateCode = federalStateCode;
            HasBicycleParking = hasBicycleParking;
            HasCarRental = hasCarRental;
            HasDBLounge = hasDBLounge;
            HasLocalPublicTransport = hasLocalPublicTransport;
            HasLockerSystem = hasLockerSystem;
            HasLostAndFound = hasLostAndFound;
            HasMobilityService = hasMobilityService;
            HasParking = hasParking;
            HasPublicFacilities = hasPublicFacilities;
            HasRailwayMission = hasRailwayMission;
            HasSteplessAccess = hasSteplessAccess;
            HasTaxiRank = hasTaxiRank;
            HasTravelCenter = hasTravelCenter;
            HasTravelNecessities = hasTravelNecessities;
            HasWiFi = hasWiFi;
            Ifopt = ifopt;
            LocalServiceStaff = localServiceStaff;
            MailingAddress = mailingAddress;
            MobilityServiceStaff = mobilityServiceStaff;
            MunicipalityCode = municipalityCode;
            Name = name;
            Number = number;
            PriceCategory = priceCategory;
            Regionalbereich = regionalbereich;
            Ril100Identifiers = ril100Identifiers;
            StationManagement = stationManagement;
            Szentrale = szentrale;
            TimeTableOffice = timeTableOffice;
            WirelessLan = wirelessLan;
        }

        public required Schedule DBinformation { get; set; }
        public required Aufgabentraeger Aufgabentraeger { get; set; }
        public required int Category { get; set; }
        public required string CountryCode { get; set; }
        public required System.Collections.Generic.List<EVANumber> EvaNumbers { get; set; }
        public required string FederalState { get; set; }
        public required string FederalStateCode { get; set; }
        public required bool HasBicycleParking { get; set; }
        public required bool HasCarRental { get; set; }
        public required bool HasDBLounge { get; set; }
        public required bool HasLocalPublicTransport { get; set; }
        public required bool HasLockerSystem { get; set; }
        public required bool HasLostAndFound { get; set; }
        public required string HasMobilityService { get; set; }
        public required bool HasParking { get; set; }
        public required bool HasPublicFacilities { get; set; }
        public required bool HasRailwayMission { get; set; }
        public required Partial HasSteplessAccess { get; set; }
        public required bool HasTaxiRank { get; set; }
        public required bool HasTravelCenter { get; set; }
        public required bool HasTravelNecessities { get; set; }
        public required bool HasWiFi { get; set; }
        public required string Ifopt { get; set; }
        public required Schedule LocalServiceStaff { get; set; }
        public required Address MailingAddress { get; set; }
        public required MobilityServiceStaff MobilityServiceStaff { get; set; }
        public required string MunicipalityCode { get; set; }
        public required string Name { get; set; }
        public required int Number { get; set; }
        public required int PriceCategory { get; set; }
        public required RegionalBereich Regionalbereich { get; set; }
        public required System.Collections.Generic.List<RiL100Identifier> Ril100Identifiers { get; set; }
        public required StationManagement StationManagement { get; set; }
        public required SZentrale Szentrale { get; set; }
        public required TimetableOffice TimeTableOffice { get; set; }
        public required WirelessLan WirelessLan { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("DBinformation", DBinformation);
            yield return ("aufgabentraeger", Aufgabentraeger);
            yield return ("category", Category);
            yield return ("countryCode", CountryCode);
            yield return ("evaNumbers", EvaNumbers);
            yield return ("federalState", FederalState);
            yield return ("federalStateCode", FederalStateCode);
            yield return ("hasBicycleParking", HasBicycleParking);
            yield return ("hasCarRental", HasCarRental);
            yield return ("hasDBLounge", HasDBLounge);
            yield return ("hasLocalPublicTransport", HasLocalPublicTransport);
            yield return ("hasLockerSystem", HasLockerSystem);
            yield return ("hasLostAndFound", HasLostAndFound);
            yield return ("hasMobilityService", HasMobilityService);
            yield return ("hasParking", HasParking);
            yield return ("hasPublicFacilities", HasPublicFacilities);
            yield return ("hasRailwayMission", HasRailwayMission);
            yield return ("hasSteplessAccess", HasSteplessAccess switch
            {
                Partial.yes => "yes",
                Partial.no => "no",
                Partial.partial => "partial",
                _ => null
            });
            yield return ("hasTaxiRank", HasTaxiRank);
            yield return ("hasTravelCenter", HasTravelCenter);
            yield return ("hasTravelNecessities", HasTravelNecessities);
            yield return ("hasWiFi", HasWiFi);
            yield return ("ifopt", Ifopt);
            yield return ("localServiceStaff", LocalServiceStaff);
            yield return ("mailingAddress", MailingAddress);
            yield return ("mobilityServiceStaff", MobilityServiceStaff);
            yield return ("municipalityCode", MunicipalityCode);
            yield return ("name", Name);
            yield return ("number", Number);
            yield return ("priceCategory", PriceCategory);
            yield return ("regionalbereich", Regionalbereich);
            yield return ("ril100Identifiers", Ril100Identifiers);
            yield return ("stationManagement", StationManagement);
            yield return ("szentrale", Szentrale);
            yield return ("timeTableOffice", TimeTableOffice);
            yield return ("wirelessLan", WirelessLan);
        }
    }


    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StationManagement : __ICanIterate
    {
        public StationManagement() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StationManagement(string name, int number)
        {
            Name = name;
            Number = number;
        }

        public required string Name { get; set; }
        public required int Number { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("name", Name);
            yield return ("number", Number);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StationQuery : __ICanIterate
    {
        public StationQuery() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StationQuery(long limit, long offset, System.Collections.Generic.List<Station> result, long total)
        {
            Limit = limit;
            Offset = offset;
            Result = result;
            Total = total;
        }

        public required long Limit { get; set; }
        public required long Offset { get; set; }
        public required System.Collections.Generic.List<Station> Result { get; set; }
        public required long Total { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("limit", Limit);
            yield return ("offset", Offset);
            yield return ("result", Result);
            yield return ("total", Total);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class SZentrale : __ICanIterate
    {
        public SZentrale() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public SZentrale(Address address, string email, string internalFaxNumber, string internalPhoneNumber, string mobilePhoneNumber, string name, int number, string publicFaxNumber, string publicPhoneNumber)
        {
            Address = address;
            Email = email;
            InternalFaxNumber = internalFaxNumber;
            InternalPhoneNumber = internalPhoneNumber;
            MobilePhoneNumber = mobilePhoneNumber;
            Name = name;
            Number = number;
            PublicFaxNumber = publicFaxNumber;
            PublicPhoneNumber = publicPhoneNumber;
        }

        public required Address Address { get; set; }
        public required string Email { get; set; }
        public required string InternalFaxNumber { get; set; }
        public required string InternalPhoneNumber { get; set; }
        public required string MobilePhoneNumber { get; set; }
        public required string Name { get; set; }
        public required int Number { get; set; }
        public required string PublicFaxNumber { get; set; }
        public required string PublicPhoneNumber { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("address", Address);
            yield return ("email", Email);
            yield return ("internalFaxNumber", InternalFaxNumber);
            yield return ("internalPhoneNumber", InternalPhoneNumber);
            yield return ("mobilePhoneNumber", MobilePhoneNumber);
            yield return ("name", Name);
            yield return ("number", Number);
            yield return ("publicFaxNumber", PublicFaxNumber);
            yield return ("publicPhoneNumber", PublicPhoneNumber);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class SZentraleQuery : __ICanIterate
    {
        public SZentraleQuery() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public SZentraleQuery(long limit, long offset, System.Collections.Generic.List<SZentrale> result, long total)
        {
            Limit = limit;
            Offset = offset;
            Result = result;
            Total = total;
        }

        public required long Limit { get; set; }
        public required long Offset { get; set; }
        public required System.Collections.Generic.List<SZentrale> Result { get; set; }
        public required long Total { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("limit", Limit);
            yield return ("offset", Offset);
            yield return ("result", Result);
            yield return ("total", Total);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class TimetableOffice : __ICanIterate
    {
        public TimetableOffice() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public TimetableOffice(string email, string name)
        {
            Email = email;
            Name = name;
        }

        public required string Email { get; set; }
        public required string Name { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("email", Email);
            yield return ("name", Name);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class WirelessLan : __ICanIterate
    {
        public WirelessLan() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public WirelessLan(long amount, string installDate, string product)
        {
            Amount = amount;
            InstallDate = installDate;
            Product = product;
        }

        public required long Amount { get; set; }
        public required string InstallDate { get; set; }
        public required string Product { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("amount", Amount);
            yield return ("installDate", InstallDate);
            yield return ("product", Product);
        }
    }
}
