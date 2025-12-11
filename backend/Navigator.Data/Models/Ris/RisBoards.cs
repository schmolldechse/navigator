namespace Navigator.Data.Models.Ris;

public class RisBoards
{
    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    internal interface __ICanIterate
    {
        System.Collections.Generic.IEnumerable<(string name, object? value)> IterateProperties();
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Administration : __ICanIterate
    {
        public Administration() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Administration(string administrationID, string operatorCode, string operatorName)
        {
            AdministrationID = administrationID;
            OperatorCode = operatorCode;
            OperatorName = operatorName;
        }

        public required string AdministrationID { get; set; }
        public required string OperatorCode { get; set; }
        public required string OperatorName { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("administrationID", AdministrationID);
            yield return ("operatorCode", OperatorCode);
            yield return ("operatorName", OperatorName);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class BoardPublicArrival : __ICanIterate
    {
        public BoardPublicArrival() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public BoardPublicArrival(System.Collections.Generic.List<StopArrival> arrivals, System.Collections.Generic.List<DisruptionCommunicationEmbeddedLegacy> disruptions)
        {
            Arrivals = arrivals;
            Disruptions = disruptions;
        }

        public required System.Collections.Generic.List<StopArrival> Arrivals { get; set; }
        public required System.Collections.Generic.List<DisruptionCommunicationEmbeddedLegacy> Disruptions { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("arrivals", Arrivals);
            yield return ("disruptions", Disruptions);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class BoardPublicDeparture : __ICanIterate
    {
        public BoardPublicDeparture() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public BoardPublicDeparture(System.Collections.Generic.List<StopDeparture> departures, System.Collections.Generic.List<DisruptionCommunicationEmbeddedLegacy> disruptions)
        {
            Departures = departures;
            Disruptions = disruptions;
        }

        public required System.Collections.Generic.List<StopDeparture> Departures { get; set; }
        public required System.Collections.Generic.List<DisruptionCommunicationEmbeddedLegacy> Disruptions { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("departures", Departures);
            yield return ("disruptions", Disruptions);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class CodeShare : __ICanIterate
    {
        public CodeShare() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public CodeShare(string airlineCode, int flightnumber)
        {
            AirlineCode = airlineCode;
            Flightnumber = flightnumber;
        }

        public required string AirlineCode { get; set; }
        public required int Flightnumber { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("airlineCode", AirlineCode);
            yield return ("flightnumber", Flightnumber);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class DirectionInfo : __ICanIterate
    {
        public DirectionInfo() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public DirectionInfo(System.Collections.Generic.List<StopPlaceEmbedded> stopPlaces, string text)
        {
            StopPlaces = stopPlaces;
            Text = text;
        }

        public required System.Collections.Generic.List<StopPlaceEmbedded> StopPlaces { get; set; }
        public required string Text { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("stopPlaces", StopPlaces);
            yield return ("text", Text);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class DisruptionCommunicationDescription : __ICanIterate
    {
        public DisruptionCommunicationDescription() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public DisruptionCommunicationDescription(string text, string textShort)
        {
            Text = text;
            TextShort = textShort;
        }

        public required string Text { get; set; }
        public required string TextShort { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("text", Text);
            yield return ("textShort", TextShort);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class DisruptionCommunicationEmbeddedLegacy : __ICanIterate
    {
        public DisruptionCommunicationEmbeddedLegacy() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public DisruptionCommunicationEmbeddedLegacy(System.Collections.Generic.Dictionary<string, object> descriptions, int displayPriority, string disruptionCommunicationID, string disruptionID)
        {
            Descriptions = descriptions;
            DisplayPriority = displayPriority;
            DisruptionCommunicationID = disruptionCommunicationID;
            DisruptionID = disruptionID;
        }

        public required System.Collections.Generic.Dictionary<string, object> Descriptions { get; set; }
        public required int DisplayPriority { get; set; }
        public required string DisruptionCommunicationID { get; set; }
        public required string DisruptionID { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("descriptions", Descriptions);
            yield return ("displayPriority", DisplayPriority);
            yield return ("disruptionCommunicationID", DisruptionCommunicationID);
            yield return ("disruptionID", DisruptionID);
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
    [System.Text.Json.Serialization.JsonConverter(typeof(JourneyTypeEnumConverter))]
    public enum JourneyType
    {
        REGULAR,
        REPLACEMENT,
        RELIEF,
        EXTRA,
    }

    public static class JourneyTypeFastEnum
    {
        public static string ToString(JourneyType value) => value switch
        {
            JourneyType.REGULAR => "REGULAR",
            JourneyType.REPLACEMENT => "REPLACEMENT",
            JourneyType.RELIEF => "RELIEF",
            JourneyType.EXTRA => "EXTRA",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static JourneyType FromString(string? value) => value switch
        {
            "REGULAR" => JourneyType.REGULAR,
            "REPLACEMENT" => JourneyType.REPLACEMENT,
            "RELIEF" => JourneyType.RELIEF,
            "EXTRA" => JourneyType.EXTRA,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class JourneyTypeEnumConverter : System.Text.Json.Serialization.JsonConverter<JourneyType>
    {
        public override JourneyType Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (JourneyType)reader.GetInt32();
            }

            return JourneyTypeFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, JourneyType value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(JourneyTypeFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class MessageAttributeLegacy : __ICanIterate
    {
        public MessageAttributeLegacy() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public MessageAttributeLegacy(string code, int displayPriority, int displayPriorityDetail, string text, string textShort)
        {
            Code = code;
            DisplayPriority = displayPriority;
            DisplayPriorityDetail = displayPriorityDetail;
            Text = text;
            TextShort = textShort;
        }

        public required string Code { get; set; }
        public required int DisplayPriority { get; set; }
        public required int DisplayPriorityDetail { get; set; }
        public required string Text { get; set; }
        public required string TextShort { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("code", Code);
            yield return ("displayPriority", DisplayPriority);
            yield return ("displayPriorityDetail", DisplayPriorityDetail);
            yield return ("text", Text);
            yield return ("textShort", TextShort);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class MessageLegacy : __ICanIterate
    {
        public MessageLegacy() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public MessageLegacy(string category, string code, int displayPriority, string text, string textShort, MessageType type)
        {
            Category = category;
            Code = code;
            DisplayPriority = displayPriority;
            Text = text;
            TextShort = textShort;
            Type = type;
        }

        public required string Category { get; set; }
        public required string Code { get; set; }
        public required int DisplayPriority { get; set; }
        public required string Text { get; set; }
        public required string TextShort { get; set; }
        public required MessageType Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("category", Category);
            yield return ("code", Code);
            yield return ("displayPriority", DisplayPriority);
            yield return ("text", Text);
            yield return ("textShort", TextShort);
            yield return ("type", Type switch
            {
                MessageType.CUSTOMER_TEXT => "CUSTOMER_TEXT",
                MessageType.QUALITY_VARIATION => "QUALITY_VARIATION",
                MessageType.CUSTOMER_REASON => "CUSTOMER_REASON",
                _ => null
            });
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public enum MessageType
    {
        CUSTOMER_TEXT,
        QUALITY_VARIATION,
        CUSTOMER_REASON,
    }

    public static class MessageTypeFastEnum
    {
        public static string ToString(MessageType value) => value switch
        {
            MessageType.CUSTOMER_TEXT => "CUSTOMER_TEXT",
            MessageType.QUALITY_VARIATION => "QUALITY_VARIATION",
            MessageType.CUSTOMER_REASON => "CUSTOMER_REASON",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static MessageType FromString(string? value) => value switch
        {
            "CUSTOMER_TEXT" => MessageType.CUSTOMER_TEXT,
            "QUALITY_VARIATION" => MessageType.QUALITY_VARIATION,
            "CUSTOMER_REASON" => MessageType.CUSTOMER_REASON,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class MessageTypeEnumConverter : System.Text.Json.Serialization.JsonConverter<MessageType>
    {
        public override MessageType Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (MessageType)reader.GetInt32();
            }

            return MessageTypeFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, MessageType value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(MessageTypeFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class ReplacementTransport : __ICanIterate
    {
        public ReplacementTransport() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public ReplacementTransport(string realType)
        {
            RealType = realType;
        }

        public required string RealType { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("realType", RealType);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(SortKeyTimeEnumConverter))]
    public enum SortKeyTime
    {
        TIME,
        TIME_SCHEDULE,
    }

    public static class SortKeyTimeFastEnum
    {
        public static string ToString(SortKeyTime value) => value switch
        {
            SortKeyTime.TIME => "TIME",
            SortKeyTime.TIME_SCHEDULE => "TIME_SCHEDULE",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static SortKeyTime FromString(string? value) => value switch
        {
            "TIME" => SortKeyTime.TIME,
            "TIME_SCHEDULE" => SortKeyTime.TIME_SCHEDULE,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class SortKeyTimeEnumConverter : System.Text.Json.Serialization.JsonConverter<SortKeyTime>
    {
        public override SortKeyTime Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (SortKeyTime)reader.GetInt32();
            }

            return SortKeyTimeFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, SortKeyTime value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(SortKeyTimeFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopArrival : __ICanIterate
    {
        public StopArrival() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopArrival(bool additional, Administration administration, string arrivalID, System.Collections.Generic.List<MessageAttributeLegacy> attributes, bool canceled, System.Collections.Generic.List<CodeShare> codeshares, TransportPublicOrigin continuationFor, System.Collections.Generic.List<DisruptionCommunicationEmbeddedLegacy> disruptions, string journeyID, JourneyType journeyType, System.Collections.Generic.List<MessageLegacy> messages, bool onDemand, bool pastDisruptions, string platform, string platformSchedule, System.Collections.Generic.List<TransportPublicOrigin> reliefBy, System.Collections.Generic.List<TransportPublicOrigin> reliefFor, System.Collections.Generic.List<TransportPublicOrigin> replacedBy, System.Collections.Generic.List<TransportPublicOrigin> replacementFor, StopPlaceEmbedded station, System.DateTime time, System.DateTime timeSchedule, TimeType timeType, TransportPublicOriginVia transport, System.Collections.Generic.List<TransportPublicOrigin> travelsWith)
        {
            Additional = additional;
            Administration = administration;
            ArrivalID = arrivalID;
            Attributes = attributes;
            Canceled = canceled;
            Codeshares = codeshares;
            ContinuationFor = continuationFor;
            Disruptions = disruptions;
            JourneyID = journeyID;
            JourneyType = journeyType;
            Messages = messages;
            OnDemand = onDemand;
            PastDisruptions = pastDisruptions;
            Platform = platform;
            PlatformSchedule = platformSchedule;
            ReliefBy = reliefBy;
            ReliefFor = reliefFor;
            ReplacedBy = replacedBy;
            ReplacementFor = replacementFor;
            Station = station;
            Time = time;
            TimeSchedule = timeSchedule;
            TimeType = timeType;
            Transport = transport;
            TravelsWith = travelsWith;
        }

        public required bool Additional { get; set; }
        public required Administration Administration { get; set; }
        public required string ArrivalID { get; set; }
        public required System.Collections.Generic.List<MessageAttributeLegacy> Attributes { get; set; }
        public required bool Canceled { get; set; }
        public required System.Collections.Generic.List<CodeShare> Codeshares { get; set; }
        public required TransportPublicOrigin ContinuationFor { get; set; }
        public required System.Collections.Generic.List<DisruptionCommunicationEmbeddedLegacy> Disruptions { get; set; }
        public required string JourneyID { get; set; }
        public required JourneyType JourneyType { get; set; }
        public required System.Collections.Generic.List<MessageLegacy> Messages { get; set; }
        public required bool OnDemand { get; set; }
        public required bool PastDisruptions { get; set; }
        public required string Platform { get; set; }
        public required string PlatformSchedule { get; set; }
        public required System.Collections.Generic.List<TransportPublicOrigin> ReliefBy { get; set; }
        public required System.Collections.Generic.List<TransportPublicOrigin> ReliefFor { get; set; }
        public required System.Collections.Generic.List<TransportPublicOrigin> ReplacedBy { get; set; }
        public required System.Collections.Generic.List<TransportPublicOrigin> ReplacementFor { get; set; }
        public required StopPlaceEmbedded Station { get; set; }
        public required System.DateTime Time { get; set; }
        public required System.DateTime TimeSchedule { get; set; }
        public required TimeType TimeType { get; set; }
        public required TransportPublicOriginVia Transport { get; set; }
        public required System.Collections.Generic.List<TransportPublicOrigin> TravelsWith { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("additional", Additional);
            yield return ("administration", Administration);
            yield return ("arrivalID", ArrivalID);
            yield return ("attributes", Attributes);
            yield return ("canceled", Canceled);
            yield return ("codeshares", Codeshares);
            yield return ("continuationFor", ContinuationFor);
            yield return ("disruptions", Disruptions);
            yield return ("journeyID", JourneyID);
            yield return ("journeyType", JourneyType switch
            {
                JourneyType.REGULAR => "REGULAR",
                JourneyType.REPLACEMENT => "REPLACEMENT",
                JourneyType.RELIEF => "RELIEF",
                JourneyType.EXTRA => "EXTRA",
                _ => null
            });
            yield return ("messages", Messages);
            yield return ("onDemand", OnDemand);
            yield return ("pastDisruptions", PastDisruptions);
            yield return ("platform", Platform);
            yield return ("platformSchedule", PlatformSchedule);
            yield return ("reliefBy", ReliefBy);
            yield return ("reliefFor", ReliefFor);
            yield return ("replacedBy", ReplacedBy);
            yield return ("replacementFor", ReplacementFor);
            yield return ("station", Station);
            yield return ("time", Time);
            yield return ("timeSchedule", TimeSchedule);
            yield return ("timeType", TimeType switch
            {
                TimeType.SCHEDULE => "SCHEDULE",
                TimeType.PREVIEW => "PREVIEW",
                TimeType.REAL => "REAL",
                _ => null
            });
            yield return ("transport", Transport);
            yield return ("travelsWith", TravelsWith);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopAtStopPlace : __ICanIterate
    {
        public StopAtStopPlace() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopAtStopPlace(bool canceled, string evaNumber, string ifopt, string name)
        {
            Canceled = canceled;
            EvaNumber = evaNumber;
            Ifopt = ifopt;
            Name = name;
        }

        public required bool Canceled { get; set; }
        public required string EvaNumber { get; set; }
        public required string Ifopt { get; set; }
        public required string Name { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("canceled", Canceled);
            yield return ("evaNumber", EvaNumber);
            yield return ("ifopt", Ifopt);
            yield return ("name", Name);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopAtStopPlacePrio : __ICanIterate
    {
        public StopAtStopPlacePrio() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopAtStopPlacePrio(bool additional, bool canceled, int displayPriority, string evaNumber, string ifopt, string name)
        {
            Additional = additional;
            Canceled = canceled;
            DisplayPriority = displayPriority;
            EvaNumber = evaNumber;
            Ifopt = ifopt;
            Name = name;
        }

        public required bool Additional { get; set; }
        public required bool Canceled { get; set; }
        public required int DisplayPriority { get; set; }
        public required string EvaNumber { get; set; }
        public required string Ifopt { get; set; }
        public required string Name { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("additional", Additional);
            yield return ("canceled", Canceled);
            yield return ("displayPriority", DisplayPriority);
            yield return ("evaNumber", EvaNumber);
            yield return ("ifopt", Ifopt);
            yield return ("name", Name);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopDeparture : __ICanIterate
    {
        public StopDeparture() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopDeparture(bool additional, Administration administration, System.Collections.Generic.List<MessageAttributeLegacy> attributes, bool canceled, System.Collections.Generic.List<CodeShare> codeshares, TransportPublicDestination continuationBy, string departureID, System.Collections.Generic.List<DisruptionCommunicationEmbeddedLegacy> disruptions, bool futureDisruptions, string journeyID, JourneyType journeyType, System.Collections.Generic.List<MessageLegacy> messages, bool onDemand, string platform, string platformSchedule, System.Collections.Generic.List<TransportPublicDestination> reliefBy, System.Collections.Generic.List<TransportPublicDestination> reliefFor, System.Collections.Generic.List<TransportPublicDestination> replacedBy, System.Collections.Generic.List<TransportPublicDestination> replacementFor, StopPlaceEmbedded station, System.DateTime time, System.DateTime timeSchedule, TimeType timeType, TransportPublicDestinationVia transport, System.Collections.Generic.List<TransportPublicDestinationPortionWorking> travelsWith)
        {
            Additional = additional;
            Administration = administration;
            Attributes = attributes;
            Canceled = canceled;
            Codeshares = codeshares;
            ContinuationBy = continuationBy;
            DepartureID = departureID;
            Disruptions = disruptions;
            FutureDisruptions = futureDisruptions;
            JourneyID = journeyID;
            JourneyType = journeyType;
            Messages = messages;
            OnDemand = onDemand;
            Platform = platform;
            PlatformSchedule = platformSchedule;
            ReliefBy = reliefBy;
            ReliefFor = reliefFor;
            ReplacedBy = replacedBy;
            ReplacementFor = replacementFor;
            Station = station;
            Time = time;
            TimeSchedule = timeSchedule;
            TimeType = timeType;
            Transport = transport;
            TravelsWith = travelsWith;
        }

        public required bool Additional { get; set; }
        public required Administration Administration { get; set; }
        public required System.Collections.Generic.List<MessageAttributeLegacy> Attributes { get; set; }
        public required bool Canceled { get; set; }
        public required System.Collections.Generic.List<CodeShare> Codeshares { get; set; }
        public required TransportPublicDestination ContinuationBy { get; set; }
        public required string DepartureID { get; set; }
        public required System.Collections.Generic.List<DisruptionCommunicationEmbeddedLegacy> Disruptions { get; set; }
        public required bool FutureDisruptions { get; set; }
        public required string JourneyID { get; set; }
        public required JourneyType JourneyType { get; set; }
        public required System.Collections.Generic.List<MessageLegacy> Messages { get; set; }
        public required bool OnDemand { get; set; }
        public required string Platform { get; set; }
        public required string PlatformSchedule { get; set; }
        public required System.Collections.Generic.List<TransportPublicDestination> ReliefBy { get; set; }
        public required System.Collections.Generic.List<TransportPublicDestination> ReliefFor { get; set; }
        public required System.Collections.Generic.List<TransportPublicDestination> ReplacedBy { get; set; }
        public required System.Collections.Generic.List<TransportPublicDestination> ReplacementFor { get; set; }
        public required StopPlaceEmbedded Station { get; set; }
        public required System.DateTime Time { get; set; }
        public required System.DateTime TimeSchedule { get; set; }
        public required TimeType TimeType { get; set; }
        public required TransportPublicDestinationVia Transport { get; set; }
        public required System.Collections.Generic.List<TransportPublicDestinationPortionWorking> TravelsWith { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("additional", Additional);
            yield return ("administration", Administration);
            yield return ("attributes", Attributes);
            yield return ("canceled", Canceled);
            yield return ("codeshares", Codeshares);
            yield return ("continuationBy", ContinuationBy);
            yield return ("departureID", DepartureID);
            yield return ("disruptions", Disruptions);
            yield return ("futureDisruptions", FutureDisruptions);
            yield return ("journeyID", JourneyID);
            yield return ("journeyType", JourneyType switch
            {
                JourneyType.REGULAR => "REGULAR",
                JourneyType.REPLACEMENT => "REPLACEMENT",
                JourneyType.RELIEF => "RELIEF",
                JourneyType.EXTRA => "EXTRA",
                _ => null
            });
            yield return ("messages", Messages);
            yield return ("onDemand", OnDemand);
            yield return ("platform", Platform);
            yield return ("platformSchedule", PlatformSchedule);
            yield return ("reliefBy", ReliefBy);
            yield return ("reliefFor", ReliefFor);
            yield return ("replacedBy", ReplacedBy);
            yield return ("replacementFor", ReplacementFor);
            yield return ("station", Station);
            yield return ("time", Time);
            yield return ("timeSchedule", TimeSchedule);
            yield return ("timeType", TimeType switch
            {
                TimeType.SCHEDULE => "SCHEDULE",
                TimeType.PREVIEW => "PREVIEW",
                TimeType.REAL => "REAL",
                _ => null
            });
            yield return ("transport", Transport);
            yield return ("travelsWith", TravelsWith);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopPlaceEmbedded : __ICanIterate
    {
        public StopPlaceEmbedded() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlaceEmbedded(string evaNumber, string ifopt, string name)
        {
            EvaNumber = evaNumber;
            Ifopt = ifopt;
            Name = name;
        }

        public required string EvaNumber { get; set; }
        public required string Ifopt { get; set; }
        public required string Name { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("evaNumber", EvaNumber);
            yield return ("ifopt", Ifopt);
            yield return ("name", Name);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    [System.Text.Json.Serialization.JsonConverter(typeof(TimeTypeEnumConverter))]
    public enum TimeType
    {
        SCHEDULE,
        PREVIEW,
        REAL,
    }

    public static class TimeTypeFastEnum
    {
        public static string ToString(TimeType value) => value switch
        {
            TimeType.SCHEDULE => "SCHEDULE",
            TimeType.PREVIEW => "PREVIEW",
            TimeType.REAL => "REAL",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static TimeType FromString(string? value) => value switch
        {
            "SCHEDULE" => TimeType.SCHEDULE,
            "PREVIEW" => TimeType.PREVIEW,
            "REAL" => TimeType.REAL,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class TimeTypeEnumConverter : System.Text.Json.Serialization.JsonConverter<TimeType>
    {
        public override TimeType Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (TimeType)reader.GetInt32();
            }

            return TimeTypeFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, TimeType value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(TimeTypeFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class TransportPublicDestination : __ICanIterate
    {
        public TransportPublicDestination() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public TransportPublicDestination(string category, string categoryInternal, StopAtStopPlace destination, string dfid, StopAtStopPlace differingDestination, DirectionInfo direction, string dtid, string journeyDescription, string journeyID, string label, string line, int number, ReplacementTransport replacementTransport, TransportType type)
        {
            Category = category;
            CategoryInternal = categoryInternal;
            Destination = destination;
            Dfid = dfid;
            DifferingDestination = differingDestination;
            Direction = direction;
            Dtid = dtid;
            JourneyDescription = journeyDescription;
            JourneyID = journeyID;
            Label = label;
            Line = line;
            Number = number;
            ReplacementTransport = replacementTransport;
            Type = type;
        }

        public required string Category { get; set; }
        public required string CategoryInternal { get; set; }
        public required StopAtStopPlace Destination { get; set; }
        public required string Dfid { get; set; }
        public required StopAtStopPlace DifferingDestination { get; set; }
        public required DirectionInfo Direction { get; set; }
        public required string Dtid { get; set; }
        public required string JourneyDescription { get; set; }
        public required string JourneyID { get; set; }
        public required string Label { get; set; }
        public required string Line { get; set; }
        public required int Number { get; set; }
        public required ReplacementTransport ReplacementTransport { get; set; }
        public required TransportType Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("category", Category);
            yield return ("categoryInternal", CategoryInternal);
            yield return ("destination", Destination);
            yield return ("dfid", Dfid);
            yield return ("differingDestination", DifferingDestination);
            yield return ("direction", Direction);
            yield return ("dtid", Dtid);
            yield return ("journeyDescription", JourneyDescription);
            yield return ("journeyID", JourneyID);
            yield return ("label", Label);
            yield return ("line", Line);
            yield return ("number", Number);
            yield return ("replacementTransport", ReplacementTransport);
            yield return ("type", Type switch
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
                _ => null
            });
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class TransportPublicDestinationPortionWorking : __ICanIterate
    {
        public TransportPublicDestinationPortionWorking() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public TransportPublicDestinationPortionWorking(string category, string categoryInternal, StopAtStopPlace destination, string dfid, StopAtStopPlace differingDestination, DirectionInfo direction, string dtid, string journeyDescription, string journeyID, string label, string line, int number, ReplacementTransport replacementTransport, StopPlaceEmbedded separationAt, TransportType type)
        {
            Category = category;
            CategoryInternal = categoryInternal;
            Destination = destination;
            Dfid = dfid;
            DifferingDestination = differingDestination;
            Direction = direction;
            Dtid = dtid;
            JourneyDescription = journeyDescription;
            JourneyID = journeyID;
            Label = label;
            Line = line;
            Number = number;
            ReplacementTransport = replacementTransport;
            SeparationAt = separationAt;
            Type = type;
        }

        public required string Category { get; set; }
        public required string CategoryInternal { get; set; }
        public required StopAtStopPlace Destination { get; set; }
        public required string Dfid { get; set; }
        public required StopAtStopPlace DifferingDestination { get; set; }
        public required DirectionInfo Direction { get; set; }
        public required string Dtid { get; set; }
        public required string JourneyDescription { get; set; }
        public required string JourneyID { get; set; }
        public required string Label { get; set; }
        public required string Line { get; set; }
        public required int Number { get; set; }
        public required ReplacementTransport ReplacementTransport { get; set; }
        public required StopPlaceEmbedded SeparationAt { get; set; }
        public required TransportType Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("category", Category);
            yield return ("categoryInternal", CategoryInternal);
            yield return ("destination", Destination);
            yield return ("dfid", Dfid);
            yield return ("differingDestination", DifferingDestination);
            yield return ("direction", Direction);
            yield return ("dtid", Dtid);
            yield return ("journeyDescription", JourneyDescription);
            yield return ("journeyID", JourneyID);
            yield return ("label", Label);
            yield return ("line", Line);
            yield return ("number", Number);
            yield return ("replacementTransport", ReplacementTransport);
            yield return ("separationAt", SeparationAt);
            yield return ("type", Type switch
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
                _ => null
            });
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class TransportPublicDestinationVia : __ICanIterate
    {
        public TransportPublicDestinationVia() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public TransportPublicDestinationVia(string category, string categoryInternal, StopAtStopPlace destination, string dfid, StopAtStopPlace differingDestination, DirectionInfo direction, string dtid, string journeyDescription, string journeyID, string label, string line, int number, ReplacementTransport replacementTransport, TransportType type, System.Collections.Generic.List<StopAtStopPlacePrio> via)
        {
            Category = category;
            CategoryInternal = categoryInternal;
            Destination = destination;
            Dfid = dfid;
            DifferingDestination = differingDestination;
            Direction = direction;
            Dtid = dtid;
            JourneyDescription = journeyDescription;
            JourneyID = journeyID;
            Label = label;
            Line = line;
            Number = number;
            ReplacementTransport = replacementTransport;
            Type = type;
            Via = via;
        }

        public required string Category { get; set; }
        public required string CategoryInternal { get; set; }
        public required StopAtStopPlace Destination { get; set; }
        public required string Dfid { get; set; }
        public required StopAtStopPlace DifferingDestination { get; set; }
        public required DirectionInfo Direction { get; set; }
        public required string Dtid { get; set; }
        public required string JourneyDescription { get; set; }
        public required string JourneyID { get; set; }
        public required string Label { get; set; }
        public required string Line { get; set; }
        public required int Number { get; set; }
        public required ReplacementTransport ReplacementTransport { get; set; }
        public required TransportType Type { get; set; }
        public required System.Collections.Generic.List<StopAtStopPlacePrio> Via { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("category", Category);
            yield return ("categoryInternal", CategoryInternal);
            yield return ("destination", Destination);
            yield return ("dfid", Dfid);
            yield return ("differingDestination", DifferingDestination);
            yield return ("direction", Direction);
            yield return ("dtid", Dtid);
            yield return ("journeyDescription", JourneyDescription);
            yield return ("journeyID", JourneyID);
            yield return ("label", Label);
            yield return ("line", Line);
            yield return ("number", Number);
            yield return ("replacementTransport", ReplacementTransport);
            yield return ("type", Type switch
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
                _ => null
            });
            yield return ("via", Via);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class TransportPublicOrigin : __ICanIterate
    {
        public TransportPublicOrigin() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public TransportPublicOrigin(string category, string categoryInternal, string dfid, StopAtStopPlace differingOrigin, DirectionInfo direction, string dtid, string journeyDescription, string journeyID, string label, string line, int number, StopAtStopPlace origin, ReplacementTransport replacementTransport, TransportType type)
        {
            Category = category;
            CategoryInternal = categoryInternal;
            Dfid = dfid;
            DifferingOrigin = differingOrigin;
            Direction = direction;
            Dtid = dtid;
            JourneyDescription = journeyDescription;
            JourneyID = journeyID;
            Label = label;
            Line = line;
            Number = number;
            Origin = origin;
            ReplacementTransport = replacementTransport;
            Type = type;
        }

        public required string Category { get; set; }
        public required string CategoryInternal { get; set; }
        public required string Dfid { get; set; }
        public required StopAtStopPlace DifferingOrigin { get; set; }
        public required DirectionInfo Direction { get; set; }
        public required string Dtid { get; set; }
        public required string JourneyDescription { get; set; }
        public required string JourneyID { get; set; }
        public required string Label { get; set; }
        public required string Line { get; set; }
        public required int Number { get; set; }
        public required StopAtStopPlace Origin { get; set; }
        public required ReplacementTransport ReplacementTransport { get; set; }
        public required TransportType Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("category", Category);
            yield return ("categoryInternal", CategoryInternal);
            yield return ("dfid", Dfid);
            yield return ("differingOrigin", DifferingOrigin);
            yield return ("direction", Direction);
            yield return ("dtid", Dtid);
            yield return ("journeyDescription", JourneyDescription);
            yield return ("journeyID", JourneyID);
            yield return ("label", Label);
            yield return ("line", Line);
            yield return ("number", Number);
            yield return ("origin", Origin);
            yield return ("replacementTransport", ReplacementTransport);
            yield return ("type", Type switch
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
                _ => null
            });
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class TransportPublicOriginVia : __ICanIterate
    {
        public TransportPublicOriginVia() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public TransportPublicOriginVia(string category, string categoryInternal, string dfid, StopAtStopPlace differingOrigin, DirectionInfo direction, string dtid, string journeyDescription, string journeyID, string label, string line, int number, StopAtStopPlace origin, ReplacementTransport replacementTransport, TransportType type, System.Collections.Generic.List<StopAtStopPlacePrio> via)
        {
            Category = category;
            CategoryInternal = categoryInternal;
            Dfid = dfid;
            DifferingOrigin = differingOrigin;
            Direction = direction;
            Dtid = dtid;
            JourneyDescription = journeyDescription;
            JourneyID = journeyID;
            Label = label;
            Line = line;
            Number = number;
            Origin = origin;
            ReplacementTransport = replacementTransport;
            Type = type;
            Via = via;
        }

        public required string Category { get; set; }
        public required string CategoryInternal { get; set; }
        public required string Dfid { get; set; }
        public required StopAtStopPlace DifferingOrigin { get; set; }
        public required DirectionInfo Direction { get; set; }
        public required string Dtid { get; set; }
        public required string JourneyDescription { get; set; }
        public required string JourneyID { get; set; }
        public required string Label { get; set; }
        public required string Line { get; set; }
        public required int Number { get; set; }
        public required StopAtStopPlace Origin { get; set; }
        public required ReplacementTransport ReplacementTransport { get; set; }
        public required TransportType Type { get; set; }
        public required System.Collections.Generic.List<StopAtStopPlacePrio> Via { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("category", Category);
            yield return ("categoryInternal", CategoryInternal);
            yield return ("dfid", Dfid);
            yield return ("differingOrigin", DifferingOrigin);
            yield return ("direction", Direction);
            yield return ("dtid", Dtid);
            yield return ("journeyDescription", JourneyDescription);
            yield return ("journeyID", JourneyID);
            yield return ("label", Label);
            yield return ("line", Line);
            yield return ("number", Number);
            yield return ("origin", Origin);
            yield return ("replacementTransport", ReplacementTransport);
            yield return ("type", Type switch
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
                _ => null
            });
            yield return ("via", Via);
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
