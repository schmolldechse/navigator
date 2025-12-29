using AutoMapper;
using Navigator.Api.DTOs.Journey;
using Navigator.Api.DTOs.Journey.Message;
using Navigator.Api.Enums;
using Navigator.Data.Enums;
using Navigator.Data.Models.Ris;

namespace Navigator.Api.Mapping;

public class JourneyProfile : Profile
{
    public JourneyProfile()
    {
        CreateMap<RisJourneys.JourneyInfo, JourneyAdministration>()
            .ForMember(dest => dest.AdministrationId,
                opt => opt.MapFrom(src => src.HeaderAdministration.AdministrationID))
            .ForMember(dest => dest.OperatorCode, opt => opt.MapFrom(src => src.HeaderAdministration.OperatorCode))
            .ForMember(dest => dest.OperatorName, opt => opt.MapFrom(src => src.HeaderAdministration.OperatorName));

        CreateMap<RisJourneys.Transport, JourneyTransport>()
            .ForMember(dest => dest.Type,
                opt => opt.MapFrom(src => TransportTypeConverter.StringTransportToNavigatorTransport(src.Type)))
            .ForMember(dest => dest.ReplacementType, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.CategoryInternal, opt => opt.MapFrom(src => src.CategoryInternal))
            .ForMember(dest => dest.JourneyDescription, opt => opt.MapFrom(src => src.JourneyDescription))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.JourneyNumber))
            .ForMember(dest => dest.Line, opt => opt.MapFrom(src => src.Line));

        CreateMap<RisJourneys.StopPlaceEmbedded, JourneyStopPlace>()
            .ForMember(dest => dest.EvaNumber, opt => opt.MapFrom(src => src.EvaNumber))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        
        CreateMap<RisJourneys.StopPlaceEmbeddedWithCancel, JourneyRichStopPlace>()
            .ForMember(dest => dest.EvaNumber, opt => opt.MapFrom(src => src.EvaNumber))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Cancelled, opt => opt.MapFrom(src => src.Cancelled));

        CreateMap<RisJourneys.StopPlaceInJourney, JourneyStopPlace>()
            .ForMember(dest => dest.EvaNumber, opt => opt.MapFrom(src => src.EvaNumber))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        
        CreateMap<RisJourneys.StopPlaceDifferingInJourney, JourneyStopPlace>()
            .ForMember(dest => dest.EvaNumber, opt => opt.MapFrom(src => src.EvaNumber))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<RisJourneys.JourneyEvent, JourneyScheduledEvent>()
            .ForMember(dest => dest.StopPlace, opt => opt.MapFrom(src => src.StopPlace))
            .ForMember(dest => dest.DifferingStopPlace, opt => opt.MapFrom(src => src.StopPlace.DifferingStopPlace))
            .ForMember(dest => dest.Cancelled, opt => opt.MapFrom(src => src.Cancelled))
            .ForMember(dest => dest.Additional, opt => opt.MapFrom(src => src.Additional))
            .ForMember(dest => dest.NoPassengerChange, opt => opt.MapFrom(src => src.NoPassengerChange))
            .ForMember(dest => dest.Demand, opt => opt.MapFrom(src => src.OnDemand))
            .ForMember(dest => dest.ScheduleType, opt => opt.MapFrom(src => MapScheduleOrThrow(src.Type)))
            .ForMember(dest => dest.PlannedTime, opt => opt.MapFrom(src => src.TimeSchedule))
            .ForMember(dest => dest.ActualTime, opt => opt.MapFrom(src => src.Time))
            .ForMember(dest => dest.Delay, opt => opt.Ignore())
            .ForMember(dest => dest.PlannedPlatform, opt => opt.MapFrom(src => src.PlatformSchedule))
            .ForMember(dest => dest.ActualPlatform, opt => opt.MapFrom(src => src.Platform))
            .ForMember(dest => dest.TimeType,
                opt => opt.MapFrom(src => TimeTypeConverter.StringToNavigatorTime(src.TimeType)))
            .ForMember(dest => dest.TravelsWith,
                opt => opt.MapFrom(src => src.TravelsWith.Select(travelWith => travelWith.JourneyID)))
            .ForMember(dest => dest.MessageIds, opt => opt.MapFrom(src => src.Messages))
            .AfterMap((src, dest) => dest.Delay = (int)(dest.ActualTime - dest.PlannedTime).TotalSeconds);
        
        CreateMap<RisJourneys.JourneyEventBased, Journey>()
            .ForMember(dest => dest.JourneyId, opt => opt.MapFrom(src => src.JourneyID))
            .ForMember(dest => dest.Administration, opt => opt.MapFrom(src => src.Info))
            .ForMember(dest => dest.Transport, opt => opt.MapFrom(src => src.Info.TransportAtStart))
            .ForMember(dest => dest.ContinuationBy, opt => opt.MapFrom(src => src.ContinuationBy.Select(continuation => continuation.JourneyID)))
            .ForMember(dest => dest.ContinuationFor, opt => opt.MapFrom(src => src.ContinuationFor.Select(continuation => continuation.JourneyID)))
            .ForMember(dest => dest.Cancelled, opt => opt.MapFrom(src => src.Info.JourneyCancelled))
            .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => src.Info.Destination))
            .ForMember(dest => dest.DifferingDestination, opt => opt.MapFrom(src => src.Info.DifferingDestination))
            .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src.Info.Origin))
            .ForMember(dest => dest.DifferingOrigin, opt => opt.MapFrom(src => src.Info.DifferingOrigin))
            .ForMember(dest => dest.ScheduledEvents, opt => opt.MapFrom(src => src.Events))
            .ForMember(dest => dest.Messages, opt => opt.Ignore())
            .AfterMap((src, dest, context) =>
            {
                TransportType? replacementTransportType = null;
                var journeyEventReplacements = src.Events
                    .Where(journeyEvent => journeyEvent.Transport != null && journeyEvent.Transport.ReplacementTransport != null && !string.IsNullOrEmpty(journeyEvent.Transport.ReplacementTransport.RealType))
                    .Select(journeyEvent => TransportTypeConverter.StringTransportToNavigatorTransport(journeyEvent.Transport.ReplacementTransport.RealType))
                    .OfType<TransportType>()
                    .ToList();
                if (journeyEventReplacements.Any()) replacementTransportType = journeyEventReplacements
                    .GroupBy(transport => transport)
                    .OrderByDescending(grouping => grouping.Count())
                    .Select(grouping => grouping.Key)
                    .First();
                
                dest.Transport.ReplacementType = replacementTransportType;
                
                dest.Messages = new List<JourneyMessage>();
                void MapMessages<TSource, TTarget>(IEnumerable<TSource>? sourceList) where TTarget : JourneyMessage
                {
                    if (sourceList is null) return;

                    foreach (var item in sourceList)
                    {
                        var mappedMessage = context.Mapper.Map<TTarget>(item);
                        dest.Messages.Add(mappedMessage);
                    }
                }

                if (src.Messages is not null)
                {
                    MapMessages<RisJourneys.MessageAttribute, AttributeMessage>(src.Messages.Attributes);
                    MapMessages<RisJourneys.MessageDisruptionCommunication, DisruptionMessage>(src.Messages.Disruptions);
                    MapMessages<RisJourneys.MessageNote, NoteMessage>(src.Messages.Notes);
                    MapMessages<RisJourneys.MessageRisCauseCode, RisCauseMessage>(src.Messages.RisCauseCodes);
                    MapMessages<RisJourneys.MessageRisQualityDeviation, RisQualityDeviationMessage>(src.Messages.RisQualityDeviations);
                }
            });
        
        /// JourneyMessages
        // Attributes
        CreateMap<RisJourneys.MessageAttribute, AttributeMessage>()
            .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageID))
            .ForMember(dest => dest.Type, opt => opt.Ignore())
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => MessageKeyConverter.MapToMessageKey(src.Code)))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text));
        
        // Disruption
        CreateMap<RisJourneys.MessageDisruptionCommunication, DisruptionMessage>()
            .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageID))
            .ForMember(dest => dest.Type, opt => opt.Ignore())
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => MessageKey.UNPLANNED_INFO))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.LangDe.Text))
            .ForMember(dest => dest.TextShort, opt => opt.MapFrom(src => src.LangDe.TextShort))
            .AfterMap((src, dest, context) =>
            {
                var references = new List<JourneyMessageReference>();
                if (src.LangDe.Attachments.Any()) references.AddRange(src.LangDe.Attachments.Select(attachment => new JourneyMessageReference() {
                    ReferenceType = MessageReferenceType.Attachment, 
                    Label = attachment.Label, 
                    Url = attachment.Url
                }));
                
                if (src.LangDe.Images.Any()) references.AddRange(src.LangDe.Images.Select(image => new JourneyMessageReference() {
                    ReferenceType = MessageReferenceType.Image, 
                    Label = image.Label, 
                    Url = image.Url
                }));
                
                if (src.LangDe.Links.Any()) references.AddRange(src.LangDe.Links.Select(link => new JourneyMessageReference() {
                    ReferenceType = MessageReferenceType.Link, 
                    Label = link.Label, 
                    Url = link.Url
                }));
                
                dest.References = references;
            });
        
        // Note
        CreateMap<RisJourneys.MessageNote, NoteMessage>()
            .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageID))
            .ForMember(dest => dest.Type, opt => opt.Ignore())
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => MessageKey.UNPLANNED_INFO))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.LangDe.Text))
            .ForMember(dest => dest.TextShort, opt => opt.MapFrom(src => src.LangDe.TextShort))
            .AfterMap((src, dest, context) =>
            {
                var references = new List<JourneyMessageReference>();
                if (src.LangDe.Attachments.Any()) references.AddRange(src.LangDe.Attachments.Select(attachment => new JourneyMessageReference() {
                    ReferenceType = MessageReferenceType.Attachment, 
                    Label = attachment.Label, 
                    Url = attachment.Url
                }));
                
                if (src.LangDe.Images.Any()) references.AddRange(src.LangDe.Images.Select(image => new JourneyMessageReference() {
                    ReferenceType = MessageReferenceType.Image, 
                    Label = image.Label, 
                    Url = image.Url
                }));
                
                if (src.LangDe.Links.Any()) references.AddRange(src.LangDe.Links.Select(link => new JourneyMessageReference() {
                    ReferenceType = MessageReferenceType.Link, 
                    Label = link.Label, 
                    Url = link.Url
                }));
                
                dest.References = references;
            });
        
        // Disruption
        CreateMap<RisJourneys.MessageRisCauseCode, RisCauseMessage>()
            .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageID))
            .ForMember(dest => dest.Type, opt => opt.Ignore())
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => MessageKeyConverter.MapToMessageKey(src.Code)))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text));
        
        // Disruption
        CreateMap<RisJourneys.MessageRisQualityDeviation, RisQualityDeviationMessage>()
            .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageID))
            .ForMember(dest => dest.Type, opt => opt.Ignore())
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => MessageKeyConverter.MapToMessageKey(src.Code)))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text));
    }
    
    private Navigator.Data.Enums.ScheduleType MapScheduleOrThrow(RisJourneys.EventType eventType) => eventType switch
    {
        RisJourneys.EventType.ARRIVAL => Navigator.Data.Enums.ScheduleType.Arrival,
        RisJourneys.EventType.DEPARTURE => Navigator.Data.Enums.ScheduleType.Departure,
        _ => throw new AutoMapperMappingException($"Value '{eventType}' is not a valid ScheduleType"),
    };
}