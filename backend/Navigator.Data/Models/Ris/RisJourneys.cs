using Microsoft.Build.Tasks;
using System.Security.Cryptography.Xml;
using static Navigator.Data.Models.Ris.RisBoards;

namespace Navigator.Data.Models.Ris;

public class RisJourneys
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
    public sealed class ContinuationInfo : __ICanIterate
    {
        public ContinuationInfo() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public ContinuationInfo(string arrivalOrDepartureID, StopPlaceEmbedded stopPlace)
        {
            ArrivalOrDepartureID = arrivalOrDepartureID;
            StopPlace = stopPlace;
        }

        public required string ArrivalOrDepartureID { get; set; }
        public required StopPlaceEmbedded StopPlace { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("arrivalOrDepartureID", ArrivalOrDepartureID);
            yield return ("stopPlace", StopPlace);
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
    public sealed class DisruptionCommunicationAttachment : __ICanIterate
    {
        public DisruptionCommunicationAttachment() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public DisruptionCommunicationAttachment(string label, string url)
        {
            Label = label;
            Url = url;
        }

        public required string Label { get; set; }
        public required string Url { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("label", Label);
            yield return ("url", Url);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class DisruptionCommunicationDescription : __ICanIterate
    {
        public DisruptionCommunicationDescription() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public DisruptionCommunicationDescription(System.Collections.Generic.List<DisruptionCommunicationAttachment> attachments, System.Collections.Generic.List<DisruptionCommunicationImage> images, System.Collections.Generic.List<DisruptionCommunicationLink> links, string text, string textShort)
        {
            Attachments = attachments;
            Images = images;
            Links = links;
            Text = text;
            TextShort = textShort;
        }

        public required System.Collections.Generic.List<DisruptionCommunicationAttachment> Attachments { get; set; }
        public required System.Collections.Generic.List<DisruptionCommunicationImage> Images { get; set; }
        public required System.Collections.Generic.List<DisruptionCommunicationLink> Links { get; set; }
        public required string Text { get; set; }
        public required string TextShort { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("attachments", Attachments);
            yield return ("images", Images);
            yield return ("links", Links);
            yield return ("text", Text);
            yield return ("textShort", TextShort);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class DisruptionCommunicationImage : __ICanIterate
    {
        public DisruptionCommunicationImage() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public DisruptionCommunicationImage(string label, string url)
        {
            Label = label;
            Url = url;
        }

        public required string Label { get; set; }
        public required string Url { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("label", Label);
            yield return ("url", Url);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class DisruptionCommunicationLink : __ICanIterate
    {
        public DisruptionCommunicationLink() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public DisruptionCommunicationLink(string label, string url)
        {
            Label = label;
            Url = url;
        }

        public required string Label { get; set; }
        public required string Url { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("label", Label);
            yield return ("url", Url);
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
    [System.Text.Json.Serialization.JsonConverter(typeof(EventTypeEnumConverter))]
    public enum EventType
    {
        ARRIVAL,
        DEPARTURE,
    }

    public static class EventTypeFastEnum
    {
        public static string ToString(EventType value) => value switch
        {
            EventType.ARRIVAL => "ARRIVAL",
            EventType.DEPARTURE => "DEPARTURE",
            _ => throw new System.NotSupportedException(value + " is not a supported Enum value")
        };

        public static EventType FromString(string? value) => value switch
        {
            "ARRIVAL" => EventType.ARRIVAL,
            "DEPARTURE" => EventType.DEPARTURE,
            _ => throw new System.NotSupportedException((value ?? "NULL") + " is not a supported Enum value")
        };
    }

    public class EventTypeEnumConverter : System.Text.Json.Serialization.JsonConverter<EventType>
    {
        public override EventType Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType is System.Text.Json.JsonTokenType.Number)
            {
                return (EventType)reader.GetInt32();
            }

            return EventTypeFastEnum.FromString(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, EventType value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(EventTypeFastEnum.ToString(value));
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyBatchError : __ICanIterate
    {
        public JourneyBatchError() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyBatchError(string errorCode, string errorText, string journeyID)
        {
            ErrorCode = errorCode;
            ErrorText = errorText;
            JourneyID = journeyID;
        }

        public required string ErrorCode { get; set; }
        public required string ErrorText { get; set; }
        public required string JourneyID { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("errorCode", ErrorCode);
            yield return ("errorText", ErrorText);
            yield return ("journeyID", JourneyID);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyBatchMatchByEventsRequest : __ICanIterate
    {
        public JourneyBatchMatchByEventsRequest() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyBatchMatchByEventsRequest(System.Collections.Generic.List<JourneyMatchByEventsRequest> requests, bool returnJourneyEvents, int timeTolerance)
        {
            Requests = requests;
            ReturnJourneyEvents = returnJourneyEvents;
            TimeTolerance = timeTolerance;
        }

        public required System.Collections.Generic.List<JourneyMatchByEventsRequest> Requests { get; set; }
        public required bool ReturnJourneyEvents { get; set; }
        public required int TimeTolerance { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("requests", Requests);
            yield return ("returnJourneyEvents", ReturnJourneyEvents);
            yield return ("timeTolerance", TimeTolerance);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyBatchMatchByStartEndRequest : __ICanIterate
    {
        public JourneyBatchMatchByStartEndRequest() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyBatchMatchByStartEndRequest(System.Collections.Generic.List<JourneyMatchByStartEndRequest> requests, bool returnJourneyEvents, int timeTolerance)
        {
            Requests = requests;
            ReturnJourneyEvents = returnJourneyEvents;
            TimeTolerance = timeTolerance;
        }

        public required System.Collections.Generic.List<JourneyMatchByStartEndRequest> Requests { get; set; }
        public required bool ReturnJourneyEvents { get; set; }
        public required int TimeTolerance { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("requests", Requests);
            yield return ("returnJourneyEvents", ReturnJourneyEvents);
            yield return ("timeTolerance", TimeTolerance);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyBatchMatchResponse : __ICanIterate
    {
        public JourneyBatchMatchResponse() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyBatchMatchResponse(System.Collections.Generic.List<JourneyMatchResponse> responses)
        {
            Responses = responses;
        }

        public required System.Collections.Generic.List<JourneyMatchResponse> Responses { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("responses", Responses);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyBatchRequest : __ICanIterate
    {
        public JourneyBatchRequest() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyBatchRequest(bool includeReferences, System.Collections.Generic.List<string> journeyIDs, System.Collections.Generic.List<string> languages, bool separateCancelled)
        {
            IncludeReferences = includeReferences;
            JourneyIDs = journeyIDs;
            Languages = languages;
            SeparateCancelled = separateCancelled;
        }

        public required bool IncludeReferences { get; set; }
        public required System.Collections.Generic.List<string> JourneyIDs { get; set; }
        public required System.Collections.Generic.List<string> Languages { get; set; }
        public required bool SeparateCancelled { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("includeReferences", IncludeReferences);
            yield return ("journeyIDs", JourneyIDs);
            yield return ("languages", Languages);
            yield return ("separateCancelled", SeparateCancelled);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyBatchResponse : __ICanIterate
    {
        public JourneyBatchResponse() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyBatchResponse(System.Collections.Generic.List<JourneyBatchError> erroneousJourneys, System.Collections.Generic.List<JourneyEventBased> journeys, System.Collections.Generic.List<MetaLastChangedTimestamp> metaLastChangedTimestamps)
        {
            ErroneousJourneys = erroneousJourneys;
            Journeys = journeys;
            MetaLastChangedTimestamps = metaLastChangedTimestamps;
        }

        public required System.Collections.Generic.List<JourneyBatchError> ErroneousJourneys { get; set; }
        public required System.Collections.Generic.List<JourneyEventBased> Journeys { get; set; }
        public required System.Collections.Generic.List<MetaLastChangedTimestamp> MetaLastChangedTimestamps { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("erroneousJourneys", ErroneousJourneys);
            yield return ("journeys", Journeys);
            yield return ("metaLastChangedTimestamps", MetaLastChangedTimestamps);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyByRelationResults : __ICanIterate
    {
        public JourneyByRelationResults() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyByRelationResults(System.Collections.Generic.List<JourneyFindResult> journeys)
        {
            Journeys = journeys;
        }

        public required System.Collections.Generic.List<JourneyFindResult> Journeys { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("journeys", Journeys);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyEvent : __ICanIterate
    {
        public JourneyEvent() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyEvent(bool additional, string arrivalOrDepartureID, bool cancelled, System.Collections.Generic.List<CodeShare> codeshares, System.Collections.Generic.List<int> messages, bool noPassengerChange, bool onDemand, string platform, string platformIfopt, string platformSchedule, string platformScheduleIfopt, System.Collections.Generic.List<TransportDestinationRef> reliefBy, System.Collections.Generic.List<TransportDestinationRef> reliefFor, System.Collections.Generic.List<TransportDestinationRef> replacedBy, System.Collections.Generic.List<TransportDestinationRef> replacementFor, StopPlaceInJourney stopPlace, System.DateTime time, System.DateTime timeSchedule, string timeType, TransportWithDirection transport, System.Collections.Generic.List<TransportDestinationPortionWorkingRef> travelsWith, EventType type)
        {
            Additional = additional;
            ArrivalOrDepartureID = arrivalOrDepartureID;
            Cancelled = cancelled;
            Codeshares = codeshares;
            Messages = messages;
            NoPassengerChange = noPassengerChange;
            OnDemand = onDemand;
            Platform = platform;
            PlatformIfopt = platformIfopt;
            PlatformSchedule = platformSchedule;
            PlatformScheduleIfopt = platformScheduleIfopt;
            ReliefBy = reliefBy;
            ReliefFor = reliefFor;
            ReplacedBy = replacedBy;
            ReplacementFor = replacementFor;
            StopPlace = stopPlace;
            Time = time;
            TimeSchedule = timeSchedule;
            TimeType = timeType;
            Transport = transport;
            TravelsWith = travelsWith;
            Type = type;
        }

        public required bool Additional { get; set; }
        public required string ArrivalOrDepartureID { get; set; }
        public required bool Cancelled { get; set; }
        public required System.Collections.Generic.List<CodeShare> Codeshares { get; set; }
        public required System.Collections.Generic.List<int> Messages { get; set; }
        public required bool NoPassengerChange { get; set; }
        public required bool OnDemand { get; set; }
        public required string Platform { get; set; }
        public required string PlatformIfopt { get; set; }
        public required string PlatformSchedule { get; set; }
        public required string PlatformScheduleIfopt { get; set; }
        public required System.Collections.Generic.List<TransportDestinationRef> ReliefBy { get; set; }
        public required System.Collections.Generic.List<TransportDestinationRef> ReliefFor { get; set; }
        public required System.Collections.Generic.List<TransportDestinationRef> ReplacedBy { get; set; }
        public required System.Collections.Generic.List<TransportDestinationRef> ReplacementFor { get; set; }
        public required StopPlaceInJourney StopPlace { get; set; }
        public required System.DateTime Time { get; set; }
        public required System.DateTime TimeSchedule { get; set; }
        public required string TimeType { get; set; }
        public required TransportWithDirection Transport { get; set; }
        public required System.Collections.Generic.List<TransportDestinationPortionWorkingRef> TravelsWith { get; set; }
        public required EventType Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("additional", Additional);
            yield return ("arrivalOrDepartureID", ArrivalOrDepartureID);
            yield return ("cancelled", Cancelled);
            yield return ("codeshares", Codeshares);
            yield return ("messages", Messages);
            yield return ("noPassengerChange", NoPassengerChange);
            yield return ("onDemand", OnDemand);
            yield return ("platform", Platform);
            yield return ("platformIfopt", PlatformIfopt);
            yield return ("platformSchedule", PlatformSchedule);
            yield return ("platformScheduleIfopt", PlatformScheduleIfopt);
            yield return ("reliefBy", ReliefBy);
            yield return ("reliefFor", ReliefFor);
            yield return ("replacedBy", ReplacedBy);
            yield return ("replacementFor", ReplacementFor);
            yield return ("stopPlace", StopPlace);
            yield return ("time", Time);
            yield return ("timeSchedule", TimeSchedule);
            yield return ("timeType", TimeType);
            yield return ("transport", Transport);
            yield return ("travelsWith", TravelsWith);
            yield return ("type", Type switch
            {
                EventType.ARRIVAL => "ARRIVAL",
                EventType.DEPARTURE => "DEPARTURE",
                _ => null
            });
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyEventBased : __ICanIterate
    {
        public JourneyEventBased() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyEventBased(System.Collections.Generic.List<TransportDestinationContinuationRef> continuationBy, System.Collections.Generic.List<TransportOriginContinuationRef> continuationFor, System.Collections.Generic.List<JourneyEvent> events, System.Collections.Generic.List<JourneyEvent> eventsCancelled, JourneyInfo info, string journeyID, Messages messages)
        {
            ContinuationBy = continuationBy;
            ContinuationFor = continuationFor;
            Events = events;
            EventsCancelled = eventsCancelled;
            Info = info;
            JourneyID = journeyID;
            Messages = messages;
        }

        public required System.Collections.Generic.List<TransportDestinationContinuationRef> ContinuationBy { get; set; }
        public required System.Collections.Generic.List<TransportOriginContinuationRef> ContinuationFor { get; set; }
        public required System.Collections.Generic.List<JourneyEvent> Events { get; set; }
        public required System.Collections.Generic.List<JourneyEvent> EventsCancelled { get; set; }
        public required JourneyInfo Info { get; set; }
        public required string JourneyID { get; set; }
        public required Messages Messages { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("continuationBy", ContinuationBy);
            yield return ("continuationFor", ContinuationFor);
            yield return ("events", Events);
            yield return ("eventsCancelled", EventsCancelled);
            yield return ("info", Info);
            yield return ("journeyID", JourneyID);
            yield return ("messages", Messages);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyFindResult : __ICanIterate
    {
        public JourneyFindResult() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyFindResult(JourneyInfo info, string journeyID, JourneyRelation journeyRelation)
        {
            Info = info;
            JourneyID = journeyID;
            JourneyRelation = journeyRelation;
        }

        public required JourneyInfo Info { get; set; }
        public required string JourneyID { get; set; }
        public required JourneyRelation JourneyRelation { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("info", Info);
            yield return ("journeyID", JourneyID);
            yield return ("journeyRelation", JourneyRelation);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyFindResults : __ICanIterate
    {
        public JourneyFindResults() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyFindResults(System.Collections.Generic.List<JourneyFindResult> journeys, int limit, int offset, int total)
        {
            Journeys = journeys;
            Limit = limit;
            Offset = offset;
            Total = total;
        }

        public required System.Collections.Generic.List<JourneyFindResult> Journeys { get; set; }
        public required int Limit { get; set; }
        public required int Offset { get; set; }
        public required int Total { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("journeys", Journeys);
            yield return ("limit", Limit);
            yield return ("offset", Offset);
            yield return ("total", Total);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyInfo : __ICanIterate
    {
        public JourneyInfo() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyInfo(StopPlaceEmbeddedWithCancel destination, StopPlaceEmbedded differingDestination, StopPlaceEmbedded differingOrigin, Administration headerAdministration, int headerJourneyNumber, bool journeyCancelled, StopPlaceEmbeddedWithCancel origin, Transport transportAtStart, string type)
        {
            Destination = destination;
            DifferingDestination = differingDestination;
            DifferingOrigin = differingOrigin;
            HeaderAdministration = headerAdministration;
            HeaderJourneyNumber = headerJourneyNumber;
            JourneyCancelled = journeyCancelled;
            Origin = origin;
            TransportAtStart = transportAtStart;
            Type = type;
        }

        public required StopPlaceEmbeddedWithCancel Destination { get; set; }
        public required StopPlaceEmbedded DifferingDestination { get; set; }
        public required StopPlaceEmbedded DifferingOrigin { get; set; }
        public required Administration HeaderAdministration { get; set; }
        public required int HeaderJourneyNumber { get; set; }
        public required bool JourneyCancelled { get; set; }
        public required StopPlaceEmbeddedWithCancel Origin { get; set; }
        public required Transport TransportAtStart { get; set; }
        public required string Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("destination", Destination);
            yield return ("differingDestination", DifferingDestination);
            yield return ("differingOrigin", DifferingOrigin);
            yield return ("headerAdministration", HeaderAdministration);
            yield return ("headerJourneyNumber", HeaderJourneyNumber);
            yield return ("journeyCancelled", JourneyCancelled);
            yield return ("origin", Origin);
            yield return ("transportAtStart", TransportAtStart);
            yield return ("type", Type);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyMatchByEventsRequest : __ICanIterate
    {
        public JourneyMatchByEventsRequest() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyMatchByEventsRequest(System.Collections.Generic.List<JourneyMatchJourneyEvent> matchEvents, string requestID, System.Collections.Generic.List<string> transportTypes)
        {
            MatchEvents = matchEvents;
            RequestID = requestID;
            TransportTypes = transportTypes;
        }

        public required System.Collections.Generic.List<JourneyMatchJourneyEvent> MatchEvents { get; set; }
        public required string RequestID { get; set; }
        public required System.Collections.Generic.List<string> TransportTypes { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("matchEvents", MatchEvents);
            yield return ("requestID", RequestID);
            yield return ("transportTypes", TransportTypes);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyMatchByStartEndRequest : __ICanIterate
    {
        public JourneyMatchByStartEndRequest() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyMatchByStartEndRequest(JourneyMatchJourneyStartEnd matchStartEnd, string requestID, System.Collections.Generic.List<string> transportTypes)
        {
            MatchStartEnd = matchStartEnd;
            RequestID = requestID;
            TransportTypes = transportTypes;
        }

        public required JourneyMatchJourneyStartEnd MatchStartEnd { get; set; }
        public required string RequestID { get; set; }
        public required System.Collections.Generic.List<string> TransportTypes { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("matchStartEnd", MatchStartEnd);
            yield return ("requestID", RequestID);
            yield return ("transportTypes", TransportTypes);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyMatchJourneyEvent : __ICanIterate
    {
        public JourneyMatchJourneyEvent() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyMatchJourneyEvent(string administrationID, string category, System.DateTime date, int journeyNumber, string line, JourneyMatchStopPlace stopPlace, string time)
        {
            AdministrationID = administrationID;
            Category = category;
            Date = date;
            JourneyNumber = journeyNumber;
            Line = line;
            StopPlace = stopPlace;
            Time = time;
        }

        public required string AdministrationID { get; set; }
        public required string Category { get; set; }
        public required System.DateTime Date { get; set; }
        public required int JourneyNumber { get; set; }
        public required string Line { get; set; }
        public required JourneyMatchStopPlace StopPlace { get; set; }
        public required string Time { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("administrationID", AdministrationID);
            yield return ("category", Category);
            yield return ("date", Date);
            yield return ("journeyNumber", JourneyNumber);
            yield return ("line", Line);
            yield return ("stopPlace", StopPlace);
            yield return ("time", Time);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyMatchJourneyEventResult : __ICanIterate
    {
        public JourneyMatchJourneyEventResult() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyMatchJourneyEventResult(string arrivalOrDepartureID, int index, System.DateTime scheduledTime, JourneyMatchStopPlace stopPlace, EventType type)
        {
            ArrivalOrDepartureID = arrivalOrDepartureID;
            Index = index;
            ScheduledTime = scheduledTime;
            StopPlace = stopPlace;
            Type = type;
        }

        public required string ArrivalOrDepartureID { get; set; }
        public required int Index { get; set; }
        public required System.DateTime ScheduledTime { get; set; }
        public required JourneyMatchStopPlace StopPlace { get; set; }
        public required EventType Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("arrivalOrDepartureID", ArrivalOrDepartureID);
            yield return ("index", Index);
            yield return ("scheduledTime", ScheduledTime);
            yield return ("stopPlace", StopPlace);
            yield return ("type", Type switch
            {
                EventType.ARRIVAL => "ARRIVAL",
                EventType.DEPARTURE => "DEPARTURE",
                _ => null
            });
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyMatchJourneyStartEnd : __ICanIterate
    {
        public JourneyMatchJourneyStartEnd() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyMatchJourneyStartEnd(JourneyMatchStopPlace endStopPlace, System.DateTime endTime, string headerAdministrationID, string startAdministrationID, string startCategory, int startJourneyNumber, JourneyMatchStopPlace startStopPlace, System.DateTime startTime)
        {
            EndStopPlace = endStopPlace;
            EndTime = endTime;
            HeaderAdministrationID = headerAdministrationID;
            StartAdministrationID = startAdministrationID;
            StartCategory = startCategory;
            StartJourneyNumber = startJourneyNumber;
            StartStopPlace = startStopPlace;
            StartTime = startTime;
        }

        public required JourneyMatchStopPlace EndStopPlace { get; set; }
        public required System.DateTime EndTime { get; set; }
        public required string HeaderAdministrationID { get; set; }
        public required string StartAdministrationID { get; set; }
        public required string StartCategory { get; set; }
        public required int StartJourneyNumber { get; set; }
        public required JourneyMatchStopPlace StartStopPlace { get; set; }
        public required System.DateTime StartTime { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("endStopPlace", EndStopPlace);
            yield return ("endTime", EndTime);
            yield return ("headerAdministrationID", HeaderAdministrationID);
            yield return ("startAdministrationID", StartAdministrationID);
            yield return ("startCategory", StartCategory);
            yield return ("startJourneyNumber", StartJourneyNumber);
            yield return ("startStopPlace", StartStopPlace);
            yield return ("startTime", StartTime);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyMatchResponse : __ICanIterate
    {
        public JourneyMatchResponse() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyMatchResponse(System.Collections.Generic.List<JourneyMatchJourneyEventResult> events, string journeyID, string requestID)
        {
            Events = events;
            JourneyID = journeyID;
            RequestID = requestID;
        }

        public required System.Collections.Generic.List<JourneyMatchJourneyEventResult> Events { get; set; }
        public required string JourneyID { get; set; }
        public required string RequestID { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("events", Events);
            yield return ("journeyID", JourneyID);
            yield return ("requestID", RequestID);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyMatchStopPlace : __ICanIterate
    {
        public JourneyMatchStopPlace() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyMatchStopPlace(string evaNumber, string rl100)
        {
            EvaNumber = evaNumber;
            Rl100 = rl100;
        }

        public required string EvaNumber { get; set; }
        public required string Rl100 { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("evaNumber", EvaNumber);
            yield return ("rl100", Rl100);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class JourneyRelation : __ICanIterate
    {
        public JourneyRelation() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public JourneyRelation(string endEvaNumber, System.DateTime endTime, string headerAdministrationID, int headerJourneyNumber, string startAdministrationID, string startCategory, string startEvaNumber, int startJourneyNumber, System.DateTime startTime)
        {
            EndEvaNumber = endEvaNumber;
            EndTime = endTime;
            HeaderAdministrationID = headerAdministrationID;
            HeaderJourneyNumber = headerJourneyNumber;
            StartAdministrationID = startAdministrationID;
            StartCategory = startCategory;
            StartEvaNumber = startEvaNumber;
            StartJourneyNumber = startJourneyNumber;
            StartTime = startTime;
        }

        public required string EndEvaNumber { get; set; }
        public required System.DateTime EndTime { get; set; }
        public required string HeaderAdministrationID { get; set; }
        public required int HeaderJourneyNumber { get; set; }
        public required string StartAdministrationID { get; set; }
        public required string StartCategory { get; set; }
        public required string StartEvaNumber { get; set; }
        public required int StartJourneyNumber { get; set; }
        public required System.DateTime StartTime { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("endEvaNumber", EndEvaNumber);
            yield return ("endTime", EndTime);
            yield return ("headerAdministrationID", HeaderAdministrationID);
            yield return ("headerJourneyNumber", HeaderJourneyNumber);
            yield return ("startAdministrationID", StartAdministrationID);
            yield return ("startCategory", StartCategory);
            yield return ("startEvaNumber", StartEvaNumber);
            yield return ("startJourneyNumber", StartJourneyNumber);
            yield return ("startTime", StartTime);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class MessageAttribute : __ICanIterate
    {
        public MessageAttribute() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public MessageAttribute(string code, int displayPriority, int displayPriorityDetail, int messageID, string text)
        {
            Code = code;
            DisplayPriority = displayPriority;
            DisplayPriorityDetail = displayPriorityDetail;
            MessageID = messageID;
            Text = text;
        }

        public required string Code { get; set; }
        public required int DisplayPriority { get; set; }
        public required int DisplayPriorityDetail { get; set; }
        public required int MessageID { get; set; }
        public required string Text { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("code", Code);
            yield return ("displayPriority", DisplayPriority);
            yield return ("displayPriorityDetail", DisplayPriorityDetail);
            yield return ("messageID", MessageID);
            yield return ("text", Text);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class MessageDisruptionCommunication : __ICanIterate
    {
        public MessageDisruptionCommunication() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public MessageDisruptionCommunication(string cause, int displayPriority, string disruptionCommunicationID, string disruptionID, string effect, bool hasAlternatives, DisruptionCommunicationDescription langCs, DisruptionCommunicationDescription langDa, DisruptionCommunicationDescription langDe, DisruptionCommunicationDescription langEn, DisruptionCommunicationDescription langEs, DisruptionCommunicationDescription langFr, DisruptionCommunicationDescription langIt, DisruptionCommunicationDescription langNl, DisruptionCommunicationDescription langPl, int messageID)
        {
            Cause = cause;
            DisplayPriority = displayPriority;
            DisruptionCommunicationID = disruptionCommunicationID;
            DisruptionID = disruptionID;
            Effect = effect;
            HasAlternatives = hasAlternatives;
            LangCs = langCs;
            LangDa = langDa;
            LangDe = langDe;
            LangEn = langEn;
            LangEs = langEs;
            LangFr = langFr;
            LangIt = langIt;
            LangNl = langNl;
            LangPl = langPl;
            MessageID = messageID;
        }

        public required string Cause { get; set; }
        public required int DisplayPriority { get; set; }
        public required string DisruptionCommunicationID { get; set; }
        public required string DisruptionID { get; set; }
        public required string Effect { get; set; }
        public required bool HasAlternatives { get; set; }
        public required DisruptionCommunicationDescription LangCs { get; set; }
        public required DisruptionCommunicationDescription LangDa { get; set; }
        public required DisruptionCommunicationDescription LangDe { get; set; }
        public required DisruptionCommunicationDescription LangEn { get; set; }
        public required DisruptionCommunicationDescription LangEs { get; set; }
        public required DisruptionCommunicationDescription LangFr { get; set; }
        public required DisruptionCommunicationDescription LangIt { get; set; }
        public required DisruptionCommunicationDescription LangNl { get; set; }
        public required DisruptionCommunicationDescription LangPl { get; set; }
        public required int MessageID { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("cause", Cause);
            yield return ("displayPriority", DisplayPriority);
            yield return ("disruptionCommunicationID", DisruptionCommunicationID);
            yield return ("disruptionID", DisruptionID);
            yield return ("effect", Effect);
            yield return ("hasAlternatives", HasAlternatives);
            yield return ("langCs", LangCs);
            yield return ("langDa", LangDa);
            yield return ("langDe", LangDe);
            yield return ("langEn", LangEn);
            yield return ("langEs", LangEs);
            yield return ("langFr", LangFr);
            yield return ("langIt", LangIt);
            yield return ("langNl", LangNl);
            yield return ("langPl", LangPl);
            yield return ("messageID", MessageID);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class MessageNote : __ICanIterate
    {
        public MessageNote() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public MessageNote(string category, string code, string disruptionCommunicationID, MessageNoteDescription langCs, MessageNoteDescription langDa, MessageNoteDescription langDe, MessageNoteDescription langEn, MessageNoteDescription langEs, MessageNoteDescription langFr, MessageNoteDescription langIt, MessageNoteDescription langNl, MessageNoteDescription langPl, int messageID, string text, string textShort)
        {
            Category = category;
            Code = code;
            DisruptionCommunicationID = disruptionCommunicationID;
            LangCs = langCs;
            LangDa = langDa;
            LangDe = langDe;
            LangEn = langEn;
            LangEs = langEs;
            LangFr = langFr;
            LangIt = langIt;
            LangNl = langNl;
            LangPl = langPl;
            MessageID = messageID;
            Text = text;
            TextShort = textShort;
        }

        public required string Category { get; set; }
        public required string Code { get; set; }
        public required string DisruptionCommunicationID { get; set; }
        public required MessageNoteDescription LangCs { get; set; }
        public required MessageNoteDescription LangDa { get; set; }
        public required MessageNoteDescription LangDe { get; set; }
        public required MessageNoteDescription LangEn { get; set; }
        public required MessageNoteDescription LangEs { get; set; }
        public required MessageNoteDescription LangFr { get; set; }
        public required MessageNoteDescription LangIt { get; set; }
        public required MessageNoteDescription LangNl { get; set; }
        public required MessageNoteDescription LangPl { get; set; }
        public required int MessageID { get; set; }
        public required string Text { get; set; }
        public required string TextShort { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("category", Category);
            yield return ("code", Code);
            yield return ("disruptionCommunicationID", DisruptionCommunicationID);
            yield return ("langCs", LangCs);
            yield return ("langDa", LangDa);
            yield return ("langDe", LangDe);
            yield return ("langEn", LangEn);
            yield return ("langEs", LangEs);
            yield return ("langFr", LangFr);
            yield return ("langIt", LangIt);
            yield return ("langNl", LangNl);
            yield return ("langPl", LangPl);
            yield return ("messageID", MessageID);
            yield return ("text", Text);
            yield return ("textShort", TextShort);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class MessageNoteDescription : __ICanIterate
    {
        public MessageNoteDescription() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public MessageNoteDescription(System.Collections.Generic.List<DisruptionCommunicationAttachment> attachments, System.Collections.Generic.List<DisruptionCommunicationImage> images, System.Collections.Generic.List<DisruptionCommunicationLink> links, string text, string textShort)
        {
            Attachments = attachments;
            Images = images;
            Links = links;
            Text = text;
            TextShort = textShort;
        }

        public required System.Collections.Generic.List<DisruptionCommunicationAttachment> Attachments { get; set; }
        public required System.Collections.Generic.List<DisruptionCommunicationImage> Images { get; set; }
        public required System.Collections.Generic.List<DisruptionCommunicationLink> Links { get; set; }
        public required string Text { get; set; }
        public required string TextShort { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("attachments", Attachments);
            yield return ("images", Images);
            yield return ("links", Links);
            yield return ("text", Text);
            yield return ("textShort", TextShort);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class MessageRisCauseCode : __ICanIterate
    {
        public MessageRisCauseCode() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public MessageRisCauseCode(string code, int messageID, string text)
        {
            Code = code;
            MessageID = messageID;
            Text = text;
        }

        public required string Code { get; set; }
        public required int MessageID { get; set; }
        public required string Text { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("code", Code);
            yield return ("messageID", MessageID);
            yield return ("text", Text);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class MessageRisQualityDeviation : __ICanIterate
    {
        public MessageRisQualityDeviation() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public MessageRisQualityDeviation(string code, int messageID, string text)
        {
            Code = code;
            MessageID = messageID;
            Text = text;
        }

        public required string Code { get; set; }
        public required int MessageID { get; set; }
        public required string Text { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("code", Code);
            yield return ("messageID", MessageID);
            yield return ("text", Text);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Messages : __ICanIterate
    {
        public Messages() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Messages(System.Collections.Generic.List<MessageAttribute> attributes, System.Collections.Generic.List<MessageDisruptionCommunication> disruptions, System.Collections.Generic.List<MessageNote> notes, System.Collections.Generic.List<MessageRisCauseCode> risCauseCodes, System.Collections.Generic.List<MessageRisQualityDeviation> risQualityDeviations)
        {
            Attributes = attributes;
            Disruptions = disruptions;
            Notes = notes;
            RisCauseCodes = risCauseCodes;
            RisQualityDeviations = risQualityDeviations;
        }

        public required System.Collections.Generic.List<MessageAttribute> Attributes { get; set; }
        public required System.Collections.Generic.List<MessageDisruptionCommunication> Disruptions { get; set; }
        public required System.Collections.Generic.List<MessageNote> Notes { get; set; }
        public required System.Collections.Generic.List<MessageRisCauseCode> RisCauseCodes { get; set; }
        public required System.Collections.Generic.List<MessageRisQualityDeviation> RisQualityDeviations { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("attributes", Attributes);
            yield return ("disruptions", Disruptions);
            yield return ("notes", Notes);
            yield return ("risCauseCodes", RisCauseCodes);
            yield return ("risQualityDeviations", RisQualityDeviations);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class MetaLastChangedTimestamp : __ICanIterate
    {
        public MetaLastChangedTimestamp() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public MetaLastChangedTimestamp(string journeyID, System.DateTime metaLastChangedTimestamp)
        {
            JourneyID = journeyID;
            LastChangedTimestamp = metaLastChangedTimestamp;
        }

        public required string JourneyID { get; set; }
        public required System.DateTime LastChangedTimestamp { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("journeyID", JourneyID);
            yield return ("metaLastChangedTimestamp", LastChangedTimestamp);
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
    public sealed class StopPlaceDifferingInJourney : __ICanIterate
    {
        public StopPlaceDifferingInJourney() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlaceDifferingInJourney(string evaNumber, string ifopt, string name)
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
    public sealed class StopPlaceEmbeddedWithCancel : __ICanIterate
    {
        public StopPlaceEmbeddedWithCancel() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlaceEmbeddedWithCancel(bool cancelled, string evaNumber, string ifopt, string name)
        {
            Cancelled = cancelled;
            EvaNumber = evaNumber;
            Ifopt = ifopt;
            Name = name;
        }

        public required bool Cancelled { get; set; }
        public required string EvaNumber { get; set; }
        public required string Ifopt { get; set; }
        public required string Name { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("cancelled", Cancelled);
            yield return ("evaNumber", EvaNumber);
            yield return ("ifopt", Ifopt);
            yield return ("name", Name);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class StopPlaceInJourney : __ICanIterate
    {
        public StopPlaceInJourney() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public StopPlaceInJourney(StopPlaceDifferingInJourney differingStopPlace, string evaNumber, string ifopt, string name)
        {
            DifferingStopPlace = differingStopPlace;
            EvaNumber = evaNumber;
            Ifopt = ifopt;
            Name = name;
        }

        public required StopPlaceDifferingInJourney DifferingStopPlace { get; set; }
        public required string EvaNumber { get; set; }
        public required string Ifopt { get; set; }
        public required string Name { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("differingStopPlace", DifferingStopPlace);
            yield return ("evaNumber", EvaNumber);
            yield return ("ifopt", Ifopt);
            yield return ("name", Name);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class Transport : __ICanIterate
    {
        public Transport() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public Transport(Administration administration, string category, string categoryInternal, string dfid, string dtid, string journeyDescription, int journeyNumber, string label, string line, string type)
        {
            Administration = administration;
            Category = category;
            CategoryInternal = categoryInternal;
            Dfid = dfid;
            Dtid = dtid;
            JourneyDescription = journeyDescription;
            JourneyNumber = journeyNumber;
            Label = label;
            Line = line;
            Type = type;
        }

        public required Administration Administration { get; set; }
        public required string Category { get; set; }
        public required string CategoryInternal { get; set; }
        public required string Dfid { get; set; }
        public required string Dtid { get; set; }
        public required string JourneyDescription { get; set; }
        public required int JourneyNumber { get; set; }
        public required string Label { get; set; }
        public required string Line { get; set; }
        public required string Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("administration", Administration);
            yield return ("category", Category);
            yield return ("categoryInternal", CategoryInternal);
            yield return ("dfid", Dfid);
            yield return ("dtid", Dtid);
            yield return ("journeyDescription", JourneyDescription);
            yield return ("journeyNumber", JourneyNumber);
            yield return ("label", Label);
            yield return ("line", Line);
            yield return ("type", Type);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class TransportDestinationContinuationRef : __ICanIterate
    {
        public TransportDestinationContinuationRef() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public TransportDestinationContinuationRef(string category, string categoryInternal, StopPlaceEmbeddedWithCancel destination, string dfid, ContinuationInfo differingContinuation, StopPlaceEmbedded differingDestination, string dtid, string journeyDescription, string journeyID, int journeyNumber, string label, string line, string type)
        {
            Category = category;
            CategoryInternal = categoryInternal;
            Destination = destination;
            Dfid = dfid;
            DifferingContinuation = differingContinuation;
            DifferingDestination = differingDestination;
            Dtid = dtid;
            JourneyDescription = journeyDescription;
            JourneyID = journeyID;
            JourneyNumber = journeyNumber;
            Label = label;
            Line = line;
            Type = type;
        }

        public required string Category { get; set; }
        public required string CategoryInternal { get; set; }
        public required StopPlaceEmbeddedWithCancel Destination { get; set; }
        public required string Dfid { get; set; }
        public required ContinuationInfo DifferingContinuation { get; set; }
        public required StopPlaceEmbedded DifferingDestination { get; set; }
        public required string Dtid { get; set; }
        public required string JourneyDescription { get; set; }
        public required string JourneyID { get; set; }
        public required int JourneyNumber { get; set; }
        public required string Label { get; set; }
        public required string Line { get; set; }
        public required string Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("category", Category);
            yield return ("categoryInternal", CategoryInternal);
            yield return ("destination", Destination);
            yield return ("dfid", Dfid);
            yield return ("differingContinuation", DifferingContinuation);
            yield return ("differingDestination", DifferingDestination);
            yield return ("dtid", Dtid);
            yield return ("journeyDescription", JourneyDescription);
            yield return ("journeyID", JourneyID);
            yield return ("journeyNumber", JourneyNumber);
            yield return ("label", Label);
            yield return ("line", Line);
            yield return ("type", Type);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class TransportDestinationPortionWorkingRef : __ICanIterate
    {
        public TransportDestinationPortionWorkingRef() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public TransportDestinationPortionWorkingRef(string category, string categoryInternal, StopPlaceEmbeddedWithCancel destination, string dfid, StopPlaceEmbedded differingDestination, string dtid, string journeyDescription, string journeyID, int journeyNumber, string label, string line, StopPlaceEmbedded separationAt, string type)
        {
            Category = category;
            CategoryInternal = categoryInternal;
            Destination = destination;
            Dfid = dfid;
            DifferingDestination = differingDestination;
            Dtid = dtid;
            JourneyDescription = journeyDescription;
            JourneyID = journeyID;
            JourneyNumber = journeyNumber;
            Label = label;
            Line = line;
            SeparationAt = separationAt;
            Type = type;
        }

        public required string Category { get; set; }
        public required string CategoryInternal { get; set; }
        public required StopPlaceEmbeddedWithCancel Destination { get; set; }
        public required string Dfid { get; set; }
        public required StopPlaceEmbedded DifferingDestination { get; set; }
        public required string Dtid { get; set; }
        public required string JourneyDescription { get; set; }
        public required string JourneyID { get; set; }
        public required int JourneyNumber { get; set; }
        public required string Label { get; set; }
        public required string Line { get; set; }
        public required StopPlaceEmbedded SeparationAt { get; set; }
        public required string Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("category", Category);
            yield return ("categoryInternal", CategoryInternal);
            yield return ("destination", Destination);
            yield return ("dfid", Dfid);
            yield return ("differingDestination", DifferingDestination);
            yield return ("dtid", Dtid);
            yield return ("journeyDescription", JourneyDescription);
            yield return ("journeyID", JourneyID);
            yield return ("journeyNumber", JourneyNumber);
            yield return ("label", Label);
            yield return ("line", Line);
            yield return ("separationAt", SeparationAt);
            yield return ("type", Type);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class TransportDestinationRef : __ICanIterate
    {
        public TransportDestinationRef() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public TransportDestinationRef(string category, string categoryInternal, StopPlaceEmbeddedWithCancel destination, string dfid, StopPlaceEmbedded differingDestination, string dtid, string journeyDescription, string journeyID, int journeyNumber, string label, string line, string type)
        {
            Category = category;
            CategoryInternal = categoryInternal;
            Destination = destination;
            Dfid = dfid;
            DifferingDestination = differingDestination;
            Dtid = dtid;
            JourneyDescription = journeyDescription;
            JourneyID = journeyID;
            JourneyNumber = journeyNumber;
            Label = label;
            Line = line;
            Type = type;
        }

        public required string Category { get; set; }
        public required string CategoryInternal { get; set; }
        public required StopPlaceEmbeddedWithCancel Destination { get; set; }
        public required string Dfid { get; set; }
        public required StopPlaceEmbedded DifferingDestination { get; set; }
        public required string Dtid { get; set; }
        public required string JourneyDescription { get; set; }
        public required string JourneyID { get; set; }
        public required int JourneyNumber { get; set; }
        public required string Label { get; set; }
        public required string Line { get; set; }
        public required string Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("category", Category);
            yield return ("categoryInternal", CategoryInternal);
            yield return ("destination", Destination);
            yield return ("dfid", Dfid);
            yield return ("differingDestination", DifferingDestination);
            yield return ("dtid", Dtid);
            yield return ("journeyDescription", JourneyDescription);
            yield return ("journeyID", JourneyID);
            yield return ("journeyNumber", JourneyNumber);
            yield return ("label", Label);
            yield return ("line", Line);
            yield return ("type", Type);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class TransportOriginContinuationRef : __ICanIterate
    {
        public TransportOriginContinuationRef() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public TransportOriginContinuationRef(string category, string categoryInternal, string dfid, ContinuationInfo differingContinuation, StopPlaceEmbedded differingOrigin, string dtid, string journeyDescription, string journeyID, int journeyNumber, string label, string line, StopPlaceEmbeddedWithCancel origin, string type)
        {
            Category = category;
            CategoryInternal = categoryInternal;
            Dfid = dfid;
            DifferingContinuation = differingContinuation;
            DifferingOrigin = differingOrigin;
            Dtid = dtid;
            JourneyDescription = journeyDescription;
            JourneyID = journeyID;
            JourneyNumber = journeyNumber;
            Label = label;
            Line = line;
            Origin = origin;
            Type = type;
        }

        public required string Category { get; set; }
        public required string CategoryInternal { get; set; }
        public required string Dfid { get; set; }
        public required ContinuationInfo DifferingContinuation { get; set; }
        public required StopPlaceEmbedded DifferingOrigin { get; set; }
        public required string Dtid { get; set; }
        public required string JourneyDescription { get; set; }
        public required string JourneyID { get; set; }
        public required int JourneyNumber { get; set; }
        public required string Label { get; set; }
        public required string Line { get; set; }
        public required StopPlaceEmbeddedWithCancel Origin { get; set; }
        public required string Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("category", Category);
            yield return ("categoryInternal", CategoryInternal);
            yield return ("dfid", Dfid);
            yield return ("differingContinuation", DifferingContinuation);
            yield return ("differingOrigin", DifferingOrigin);
            yield return ("dtid", Dtid);
            yield return ("journeyDescription", JourneyDescription);
            yield return ("journeyID", JourneyID);
            yield return ("journeyNumber", JourneyNumber);
            yield return ("label", Label);
            yield return ("line", Line);
            yield return ("origin", Origin);
            yield return ("type", Type);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("dotnet-openapi-generator", "10.0.0-preview.17+fdb6bff775a5f7dd92355871c893ded52c7af04b")]
    public sealed class TransportWithDirection : __ICanIterate
    {
        public TransportWithDirection() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Text.Json.Serialization.JsonConstructor]
        public TransportWithDirection(Administration administration, string category, string categoryInternal, string dfid, DirectionInfo direction, string dtid, string journeyDescription, int journeyNumber, string label, string line, ReplacementTransport replacementTransport, string type)
        {
            Administration = administration;
            Category = category;
            CategoryInternal = categoryInternal;
            Dfid = dfid;
            Direction = direction;
            Dtid = dtid;
            JourneyDescription = journeyDescription;
            JourneyNumber = journeyNumber;
            Label = label;
            Line = line;
            ReplacementTransport = replacementTransport;
            Type = type;
        }

        public required Administration Administration { get; set; }
        public required string Category { get; set; }
        public required string CategoryInternal { get; set; }
        public required string Dfid { get; set; }
        public required DirectionInfo Direction { get; set; }
        public required string Dtid { get; set; }
        public required string JourneyDescription { get; set; }
        public required int JourneyNumber { get; set; }
        public required string Label { get; set; }
        public required string Line { get; set; }
        public required ReplacementTransport ReplacementTransport { get; set; }
        public required string Type { get; set; }

        System.Collections.Generic.IEnumerable<(string name, object? value)> __ICanIterate.IterateProperties()
        {
            yield return ("administration", Administration);
            yield return ("category", Category);
            yield return ("categoryInternal", CategoryInternal);
            yield return ("dfid", Dfid);
            yield return ("direction", Direction);
            yield return ("dtid", Dtid);
            yield return ("journeyDescription", JourneyDescription);
            yield return ("journeyNumber", JourneyNumber);
            yield return ("label", Label);
            yield return ("line", Line);
            yield return ("replacementTransport", ReplacementTransport);
            yield return ("type", Type);
        }
    }
}
