using System.Text.Json;
using System.Text.Json.Serialization;

namespace Navigator.Data.Models.Ris;

public class RisStations
{

    /// <summary>
    /// Accessibility [Barrierefreiheit] information for a particular platform.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Accessibility
    {

        [System.Text.Json.Serialization.JsonPropertyName("audibleSignalsAvailable")]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<AccessibilityStatus>))]
        public AccessibilityStatus AudibleSignalsAvailable { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("automaticDoor")]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<AccessibilityStatus>))]
        public AccessibilityStatus AutomaticDoor { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("boardingAid")]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<AccessibilityStatus>))]
        public AccessibilityStatus BoardingAid { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("passengerInformationDisplay")]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<AccessibilityStatus>))]
        public AccessibilityStatus PassengerInformationDisplay { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("platformSign")]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<AccessibilityStatus>))]
        public AccessibilityStatus PlatformSign { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("stairsMarking")]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<AccessibilityStatus>))]
        public AccessibilityStatus StairsMarking { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("standardPlatformHeight")]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<AccessibilityStatus>))]
        public AccessibilityStatus StandardPlatformHeight { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("stepFreeAccess")]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<AccessibilityStatus>))]
        public AccessibilityStatus StepFreeAccess { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("tactileGuidingStrips")]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<AccessibilityStatus>))]
        public AccessibilityStatus TactileGuidingStrips { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("tactileHandrailLabel")]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<AccessibilityStatus>))]
        public AccessibilityStatus TactileHandrailLabel { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("tactilePlatformAccess")]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<AccessibilityStatus>))]
        public AccessibilityStatus TactilePlatformAccess { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Status of platform accessibility [Barrierefreiheit] information.
    /// <br/>- AVAILABLE (accessibility item is available)
    /// <br/>- NOT_AVAILABLE (accessibility item is not available)
    /// <br/>- PARTIAL (accessibility item is only partial available, for instance available for 12a but not for 12b and therefore not for 12 in total)
    /// <br/>- NOT_APPLICABLE (accessibility item is not applicable because it depends on availability of other items, for instance stair mark depends on step free access)
    /// <br/>- UNKNOWN (no information on availability for accessibility item)
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum AccessibilityStatus
    {

        [System.Runtime.Serialization.EnumMember(Value = @"AVAILABLE")]
        AVAILABLE = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"NOT_AVAILABLE")]
        NOT_AVAILABLE = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"PARTIAL")]
        PARTIAL = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"NOT_APPLICABLE")]
        NOT_APPLICABLE = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"UNKNOWN")]
        UNKNOWN = 4,

    }

    /// <summary>
    /// Address information with www info.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class AddressWithWeb
    {

        /// <summary>
        /// Additional information [Addresszusatz] for this address, like for instance 'Hinterm Haus links'.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("additionalInformation")]
        public string AdditionalInformation { get; set; }

        /// <summary>
        /// City of address the position should be retrieved for.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("city")]
        public string City { get; set; }

        /// <summary>
        /// Country of address the position should be retrieved for.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("country")]
        public string Country { get; set; }

        /// <summary>
        /// House-number of address the position should be retrieved for.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("houseNumber")]
        public string HouseNumber { get; set; }

        /// <summary>
        /// Postalcode [Postleitzahl] of address the position should be retrieved for.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        /// <summary>
        /// State of address the position should be retrieved for.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("state")]
        public string State { get; set; }

        /// <summary>
        /// Street name of address the position should be retrieved for.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("street")]
        public string Street { get; set; }

        /// <summary>
        /// Web site for address.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("website")]
        public string Website { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Connecting time [Umsteigezeit] from a particular stop place [Haltestelle], platform [Gleis, Bahnsteig, Plattform] and optional sector [Gleisabschnitt, Steigabschnitt] to a particular station, platform and optional sector.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class ConnectingTime
    {

        /// <summary>
        /// Eva number of stop place [Haltestelle] to connect from.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("fromEvaNumber")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string FromEvaNumber { get; set; }

        /// <summary>
        /// Platform [Gleis, Bahnsteig, Plattform] of stop place to connect from.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("fromPlatform")]
        public string FromPlatform { get; set; }

        /// <summary>
        /// Sector [Gleisabschnitt, Steigabschnitt] of stop place to connect from.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("fromSector")]
        public string FromSector { get; set; }

        /// <summary>
        /// Indicates whether connection takes place on the same physical platform [Bahnsteig] (platform '12' and '13' belong to physical platform '12/13' for instance).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("identicalPhysicalPlatform")]
        public bool IdenticalPhysicalPlatform { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("source")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<ConnectingTimeSource>))]
        public ConnectingTimeSource Source { get; set; }

        /// <summary>
        /// Connecting times fo different personae.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("times")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<ConnectionTime> Times { get; set; } = new System.Collections.ObjectModel.Collection<ConnectionTime>();

        /// <summary>
        /// Eva number stop place to connect to.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("toEvaNumber")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string ToEvaNumber { get; set; }

        /// <summary>
        /// Platform [Gleis, Bahnsteig, Plattform] of stop place [Haltestelle] to connect to.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("toPlatform")]
        public string ToPlatform { get; set; }

        /// <summary>
        /// Sector [Gleisabschnitt, Steigabschnitt] of stop place [Haltestelle] to connect to.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("toSector")]
        public string ToSector { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Possible groups to consider when returning connecting-times for particular stop-place.
    /// <br/>- STATION (return connecting-times for stop-place and all members of the same station [Bahnhof]
    /// <br/>- SALES (return connecting-times for stop-place and all members of the sales group [EFZ / Vertrieb inkl. ÖPNV]
    /// <br/>- ALL (return connecting-times for stop-place and all members of all groups the stop-place belongs to [all we have]
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum ConnectingTimeGroup
    {

        [System.Runtime.Serialization.EnumMember(Value = @"STATION")]
        STATION = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"SALES")]
        SALES = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"ALL")]
        ALL = 2,

    }

    /// <summary>
    /// Enumerates all possible sources for connecting times [Umsteigezeiten].
    /// <br/>- RIL420 (connecting time is based on DB guideline RIL420)
    /// <br/>- EFZ (connecting time is based on EFZ = Europäisches Fahrplanzentrum)
    /// <br/>- INDOOR_ROUTING (connecting time is based on real indoor routing information from ris-maps system)
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum ConnectingTimeSource
    {

        [System.Runtime.Serialization.EnumMember(Value = @"RIL420")]
        RIL420 = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"EFZ")]
        EFZ = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"INDOOR_ROUTING")]
        INDOOR_ROUTING = 2,

    }

    /// <summary>
    /// List of connecting times [Umsteigezeiten] for requested list of stop-places [Haltestellen].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class ConnectingTimesBatch
    {

        /// <summary>
        /// List of connecting times.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("connectingTimesList")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<ConnectingTime> ConnectingTimesList { get; set; } = new System.Collections.ObjectModel.Collection<ConnectingTime>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Connecting time [Umsteigezeit] for a particular combination of stop-places, platforms and sectors.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class ConnectingTimesSingle
    {

        [System.Text.Json.Serialization.JsonPropertyName("connectingTime")]
        public ConnectingTime ConnectingTime { get; set; }

        /// <summary>
        /// Fallback times for different personae in case no information is available.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("fallbackTimes")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<ConnectionTimeFallback> FallbackTimes { get; set; } = new System.Collections.ObjectModel.Collection<ConnectionTimeFallback>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Connection time [Anschlusszeit] for persona.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class ConnectionTime
    {

        /// <summary>
        /// Distance in meters.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("distance")]
        public double Distance { get; set; }

        /// <summary>
        /// Duration of connect in ISO8601 (for instance 'P3Y6M4DT12H30M17S').
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("duration")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.TimeSpan Duration { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("persona")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<PersonaType>))]
        public PersonaType Persona { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Fallback connection time [Anschlusszeit] for persona in case no information on stop-places and or plattform is available.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class ConnectionTimeFallback
    {

        /// <summary>
        /// Duration of connect in ISO8601 (for instance 'P3Y6M4DT12H30M17S').
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("duration")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.TimeSpan Duration { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("persona")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<PersonaType>))]
        public PersonaType Persona { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("source")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<ConnectingTimeSource>))]
        public ConnectingTimeSource Source { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Contact details for public use.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Contact
    {

        /// <summary>
        /// Name of a person.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("contactPerson")]
        public string ContactPerson { get; set; }

        /// <summary>
        /// EMail address in iso-format.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// further details of contact.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("furtherDetails")]
        public string FurtherDetails { get; set; }

        /// <summary>
        /// phone number of contact.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("phoneNumbers")]
        public System.Collections.Generic.ICollection<PhoneNumber> PhoneNumbers { get; set; }

        /// <summary>
        /// contact url.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("url")]
        public string Url { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// 2D coordinate within geo reference system.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Coordinate2D
    {

        /// <summary>
        /// Latitude position in reference system.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude position in reference system.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Locker [Schließfächer] equipments of a station [Bahnhof] search result. Take care that one particular equipment id may result in multiple lockers.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class EquipmentLocker
    {

        /// <summary>
        /// Unique id of a locker equipment.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("equipmentID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string EquipmentID { get; set; }

        /// <summary>
        /// Lockers that are provided for this equipment id. (explanation: one locker rack may have one equipment id but may contain multiple lockers.)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("lockers")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<Locker> Lockers { get; set; } = new System.Collections.ObjectModel.Collection<Locker>();

        /// <summary>
        /// Unique id of station [Bahnhof], usually the STADA for DB InfraGO Pbf owned stations.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("stationID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string StationID { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Enumerates all identifiers a locker [Schließfach] equipment of a particular station [Bahnhof] can be mapped into or mapped from.
    /// <br/>- EVA (eva number of stop-place [Haltestelle])
    /// <br/>- STATION_ID (id of the station [Bahnhof], usually the STADA for DB InfraGO Pbf owned stations)
    /// <br/>
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum EquipmentLockerKeyType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"EVA")]
        EVA = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"STATION_ID")]
        STATION_ID = 1,

    }

    /// <summary>
    /// Locker [Schließfächer] equipments of a station [Bahnhof] search result.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class EquipmentLockers
    {

        /// <summary>
        /// List of lockers [Schließfach] that matched the search result.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("lockerList")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<EquipmentLocker> LockerList { get; set; } = new System.Collections.ObjectModel.Collection<EquipmentLocker>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Detailed error information on field level.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class ErrorDetail
    {

        /// <summary>
        /// Detailed information for error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("detail")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Detail { get; set; }

        /// <summary>
        /// Unique code that identifies error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Name of field / element that raised the error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("field")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Field { get; set; }

        /// <summary>
        /// Common description of error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("title")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Title { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// API error object according to RFC7807.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class ErrorResponse
    {

        /// <summary>
        /// Detailed information for error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("detail")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Detail { get; set; }

        /// <summary>
        /// Unique code that identifies error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// List of detailed errors in case multiple errors have lead to the surrounding error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("errors")]
        public System.Collections.Generic.ICollection<ErrorDetail> Errors { get; set; }

        /// <summary>
        /// Unique identifier for instance that raised the error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("instanceId")]
        public string InstanceId { get; set; }

        /// <summary>
        /// Http status for error origin.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Common description of error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("title")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Title { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Escalator [Fahrtreppe / Rolltreppe] equipment of a particular station [Bahnhof] or platform [Gleis / Bahnsteig].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Escalator
    {

        /// <summary>
        /// Platforms [Bahnsteige / Gleise] that are associated to this equipment.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("associatedPlatforms")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<EscalatorAndLiftPlatforms> AssociatedPlatforms { get; set; } = new System.Collections.ObjectModel.Collection<EscalatorAndLiftPlatforms>();

        /// <summary>
        /// Description of equipment.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The name of the operator of the equipment.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("operatorName")]
        public string OperatorName { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("position")]
        public Coordinate2D Position { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("state")]
        [System.ComponentModel.DataAnnotations.Required]
        public State State { get; set; } = new State();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Associated platforms [Bahnsteige / Gleise] for escalators and lifts.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class EscalatorAndLiftPlatforms
    {

        /// <summary>
        /// Unique ids of stop-place [Haltestelle] associated with the escalators / lift.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("evaNumber")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string EvaNumber { get; set; }

        /// <summary>
        /// List of platforms [Bahnsteige / Gleise] associated with the escalators / lift.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("platforms")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Platforms { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Lift [Aufzug] equipment of a particular station [Bahnhof] or platform [Gleis / Bahnsteig].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Lift
    {

        /// <summary>
        /// Platforms [Bahnsteige / Gleise] that are associated to this equipment.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("associatedPlatforms")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<EscalatorAndLiftPlatforms> AssociatedPlatforms { get; set; } = new System.Collections.ObjectModel.Collection<EscalatorAndLiftPlatforms>();

        /// <summary>
        /// Description of equipment.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The name of the operator of the equipment.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("operatorName")]
        public string OperatorName { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("position")]
        public Coordinate2D Position { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("state")]
        [System.ComponentModel.DataAnnotations.Required]
        public State State { get; set; } = new State();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Information on local service [Bahnhofsnahe Dienstleistungen].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class LocalService
    {

        [System.Text.Json.Serialization.JsonPropertyName("address")]
        public AddressWithWeb Address { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("contact")]
        public Contact Contact { get; set; }

        /// <summary>
        /// Description of the local service.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// ID of the local service for external usage.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("externalID")]
        public string ExternalID { get; set; }

        /// <summary>
        /// Unique id of the local service [Bahnhofsnahe Dienstleistungen].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("localServiceID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string LocalServiceID { get; set; }

        /// <summary>
        /// Name of the local service.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Opening times for local-service in OSM notation (see https://wiki.openstreetmap.org/wiki/DE:Key:opening_hours).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("openingHours")]
        public string OpeningHours { get; set; }

        /// <summary>
        /// Collection of available payment methods. Possible values are:
        /// <br/>- CASH
        /// <br/>- GIROGO
        /// <br/>- MASTERCARD
        /// <br/>- VISA
        /// <br/>- EC
        /// <br/>- AMEX
        /// <br/>- GOOGLE_PAY
        /// <br/>- APPLE_PAY
        /// <br/>
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("paymentMethods")]
        public System.Collections.Generic.ICollection<string> PaymentMethods { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("position")]
        public Coordinate2D Position { get; set; }

        /// <summary>
        /// Unique id of station [Bahnhof], usually the STADA for DB InfraGO Pbf owned stations. This can be an empty string if the local service cannot be linked to a station.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("stationID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string StationID { get; set; }

        /// <summary>
        /// Types of local service. Possible values are:
        /// <br/>- INFORMATION_COUNTER [Informationsstand für Belange im Bahnhof (kein Fahrkartenverkauf)]
        /// <br/>- TRAVEL_CENTER [Reisezentrum]
        /// <br/>- VIDEO_TRAVEL_CENTER [Video Reisezentrum]
        /// <br/>- TRIPLE_S_CENTER [3S Zentrale für Service, Sicherheit &amp; Sauberkeit]
        /// <br/>- TRAVEL_LOUNGE [Lounge (DB Lounge z.B.)]
        /// <br/>- LOST_PROPERTY_OFFICE [Fundstelle]
        /// <br/>- RAILWAY_MISSION [Bahnhofsmission]
        /// <br/>- HANDICAPPED_TRAVELLER_SERVICE [Service für mobilitätseingeschränkte Reisende]
        /// <br/>- LOCKER [Schließfächer]
        /// <br/>- WIFI [WLan]
        /// <br/>- CAR_PARKING [Autoparkplatz, ggf. kostenpflichtig]
        /// <br/>- BICYCLE_PARKING [Fahrradparkplätze, ggf. kostenpflichtig]
        /// <br/>- PUBLIC_RESTROOM [Öffentliches WC, ggf. kostenpflichtig]
        /// <br/>- TRAVEL_NECESSITIES [Geschäft für den Reisendenbedarf]
        /// <br/>- CAR_RENTAL [Car-Sharer oder Mietwagen]
        /// <br/>- BICYCLE_RENTAL [Mieträder]
        /// <br/>- TAXI_RANK [Taxi Stand]
        /// <br/>- MOBILE_TRAVEL_SERVICE [Mobiler Service]
        /// <br/>- RAD_PLUS (Rad+ Gebiet)
        /// <br/>                     - MOBILITY_HUB (Mobility Hub)
        /// <br/>
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        /// <summary>
        /// Date the local service is valid from.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("validFrom")]
        public System.DateTimeOffset ValidFrom { get; set; }

        /// <summary>
        /// Date the local service is valid to.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("validTo")]
        public System.DateTimeOffset ValidTo { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Enumerates all identifiers a travel-center [Reisezentrum] can be mapped into or mapped from.
    /// <br/>- EVA (eva number of stop-place [Haltestelle])
    /// <br/>- STATION_ID (id of the station [Bahnhof], usually the STADA for DB InfraGO Pbf owned stations)
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum LocalServiceKeyType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"EVA")]
        EVA = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"STATION_ID")]
        STATION_ID = 1,

    }

    /// <summary>
    /// List of local services [Bahnhofsnahe Dienstleistungen].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class LocalServices
    {

        /// <summary>
        /// List of local services.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("localServices")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<LocalService> LocalServices1 { get; set; } = new System.Collections.ObjectModel.Collection<LocalService>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Cursoring local-services [Bahnhofsnahe Dienstleistungen] search result.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class LocalServicesCursoring
    {

        /// <summary>
        /// Value for the next page.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("after")]
        public string After { get; set; }

        /// <summary>
        /// Value for the previous page.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("before")]
        public string Before { get; set; }

        /// <summary>
        /// List of local-services.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("localServices")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<LocalService> LocalServices { get; set; } = new System.Collections.ObjectModel.Collection<LocalService>();

        /// <summary>
        /// Total number of items.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("total")]
        public int Total { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Locale to use, defaults to DE.
    /// <br/>- CS (Czech)
    /// <br/>- DA (Danish)
    /// <br/>- DE (German)
    /// <br/>- EN (English)
    /// <br/>- ES (Spanish)
    /// <br/>- FR (French)
    /// <br/>- IT (Italian)
    /// <br/>- NL (Dutch)
    /// <br/>- PL (Polish)
    /// <br/>
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum Locale
    {

        [System.Runtime.Serialization.EnumMember(Value = @"CS")]
        CS = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"DA")]
        DA = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"DE")]
        DE = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"EN")]
        EN = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"ES")]
        ES = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"FR")]
        FR = 5,

        [System.Runtime.Serialization.EnumMember(Value = @"IT")]
        IT = 6,

        [System.Runtime.Serialization.EnumMember(Value = @"NL")]
        NL = 7,

        [System.Runtime.Serialization.EnumMember(Value = @"PL")]
        PL = 8,

    }

    /// <summary>
    /// Locker [Schließfach] equipment of a particular station [Bahnhof].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Locker
    {

        /// <summary>
        /// Amount of units of the locker.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("amount")]
        public int Amount { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("dimension")]
        public LockerDimension Dimension { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("fee")]
        public LockerFee Fee { get; set; }

        /// <summary>
        /// Maximum lease duration [Mietdauer] for use of a single unit of the locker in ISO8601 (for instance 'P3Y6M4DT12H30M17S').
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("maxLeaseDuration")]
        public System.TimeSpan MaxLeaseDuration { get; set; }

        /// <summary>
        /// Supported payment types for locker usage. Possible values are:
        /// <br/>- CASH (cash payment available)
        /// <br/>- CASHLESS (other payment options than cash available)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("paymentTypes")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> PaymentTypes { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>
        /// Size of the locker. Possible values are:
        /// <br/>- SMALL
        /// <br/>- MEDIUM
        /// <br/>- LARGE
        /// <br/>- JUMBO
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("size")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Size { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Dimension [Ausmaße] for lockers [Schließfächer].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class LockerDimension
    {

        /// <summary>
        /// Locker depth of a single unit in mm of the locker.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("depth")]
        public int Depth { get; set; }

        /// <summary>
        /// Locker height of a single unit in mm of the locker.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("height")]
        public int Height { get; set; }

        /// <summary>
        /// Locker width of a single unit in mm of the locker.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("width")]
        public int Width { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Fee [Mietgebühr] for leasing lockers [Schließfächer].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class LockerFee
    {

        /// <summary>
        /// Fee for locker usage in cents (currency is EUR). Refers to fee period.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("fee")]
        public int Fee { get; set; }

        /// <summary>
        /// Period the refers to. Possible values are:
        /// <br/>- PER_MAX_LEASE_DURATION (fee must be payed per max lease duration)
        /// <br/>- PER_HOUR (fee must be payed per hour)
        /// <br/>- PER_DAY (fee must be payed per day)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("feePeriod")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string FeePeriod { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Locker [Schließfächer] equipments of a station [Bahnhof] search result. Take care that one particular equipment id may result in multiple lockers.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class LockerRack
    {

        /// <summary>
        /// Lockers that are provided for this equipment id. (explanation: one locker rack may have one equipment id but may contain multiple lockers.)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("lockers")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<Locker> Lockers { get; set; } = new System.Collections.ObjectModel.Collection<Locker>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Detailed operational [Betrieb] information.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Operational
    {

        /// <summary>
        /// Names of the network platforms [Netzgleis] that belong to the platform.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("networkPlatforms")]
        public System.Collections.Generic.ICollection<string> NetworkPlatforms { get; set; }

        /// <summary>
        /// Names of the operational units [Optiken] that belong to the platform.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("optics")]
        public System.Collections.Generic.ICollection<string> Optics { get; set; }

        /// <summary>
        /// Orientation of the platform in degrees (north=0, east=90, ...), seen from the origin of the local coordinates.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("orientation")]
        public double Orientation { get; set; }

        /// <summary>
        /// Positions of the reference points that determine the position of a stopping train at the platform.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("referencePoints")]
        public System.Collections.Generic.ICollection<ReferencePoint> ReferencePoints { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Specifies different personae.
    /// <br/>- HANDICAPPED (Handicaped [MER] slow traveller, not able to use stairs and escalators)
    /// <br/>- OCCASIONAL_TRAVELLER (Occasional traveller [Gelegenheits-Reisender / Standard-Reisender] having mean walking speed. This is the default traveller.)
    /// <br/>- FREQUENT_TRAVELLER (Frequent traveller [Pendler] having higher speed than occasional traveller.)
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum PersonaType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"HANDICAPPED")]
        HANDICAPPED = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"OCCASIONAL_TRAVELLER")]
        OCCASIONAL_TRAVELLER = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"FREQUENT_TRAVELLER")]
        FREQUENT_TRAVELLER = 2,

    }

    /// <summary>
    /// a phone number.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class PhoneNumber
    {

        /// <summary>
        /// phone number of contact.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("number")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Number { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<PhoneNumberType>))]
        public PhoneNumberType Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Enumerates all phone types.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum PhoneNumberType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"BUSINESS")]
        BUSINESS = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"MOBILE")]
        MOBILE = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"FAX")]
        FAX = 2,

    }

    /// <summary>
    /// Platform [Gleis, Bahnsteig, Plattform] information. All ranges and positions of objects are given in meter in local coordinates, e.g. as a distance to a fixed point somewhere on the platform and differentiating between the two possible directions by a plus- and a minus-sign.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Platform
    {

        [System.Text.Json.Serialization.JsonPropertyName("accessibility")]
        public Accessibility Accessibility { get; set; }

        /// <summary>
        /// End of the usable part of the platform given in meter in local coordinates.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("end")]
        public double End { get; set; }

        /// <summary>
        /// Indicates whether platform is a head platform [Kopfgleis].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("headPlatform")]
        public bool HeadPlatform { get; set; }

        /// <summary>
        /// List of platform heights [Bahnsteighöhen]. Please note that currently only one platform height, with start and end information from platform, is supported due to missing source systems that are able to deliver this information in reasonable quality.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("heights")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<PlatformHeight> Heights { get; set; } = new System.Collections.ObjectModel.Collection<PlatformHeight>();

        /// <summary>
        /// IFOPT (transmodel identifier for fixed objects, in germany DHID = Deutschlandweite Halte ID also known as global id) of the platform [Gleis] (for instance 'de:06412:10:17:18').
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("ifopt")]
        public string Ifopt { get; set; }

        /// <summary>
        /// Total length of platform [Baulicher Bereich].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("length")]
        public double Length { get; set; }

        /// <summary>
        /// List of platforms [Gleise] that share the same physical platform [Bahnsteig].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("linkedPlatforms")]
        public System.Collections.Generic.ICollection<string> LinkedPlatforms { get; set; }

        /// <summary>
        /// Name of the platform (12, 1a, Nord, Süd etc.).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("operational")]
        public Operational Operational { get; set; }

        /// <summary>
        /// Name of parent platform in case this is a sub platform [Teilgleis].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("parentPlatform")]
        public string ParentPlatform { get; set; }

        /// <summary>
        /// List of sectors [Sektoren] that belong to the platform.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("sectors")]
        public System.Collections.Generic.ICollection<Sector> Sectors { get; set; }

        /// <summary>
        /// Start of the usable part of the platform given in meter in local coordinates. Value is &gt;= 0 which means that we asume that the zero-point [Nullpunkt] is always at 0m.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("start")]
        public double Start { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Platform [Gleise, Bahnsteige, Plattformen] height information that may split a particular platform in mutiple chunks, defined by start and end, that may have differing heights.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class PlatformHeight
    {

        /// <summary>
        /// End of the platform height information given in meter in local coordinates.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("end")]
        public double End { get; set; }

        /// <summary>
        /// Height of the platform in cm.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("height")]
        public double Height { get; set; }

        /// <summary>
        /// Start of the platform height information given in meter in local coordinates.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("start")]
        public double Start { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// List of platforms [Gleise, Bahnsteige, Plattformen] for a station.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Platforms
    {

        /// <summary>
        /// List of platforms.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("platforms")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<Platform> Platforms1 { get; set; } = new System.Collections.ObjectModel.Collection<Platform>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Reference point that indicates where a vehicle [Fahrzeug] stops at a platform [Gleis, Bahnsteig, Plattform].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class ReferencePoint
    {

        /// <summary>
        /// Length up to the reference point is to be used by a stopping formation.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("maxLength")]
        public double MaxLength { get; set; }

        /// <summary>
        /// Name of the reference point.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>
        /// Position of the reference point in meter in local coordinates.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("position")]
        public double Position { get; set; }

        /// <summary>
        /// Determines the direction the reference point is to be used. If true, the formation moves from origin to positive values in local coordinates.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("readableFromOrigin")]
        public bool ReadableFromOrigin { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("referencePointType")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<ReferencePointType>))]
        public ReferencePointType ReferencePointType { get; set; }

        /// <summary>
        /// Unique ID of reference point.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("uuid")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Uuid { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Type of a reference point.
    /// <br/>- STOP_SIGNAL (Haltesignal)
    /// <br/>- STOP_BOARD (Haltetafel)
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum ReferencePointType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"STOP_SIGNAL")]
        STOP_SIGNAL = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"STOP_BOARD")]
        STOP_BOARD = 1,

    }

    /// <summary>
    /// Base information for replacement transport stop [EV-Halt].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class ReplacementTransportStop
    {

        /// <summary>
        /// Language dependent names for replacement transport stop, may contain different stop place names for a specific language depending on names filter.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("names")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.IDictionary<string, StopPlaceName> Names { get; set; } = new System.Collections.Generic.Dictionary<string, StopPlaceName>();

        [System.Text.Json.Serialization.JsonPropertyName("position")]
        [System.ComponentModel.DataAnnotations.Required]
        public Coordinate2D Position { get; set; } = new Coordinate2D();

        /// <summary>
        /// ID of replacement transport stop [EV-Halt] belongs to [usually the DHID in Germany]
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("replacementTransportStopID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string ReplacementTransportStopID { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Replacement transport stops [EV-Halte] search result.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class ReplacementTransportStopResult
    {

        /// <summary>
        /// Replacement transport stops [EV-Halte] matching provided search criterias.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("replacementTransportStops")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<ReplacementTransportStop> ReplacementTransportStops { get; set; } = new System.Collections.ObjectModel.Collection<ReplacementTransportStop>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Platform [Gleis, Bahnsteig, Plattform] sector [Gleisabschnitt, Steigabschnitt] information.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Sector
    {

        /// <summary>
        /// Position of the cube [Sektorwürfel] given in meters in local coordinates.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("cubePosition")]
        public double CubePosition { get; set; }

        /// <summary>
        /// Indicates whether cube [Sektorwürfel] has signage [Beschilderung] nor not.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("cubeSignage")]
        public bool CubeSignage { get; set; }

        /// <summary>
        /// End of the sector given in meters in local coordinates.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("end")]
        public double End { get; set; }

        /// <summary>
        /// Name of the sector [Sektor / Mast etc.].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>
        /// Start of the sector given in meters in local coordinates.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("start")]
        public double Start { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Operational state of the station equipment.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class State
    {

        /// <summary>
        /// Detailed explanation for operational state, may contain further information like construction hints.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("explanation")]
        public string Explanation { get; set; }

        /// <summary>
        /// Date ('YYYY-MM-dd') of recommissioning [Wiederinbetriebnahme-Datum]
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("recommissioningDate")]
        [System.Text.Json.Serialization.JsonConverter(typeof(DateFormatConverter))]
        public System.DateTimeOffset RecommissioningDate { get; set; }

        /// <summary>
        /// Operational state of the station equipment. Possible values are: 
        /// <br/> - ACTIVE
        /// <br/>- INACTIVE
        /// <br/>- UNKNOWN
        /// <br/>
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Base information for a station [Bahnhof].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Station
    {

        [System.Text.Json.Serialization.JsonPropertyName("address")]
        [System.ComponentModel.DataAnnotations.Required]
        public AddressWithWeb Address { get; set; } = new AddressWithWeb();

        /// <summary>
        /// Deprecated: Use for example '/local-services/by-key?keyType=STATION_ID&amp;key=1866' to get all available local-services for Frankfurt Hbf.
        /// <br/>All local services [Bahnhofsnahe Dienstleistungen] that are available at the station.
        /// <br/>- INFORMATION_COUNTER [Informationsstand für Belange im Bahnhof (kein Fahrkartenverkauf)]
        /// <br/>- TRAVEL_CENTER [Reisezentrum]
        /// <br/>- VIDEO_TRAVEL_CENTER [Video Reisezentrum]
        /// <br/>- TRIPLE_S_CENTER [3S Zentrale für Service, Sicherheit &amp; Sauberkeit]
        /// <br/>- TRAVEL_LOUNGE [Lounge (DB Lounge z.B.)]
        /// <br/>- LOST_PROPERTY_OFFICE [Fundstelle]
        /// <br/>- RAILWAY_MISSION [Bahnhofsmission]
        /// <br/>- HANDICAPPED_TRAVELLER_SERVICE [Service für mobilitätseingeschränkte Reisende]
        /// <br/>- LOCKER [Schließfächer]
        /// <br/>- WIFI [WLan]
        /// <br/>- CAR_PARKING [Autoparkplatz, ggf. kostenpflichtig]
        /// <br/>- BICYCLE_PARKING [Fahrradparkplätze, ggf. kostenpflichtig]
        /// <br/>- PUBLIC_RESTROOM [Öffentliches WC, ggf. kostenpflichtig]
        /// <br/>- TRAVEL_NECESSITIES [Geschäft für den Reisendenbedarf]
        /// <br/>- CAR_RENTAL [Car-Sharer oder Mietwagen]
        /// <br/>- BICYCLE_RENTAL [Mieträder]
        /// <br/>- TAXI_RANK [Taxi Stand]
        /// <br/>- MOBILE_TRAVEL_SERVICE [Mobiler Service]
        /// <br/>- RAD_PLUS (Rad+ Gebiet)
        /// <br/>                     - MOBILITY_HUB (Mobility Hub)
        /// <br/>
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("availableLocalServices")]
        [System.ComponentModel.DataAnnotations.Required]
        [System.Obsolete]
        public System.Collections.Generic.ICollection<string> AvailableLocalServices { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>
        /// Available transport types [Verkehrsarten] at station.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("availableTransports")]
        // TODO(system.text.json): Add ItemConverterType with enum converter when supported
        [System.ComponentModel.DataAnnotations.Required]
        [System.Obsolete]
        public System.Collections.Generic.ICollection<TransportType> AvailableTransports { get; set; } = new System.Collections.ObjectModel.Collection<TransportType>();

        /// <summary>
        /// Country [Staat / Land] the station belongs to as ISO 3166-1 alpha-2 code [germany = 'DE' for instance].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("countryCode")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string CountryCode { get; set; }

        /// <summary>
        /// Language dependent names for metropolis [Metropole].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("metropolis")]
        public System.Collections.Generic.IDictionary<string, string> Metropolis { get; set; }

        /// <summary>
        /// Mobility Service staff on site
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("mobilityServiceStaffOnSite")]
        public bool MobilityServiceStaffOnSite { get; set; }

        /// <summary>
        /// The municipality key [Amtlicher Gemeindeschlüssel (AGS)] the station belongs to. Only available for germany and may be empty.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("municipalityKey")]
        public string MunicipalityKey { get; set; }

        /// <summary>
        /// Language dependent names for a station, may contain different station names for a specific language depending on names filter.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("names")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.IDictionary<string, StationName> Names { get; set; } = new System.Collections.Generic.Dictionary<string, StationName>();

        /// <summary>
        /// Opening times for station in OSM notation (see https://wiki.openstreetmap.org/wiki/DE:Key:opening_hours).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("openingHours")]
        public string OpeningHours { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("owner")]
        [System.ComponentModel.DataAnnotations.Required]
        public StationOwner Owner { get; set; } = new StationOwner();

        [System.Text.Json.Serialization.JsonPropertyName("position")]
        public Coordinate2D Position { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("roofing")]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<StationRoofingType>))]
        public StationRoofingType Roofing { get; set; }

        /// <summary>
        /// The state code [Bundeslandkürzel] the station belongs to.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("state")]
        public string State { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("stationCategory")]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<StationCategory>))]
        public StationCategory StationCategory { get; set; }

        /// <summary>
        /// Unique id of station [Bahnhof], usually the STADA for DB InfraGO Pbf owned stations.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("stationID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string StationID { get; set; }

        /// <summary>
        /// Timezone the station belongs to, for instance 'Europe/Berlin'. Must not necessarily be the time zone of the geo coordinate.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("timeZone")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string TimeZone { get; set; }

        /// <summary>
        /// Available transport associations [Verkehrsverbünde] at station.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("transportAssociations")]
        [System.Obsolete]
        public System.Collections.Generic.ICollection<string> TransportAssociations { get; set; }

        /// <summary>
        /// Date the station is valid from.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("validFrom")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.DateTimeOffset ValidFrom { get; set; }

        /// <summary>
        /// Date the station is valid to.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("validTo")]
        public System.DateTimeOffset ValidTo { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Category of station conforming to DB InfraGO Pbf..
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum StationCategory
    {

        [System.Runtime.Serialization.EnumMember(Value = @"CATEGORY_1")]
        CATEGORY_1 = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"CATEGORY_2")]
        CATEGORY_2 = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"CATEGORY_3")]
        CATEGORY_3 = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"CATEGORY_4")]
        CATEGORY_4 = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"CATEGORY_5")]
        CATEGORY_5 = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"CATEGORY_6")]
        CATEGORY_6 = 5,

        [System.Runtime.Serialization.EnumMember(Value = @"CATEGORY_7")]
        CATEGORY_7 = 6,

    }

    /// <summary>
    /// Station equipment for a particular station [Bahnhof].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StationEquipment
    {

        /// <summary>
        /// Unique id of a station equipment.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("equipmentID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string EquipmentID { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("escalator")]
        public Escalator Escalator { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("lift")]
        public Lift Lift { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("lockerRack")]
        public LockerRack LockerRack { get; set; }

        /// <summary>
        /// Unique id of station [Bahnhof], usually the STADA for DB InfraGO Pbf owned stations.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("stationID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string StationID { get; set; }

        /// <summary>
        /// Type of station equipment. Possible values are: 
        /// <br/> - LOCKER_RACK [Schließfach]
        /// <br/>- LIFT [Aufzug]
        /// <br/>- ESCALATOR [Fahrtreppe / Rolltreppe]
        /// <br/>
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Result of station equipments.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StationEquipments
    {

        /// <summary>
        /// List of station equipments.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("stationEquipments")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<StationEquipment> StationEquipments1 { get; set; } = new System.Collections.ObjectModel.Collection<StationEquipment>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Name information for a station [Bahnhof].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StationName
    {

        /// <summary>
        /// Name for station.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Organisational unit [Regionalbereich] information, usually from DB Netz.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StationOrganisationalUnit
    {

        /// <summary>
        /// Number of organisational unit.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Name of organisational unit.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>
        /// Short name of organisational unit, may be empty.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("nameShort")]
        public string NameShort { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Base information for a stations [Bahnhof] owner [Eigentümer / Betreiber].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StationOwner
    {

        /// <summary>
        /// Name of owner.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("organisationalUnit")]
        public StationOrganisationalUnit OrganisationalUnit { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Defines station roofing [Bahnhofsüberdachung].
    /// <br/>- COVERED [überdacht]
    /// <br/>- PARTIALLY_COVERED [teilweise überdacht]
    /// <br/>- NOT_COVERED [nicht überdacht]
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum StationRoofingType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"COVERED")]
        COVERED = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"PARTIALLY_COVERED")]
        PARTIALLY_COVERED = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"NOT_COVERED")]
        NOT_COVERED = 2,

    }

    /// <summary>
    /// Transport companies [Verkehrsunternehmen] result.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StationTransportCompanies
    {

        /// <summary>
        /// List of transport companies.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("transportCompanies")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<StationTransportCompany> TransportCompanies { get; set; } = new System.Collections.ObjectModel.Collection<StationTransportCompany>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Basic information about a transport company [Verkehrsunternehmen]:
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StationTransportCompany
    {

        /// <summary>
        /// Email of transport company.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// Name of transport company.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>
        /// Phone number of transport company.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Short name of transport company.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("shortName")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string ShortName { get; set; }

        /// <summary>
        /// Website of transport company.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("website")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Website { get; set; }

        /// <summary>
        /// Deep link to website of digital assistant [digitaler Assistent, Chatbot etc.] of transport company.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("websiteDigitalAssistant")]
        public string WebsiteDigitalAssistant { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Stations [Bahnhof] result.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Stations
    {

        /// <summary>
        /// List of stations.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("stations")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<Station> Stations1 { get; set; } = new System.Collections.ObjectModel.Collection<Station>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Pageable stations search result.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StationsPageable
    {

        /// <summary>
        /// Maximum number of results the caller has requested to return from provided offset.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("limit")]
        public int Limit { get; set; }

        /// <summary>
        /// Pagination offset the caller has requested in order to navigate through results.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("offset")]
        public int Offset { get; set; }

        /// <summary>
        /// List of stations.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("stations")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<Station> Stations { get; set; } = new System.Collections.ObjectModel.Collection<Station>();

        /// <summary>
        /// Total number of available results.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("total")]
        public int Total { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Base information for a stop-place [Haltestelle].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlace
    {

        /// <summary>
        /// Available physical transport types [physische Verkehrsarten] at stop place, that may differ in case of replacement transports [Ersatzverkehren] (ie a 'REGIONAL_TRAIN' gets usually replaced by a 'BUS').
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("availablePhysicalTransports")]
        // TODO(system.text.json): Add ItemConverterType with enum converter when supported
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<TransportType> AvailablePhysicalTransports { get; set; } = new System.Collections.ObjectModel.Collection<TransportType>();

        /// <summary>
        /// Available transport types [Verkehrsarten] at stop place, may include replacement transports [Ersatzverkehre].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("availableTransports")]
        // TODO(system.text.json): Add ItemConverterType with enum converter when supported
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<TransportType> AvailableTransports { get; set; } = new System.Collections.ObjectModel.Collection<TransportType>();

        /// <summary>
        /// Country [Staat / Land] the stop place belongs to as ISO 3166-1 alpha-2 code [germany = 'DE' for instance].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("countryCode")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string CountryCode { get; set; }

        /// <summary>
        /// Eva number of stop-place.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("evaNumber")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string EvaNumber { get; set; }

        /// <summary>
        /// Language dependent name for metropolis [Metropole].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("metropolis")]
        public System.Collections.Generic.IDictionary<string, string> Metropolis { get; set; }

        /// <summary>
        /// The municipality key [Amtlicher Gemeindeschlüssel (AGS)] the stop place belongs to. Only available for germany and may be empty.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("municipalityKey")]
        public string MunicipalityKey { get; set; }

        /// <summary>
        /// Language dependent names for stop place, may contain different stop place names for a specific language depending on names filter.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("names")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.IDictionary<string, StopPlaceName> Names { get; set; } = new System.Collections.Generic.Dictionary<string, StopPlaceName>();

        [System.Text.Json.Serialization.JsonPropertyName("position")]
        public Coordinate2D Position { get; set; }

        /// <summary>
        /// Postal code [Postleitzahl] the stop place belongs to.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Indicates whether replacement transports [Ersatzverkehre] are available at this stop place.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("replacementTransportsAvailable")]
        public bool ReplacementTransportsAvailable { get; set; }

        /// <summary>
        /// The state code [Bundeslandkürzel] the stop place belongs to.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("state")]
        public string State { get; set; }

        /// <summary>
        /// ID of station [Bahnhof] the stop place belongs to [usually the STADA code for DB DB InfraGO Pbf], may be empty when stop place is not part of a station.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("stationID")]
        public string StationID { get; set; }

        /// <summary>
        /// Timezone the stop place belongs to, for instance 'Europe/Berlin'. Must not necessarily be the time zone of the geo coordinate.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("timeZone")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string TimeZone { get; set; }

        /// <summary>
        /// Available transport associations [Verkehrsverbünde] at stop place.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("transportAssociations")]
        public System.Collections.Generic.ICollection<string> TransportAssociations { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Group for stop places [Haltestellen] with all group members.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlaceGroup
    {

        /// <summary>
        /// Identifier for group.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("groupIdentifier")]
        public string GroupIdentifier { get; set; }

        /// <summary>
        /// List of stop place ids [Eva-Number] that belong to the group.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("members")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Members { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<StopPlaceGroupType>))]
        public StopPlaceGroupType Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Possible groups [Station] a set of stop places [Haltestellen] may belong to.
    /// <br/>- STATION (group defined by station, for instance FFM = Hoch + Tief, maintained by DB InfraGO Pbf STADA hierarchy)
    /// <br/>- SALES (group defined by sales [Vertrieb], for instance FFM = Hoch + Tief + Bus + Tram + Subway, maintained by DB Vertrieb via EFZ [Europäisches Fahrplanzentrum])
    /// <br/>- METROPOLITAN_AREA (group defined by sales [Vertrieb], for instance Stadtgebiet FFM = all big stations within FFM, maintained by DB Vertrieb via EFZ [Europäisches Fahrplanzentrum])
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum StopPlaceGroupType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"STATION")]
        STATION = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"SALES")]
        SALES = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"METROPOLITAN_AREA")]
        METROPOLITAN_AREA = 2,

    }

    /// <summary>
    /// List of groups a stop place [Haltestelle] belongs to with all associated group members.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlaceGroups
    {

        /// <summary>
        /// List of groups the passed eva numbers belongs to including group members.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("groups")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<StopPlaceGroup> Groups { get; set; } = new System.Collections.ObjectModel.Collection<StopPlaceGroup>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Key mapping for a stop place [Haltestelle].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlaceKey
    {

        /// <summary>
        /// Key value.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("key")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Key { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<StopPlaceKeyType>))]
        public StopPlaceKeyType Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Enumerates all identifiers a stop-place [Haltestelle] can be mapped from.
    /// <br/>- IFOPT (Transmodel identifier for fixed objects, in germany DHID = Deutschlandweite Halt ID also known as global id)
    /// <br/>- EVA (eva number)
    /// <br/>- RL100 (primary or alternative rl100 / ds100)
    /// <br/>- EPA (epa uic number)
    /// <br/>- STADA (Stationsdatenbank number)
    /// <br/>- IBNR (internal station number [interne bahnhofsnummer])
    /// <br/>- EBHF ([Tarifpunktnummer / Einheitliche Bahnhofsdatei Nr.])
    /// <br/>- UIC (international station number)
    /// <br/>- PLC (primary location code)
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum StopPlaceKeyFilter
    {

        [System.Runtime.Serialization.EnumMember(Value = @"IFOPT")]
        IFOPT = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"EVA")]
        EVA = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"RL100")]
        RL100 = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"EPA")]
        EPA = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"STADA")]
        STADA = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"IBNR")]
        IBNR = 5,

        [System.Runtime.Serialization.EnumMember(Value = @"EBHF")]
        EBHF = 6,

        [System.Runtime.Serialization.EnumMember(Value = @"UIC")]
        UIC = 7,

        [System.Runtime.Serialization.EnumMember(Value = @"PLC")]
        PLC = 8,

    }

    /// <summary>
    /// Enumerates all identifiers a stop-place [Haltestelle] can be mapped into or mapped from.
    /// <br/>- IFOPT (Transmodel identifier for fixed objects, in germany DHID = Deutschlandweite Halt ID also known as global id)
    /// <br/>- EVA (eva number)
    /// <br/>- RL100 (primary rl100 / ds100)
    /// <br/>- RL100_ALTERNATIVE (alternative rl100 / ds100)
    /// <br/>- EPA (epa number)
    /// <br/>- STADA (Stationsdatenbank number)
    /// <br/>- IBNR (internal station number [interne bahnhofsnummer])
    /// <br/>- EBHF ([Tarifpunktnummer / Einheitliche Bahnhofsdatei Nr.])
    /// <br/>- UIC (international station number)
    /// <br/>- PLC (primary location code)
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum StopPlaceKeyType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"IFOPT")]
        IFOPT = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"EVA")]
        EVA = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"RL100")]
        RL100 = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"RL100_ALTERNATIVE")]
        RL100_ALTERNATIVE = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"EPA")]
        EPA = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"STADA")]
        STADA = 5,

        [System.Runtime.Serialization.EnumMember(Value = @"IBNR")]
        IBNR = 6,

        [System.Runtime.Serialization.EnumMember(Value = @"EBHF")]
        EBHF = 7,

        [System.Runtime.Serialization.EnumMember(Value = @"UIC")]
        UIC = 8,

        [System.Runtime.Serialization.EnumMember(Value = @"PLC")]
        PLC = 9,

    }

    /// <summary>
    /// Different key mappings a stop place [Haltestelle] may have.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlaceKeys
    {

        /// <summary>
        /// List of stop place keys.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("keys")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<StopPlaceKey> Keys { get; set; } = new System.Collections.ObjectModel.Collection<StopPlaceKey>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Group for stop places [Haltestellen] with all group members.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlaceMultiGroup
    {

        /// <summary>
        /// Identifier for group.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("groupIdentifier")]
        public string GroupIdentifier { get; set; }

        /// <summary>
        /// List of stop place ids that belong to the group.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("members")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Members { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>
        /// Type of stop place group.
        /// <br/>- STATION (group defined by station, for instance FFM = Hoch + Tief, maintained by DB InfraGO Pbf STADA hierarchy)
        /// <br/>- SALES (group defined by sales [Vertrieb], for instance FFM = Hoch + Tief + Bus + Tram + Subway, maintained by DB Fernverkehr via EFZ [Europäisches Fahrplanzentrum])
        /// <br/>- DIRECT_SALES (group defined by sales [Vertrieb], for instance FFM = Hoch + Tief + Bus + Tram + Subway, maintained by DB Fernverkehr via EFZ [Europäisches Fahrplanzentrum])
        /// <br/>- METROPOLITAN_AREA (group defined by sales [Vertrieb], for instance Stadtgebiet FFM = all big stations within FFM, maintained by DB Fernverkehr via EFZ [Europäisches Fahrplanzentrum])
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// List of groups a stop place [Haltestelle] belongs to with all associated group members.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlaceMultiGroups
    {

        /// <summary>
        /// List of groups the passed eva numbers belongs to including group members.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("groups")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<StopPlaceMultiGroup> Groups { get; set; } = new System.Collections.ObjectModel.Collection<StopPlaceMultiGroup>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Name information for stop place [Haltestelle].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlaceName
    {

        /// <summary>
        /// Name that is applicable for local areas, for instance 'Berlin Zoologischer Garten' may become 'B Zoologischer Garten'.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("nameLocal")]
        public string NameLocal { get; set; }

        /// <summary>
        /// Primary full long name for stop place.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("nameLong")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string NameLong { get; set; }

        /// <summary>
        /// Short name (max. 20 characters) for stop place, if available.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("nameShort")]
        public string NameShort { get; set; }

        /// <summary>
        /// Long name speech information for stop place [Haltestelle].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("speechLong")]
        public string SpeechLong { get; set; }

        /// <summary>
        /// Short name speech information for stop place [Haltestelle].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("speechShort")]
        public string SpeechShort { get; set; }

        /// <summary>
        /// Symbol information [UTF-8] for stop place [Haltestelle].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// Synonyms [alternative Namen] for this stop place
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("synonyms")]
        public System.Collections.Generic.ICollection<string> Synonyms { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Different grouping options for stop places name query.
    /// <br/>- STATION (group by parent station that is defined by DB InfraGO Pbf STADA-ID)
    /// <br/>- SALES (group defined by sales [Vertrieb], for instance FFM = Hoch + Tief + Bus + Tram + Subway, maintained by DB Vertrieb via EFZ [Europäisches Fahrplanzentrum])
    /// <br/>- NONE (no grouping is applied, just stop-places are returned)
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum StopPlaceSearchGroupByKey
    {

        [System.Runtime.Serialization.EnumMember(Value = @"STATION")]
        STATION = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"SALES")]
        SALES = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"NONE")]
        NONE = 2,

    }

    /// <summary>
    /// Search result information for a stop place [Haltestelle].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlaceSearchResult
    {

        /// <summary>
        /// Available transport types [Verkehrsarten] at stop place.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("availableTransports")]
        // TODO(system.text.json): Add ItemConverterType with enum converter when supported
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<TransportType> AvailableTransports { get; set; } = new System.Collections.ObjectModel.Collection<TransportType>();

        /// <summary>
        /// Eva number of stop place.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("evaNumber")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string EvaNumber { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("groupMembers")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> GroupMembers { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>
        /// Language dependent names for stop place.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("names")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.IDictionary<string, StopPlaceName> Names { get; set; } = new System.Collections.Generic.Dictionary<string, StopPlaceName>();

        [System.Text.Json.Serialization.JsonPropertyName("position")]
        public Coordinate2D Position { get; set; }

        /// <summary>
        /// Indicates whether replacement transports [Ersatzverkehre] are available at this stop place.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("replacementTransportsAvailable")]
        public bool ReplacementTransportsAvailable { get; set; }

        /// <summary>
        /// ID of station [Bahnhof] the stop place belongs to [usually the STADA code for DB DB InfraGO Pbf], may be empty when stop place is not part of a station.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("stationID")]
        public string StationID { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Stop place [Haltestelle] search result.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlaceSearchResults
    {

        /// <summary>
        /// Stop places matching provided search criteria.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("stopPlaces")]
        public System.Collections.Generic.ICollection<StopPlaceSearchResult> StopPlaces { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Different sorting keys for stop place queries.
    /// <br/>- RELEVANCE (stop places are sorted by relevance descending (central stations etc. first))
    /// <br/>- QUERY_MATCH (stop places are sorted by matching the provided query descending)
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum StopPlaceSortKey
    {

        [System.Runtime.Serialization.EnumMember(Value = @"RELEVANCE")]
        RELEVANCE = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"QUERY_MATCH")]
        QUERY_MATCH = 1,

    }

    /// <summary>
    /// Stop place [Haltestelle] result.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlaces
    {

        /// <summary>
        /// Stop places matching provided criteria.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("stopPlaces")]
        public System.Collections.Generic.ICollection<StopPlace> StopPlaces1 { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Single batch request for stop places [Haltestellen] by keys.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlacesByKeysEntry
    {

        /// <summary>
        /// Key for stop place [Haltestelle].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("key")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Key { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Request for stop places [Haltestellen] by keys.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlacesByKeysRequest
    {

        /// <summary>
        /// Specifies the key-type that is passed in the keys collection. Supported key-types are:
        /// <br/>- EVA (eva number)
        /// <br/>- RL100 (primary or alternative rl100 / ds100)
        /// <br/>- STADA (Stationsdatenbank number)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("keyType")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string KeyType { get; set; }

        /// <summary>
        /// Keys for stop places [Haltestellen]. A maximum of 500 keys is allowed.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("keys")]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MaxLength(500)]
        public System.Collections.Generic.ICollection<StopPlacesByKeysEntry> Keys { get; set; } = new System.Collections.ObjectModel.Collection<StopPlacesByKeysEntry>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Type of transport.
    /// <br/>- HIGH_SPEED_TRAIN (High speed train [Hochgeschwindigkeitszug] like ICE or TGV etc.)
    /// <br/>- INTERCITY_TRAIN (Inter city train [Intercityzug])
    /// <br/>- INTER_REGIONAL_TRAIN (Inter regional train [Interregiozug])
    /// <br/>- REGIONAL_TRAIN (Regional train [Regionalzug])
    /// <br/>- CITY_TRAIN (City train [S-Bahn])
    /// <br/>- SUBWAY (Subway [U-Bahn])
    /// <br/>- TRAM (Tram [Strassenbahn])
    /// <br/>- BUS (Bus [Bus])
    /// <br/>- FERRY (Ferry [Faehre])
    /// <br/>- FLIGHT (Flight [Flugzeug])
    /// <br/>- CAR (Car [Auto])
    /// <br/>- TAXI (Taxi)
    /// <br/>- SHUTTLE (Shuttle [Ruftaxi])
    /// <br/>- BIKE ((E-)Bike [Fahrrad])
    /// <br/>- SCOOTER ((E-)Scooter [Roller])
    /// <br/>- WALK (Walk ([Laufen])
    /// <br/>- UNKNOWN (Unknown)
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    [System.Text.Json.Serialization.JsonConverter(typeof(TolerantTransportTypeConverter))]
    public enum TransportType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"HIGH_SPEED_TRAIN")]
        HIGH_SPEED_TRAIN = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"INTERCITY_TRAIN")]
        INTERCITY_TRAIN = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"INTER_REGIONAL_TRAIN")]
        INTER_REGIONAL_TRAIN = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"REGIONAL_TRAIN")]
        REGIONAL_TRAIN = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"CITY_TRAIN")]
        CITY_TRAIN = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"SUBWAY")]
        SUBWAY = 5,

        [System.Runtime.Serialization.EnumMember(Value = @"TRAM")]
        TRAM = 6,

        [System.Runtime.Serialization.EnumMember(Value = @"BUS")]
        BUS = 7,

        [System.Runtime.Serialization.EnumMember(Value = @"FERRY")]
        FERRY = 8,

        [System.Runtime.Serialization.EnumMember(Value = @"FLIGHT")]
        FLIGHT = 9,

        [System.Runtime.Serialization.EnumMember(Value = @"CAR")]
        CAR = 10,

        [System.Runtime.Serialization.EnumMember(Value = @"TAXI")]
        TAXI = 11,

        [System.Runtime.Serialization.EnumMember(Value = @"SHUTTLE")]
        SHUTTLE = 12,

        [System.Runtime.Serialization.EnumMember(Value = @"BIKE")]
        BIKE = 13,

        [System.Runtime.Serialization.EnumMember(Value = @"SCOOTER")]
        SCOOTER = 14,

        [System.Runtime.Serialization.EnumMember(Value = @"WALK")]
        WALK = 15,

        [System.Runtime.Serialization.EnumMember(Value = @"UNKNOWN")]
        UNKNOWN = 16,

    }

    internal class TolerantTransportTypeConverter : System.Text.Json.Serialization.JsonConverter<TransportType>
    {
        public override TransportType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string transportType = reader.GetString();
            if (Enum.TryParse<TransportType>(transportType, true, out var result)) return result;
            return TransportType.UNKNOWN;
        }

        public override void Write(Utf8JsonWriter writer, TransportType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    internal class DateFormatConverter : System.Text.Json.Serialization.JsonConverter<System.DateTimeOffset>
    {
        public override System.DateTimeOffset Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            var dateTime = reader.GetString();
            if (dateTime == null)
            {
                throw new System.Text.Json.JsonException("Unexpected JsonTokenType.Null");
            }

            return System.DateTimeOffset.Parse(dateTime);
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, System.DateTimeOffset value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
        }
    }
}
