using AutoMapper;
using Navigator.Data.Entities.Journey;
using Navigator.Data.Entities.Journey.Message;
using Navigator.Data.Enums;
using Navigator.Data.Models.Ris;
using System.Globalization;

namespace Navigator.Daemon.Mapping;

public class RisJourneysProfile : Profile
{
    public RisJourneysProfile()
    {
        CreateMap<RisJourneys.JourneyEventBased, Journey>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.JourneyID))
            .ForMember(dest => dest.Date, opt => opt.Ignore())
            .ForMember(dest => dest.Cancelled, opt => opt.MapFrom(src => src.Info.JourneyCancelled))
            .ForMember(dest => dest.Transport, opt => opt.MapFrom(src => MapJourneyTransport(src)))
            .ForMember(dest => dest.JourneyType, opt => opt.MapFrom(src => MapJourneyOrThrow(src.Info.Type)))
            .ForMember(dest => dest.InsertedAt, opt => opt.Ignore())
            .ForMember(dest => dest.StopPlaces, opt => opt.Ignore())
            .ForMember(dest => dest.Messages, opt => opt.Ignore())
            .ForMember(dest => dest.Administration, opt => opt.Ignore())
            .AfterMap((src, dest, context) =>
            {
                if (!DateOnly.TryParseExact(dest.Id[..8], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    throw new AutoMapperMappingException($"Value '{dest.Id}' is not valid to be mapped for Date");
                dest.Date = date;

                var messageLookup = new Dictionary<int, JourneyMessage>();
                dest.Messages = new List<JourneyMessage>();

                void MapMessages<TSource>(IEnumerable<TSource>? sourceList, Func<TSource, int> idSelector)
                {
                    if (sourceList is null) return;

                    foreach (var item in sourceList)
                    {
                        var mappedMessage = context.Mapper.Map<JourneyMessage>(item);
                        mappedMessage.JourneyId = dest.Id;

                        dest.Messages.Add(mappedMessage);

                        var id = idSelector(item);
                        if (!messageLookup.ContainsKey(id)) messageLookup[id] = mappedMessage;
                    }
                }

                if (src.Messages is not null)
                {
                    MapMessages(src.Messages.Attributes, message => message.MessageID);
                    MapMessages(src.Messages.Disruptions, message => message.MessageID);
                    MapMessages(src.Messages.Notes, message => message.MessageID);
                    MapMessages(src.Messages.RisCauseCodes, message => message.MessageID);
                    MapMessages(src.Messages.RisQualityDeviations, message => message.MessageID);
                }

                dest.StopPlaces = new List<JourneyStopPlace>();
                if (src.Events is not null)
                {
                    foreach (var srcEvent in src.Events)
                    {
                        var destStop = context.Mapper.Map<JourneyStopPlace>(srcEvent);
                        destStop.JourneyId = dest.Id;

                        if (srcEvent.Messages is null || !srcEvent.Messages.Any()) continue;

                        foreach (var messageId in srcEvent.Messages)
                        {
                            if (!messageLookup.TryGetValue(messageId, out var linkedMessage)) continue;
                            destStop.Messages.Add(new JourneyStopPlaceMessage()
                            {
                                StopPlace = destStop,
                                Message = linkedMessage
                            });
                        }

                        dest.StopPlaces.Add(destStop);
                    }
                }
            });

        CreateMap<RisJourneys.JourneyEvent, JourneyStopPlace>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.JourneyId, opt => opt.Ignore())
            .ForMember(dest => dest.ScheduleType, opt => opt.MapFrom(src => MapScheduleOrThrow(src.Type)))
            .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.StopPlace.Name))
            .ForMember(dest => dest.StationEvaNumber, opt => opt.MapFrom(src => ParseEvaOrThrow(src.StopPlace.EvaNumber)))
            .ForMember(dest => dest.Cancelled, opt => opt.MapFrom(src => src.Cancelled))
            .ForMember(dest => dest.Additional, opt => opt.MapFrom(src => src.Additional))
            .ForMember(dest => dest.Demand, opt => opt.MapFrom(src => src.OnDemand))
            .ForMember(dest => dest.NoPassengerChange, opt => opt.MapFrom(src => src.NoPassengerChange))
            .ForMember(dest => dest.PlannedTime, opt => opt.MapFrom(src => src.TimeSchedule))
            .ForMember(dest => dest.ActualTime, opt => opt.MapFrom(src => src.Time))
            .ForMember(dest => dest.TimeType, opt => opt.MapFrom(src => MapTimeOrThrow(src.TimeType)))
            .ForMember(dest => dest.PlannedPlatform, opt => opt.MapFrom(src => src.PlatformSchedule))
            .ForMember(dest => dest.ActualPlatform, opt => opt.MapFrom(src => src.Platform))
            .ForMember(dest => dest.Messages, opt => opt.Ignore());

        /// Journey Messages
        /// Attribute
        CreateMap<RisJourneys.MessageAttribute, JourneyMessage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.JourneyId, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MessageType.Attribute))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ForMember(dest => dest.TextShort, opt => opt.Ignore())
            .ForMember(dest => dest.DisruptionCause, opt => opt.Ignore())
            .ForMember(dest => dest.DisruptionEffect, opt => opt.Ignore())
            .ForMember(dest => dest.NoteCategory, opt => opt.Ignore())
            .ForMember(dest => dest.References, opt => opt.Ignore())
            .ForMember(dest => dest.JourneyStopPlaceMessages, opt => opt.Ignore());

        /// Disruption
        CreateMap<RisJourneys.MessageDisruptionCommunication, JourneyMessage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.JourneyId, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MessageType.Disruption))
            .ForMember(dest => dest.Code, opt => opt.Ignore())
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.LangDe.Text))
            .ForMember(dest => dest.TextShort, opt => opt.MapFrom(src => src.LangDe.TextShort))
            .ForMember(dest => dest.DisruptionCause, opt => opt.MapFrom(src => src.Cause))
            .ForMember(dest => dest.DisruptionEffect, opt => opt.MapFrom(src => src.Effect))
            .ForMember(dest => dest.NoteCategory, opt => opt.Ignore())
            .ForMember(dest => dest.References, opt => opt.MapFrom(src => MapDisruptionReferences(src)))
            .ForMember(dest => dest.JourneyStopPlaceMessages, opt => opt.Ignore());

        /// Note
        CreateMap<RisJourneys.MessageNote, JourneyMessage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.JourneyId, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MessageType.Note))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.LangDe.Text))
            .ForMember(dest => dest.TextShort, opt => opt.MapFrom(src => src.LangDe.TextShort))
            .ForMember(dest => dest.DisruptionCause, opt => opt.Ignore())
            .ForMember(dest => dest.DisruptionEffect, opt => opt.Ignore())
            .ForMember(dest => dest.NoteCategory, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.References, opt => opt.MapFrom(src => MapNoteReferences(src)))
            .ForMember(dest => dest.JourneyStopPlaceMessages, opt => opt.Ignore());

        /// RIS Cause
        CreateMap<RisJourneys.MessageRisCauseCode, JourneyMessage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.JourneyId, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MessageType.RisCause))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ForMember(dest => dest.TextShort, opt => opt.Ignore())
            .ForMember(dest => dest.DisruptionCause, opt => opt.Ignore())
            .ForMember(dest => dest.DisruptionEffect, opt => opt.Ignore())
            .ForMember(dest => dest.NoteCategory, opt => opt.Ignore())
            .ForMember(dest => dest.References, opt => opt.Ignore())
            .ForMember(dest => dest.JourneyStopPlaceMessages, opt => opt.Ignore());

        /// RIS Quality Deviation
        CreateMap<RisJourneys.MessageRisQualityDeviation, JourneyMessage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.JourneyId, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MessageType.RisQualityDeviation))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ForMember(dest => dest.TextShort, opt => opt.Ignore())
            .ForMember(dest => dest.DisruptionCause, opt => opt.Ignore())
            .ForMember(dest => dest.DisruptionEffect, opt => opt.Ignore())
            .ForMember(dest => dest.NoteCategory, opt => opt.Ignore())
            .ForMember(dest => dest.References, opt => opt.Ignore())
            .ForMember(dest => dest.JourneyStopPlaceMessages, opt => opt.Ignore());
    }
    private List<JourneyMessageReference> MapNoteReferences(RisJourneys.MessageNote source)
    {
        var references = new List<JourneyMessageReference>();
        if (source.LangDe is null) return references;

        if (source.LangDe.Links.Any())
        {
            references.AddRange(source.LangDe.Links.Select(link => new JourneyMessageReference()
            {
                ReferenceType = MessageReferenceType.Link,
                Url = link.Url,
                Label = link.Label,
                Message = null!
            }));
        }

        if (source.LangDe.Images.Any())
        {
            references.AddRange(source.LangDe.Images.Select(link => new JourneyMessageReference()
            {
                ReferenceType = MessageReferenceType.Image,
                Url = link.Url,
                Label = link.Label,
                Message = null!
            }));
        }

        if (source.LangDe.Attachments.Any())
        {
            references.AddRange(source.LangDe.Attachments.Select(link => new JourneyMessageReference()
            {
                ReferenceType = MessageReferenceType.Attachment,
                Url = link.Url,
                Label = link.Label,
                Message = null!
            }));
        }

        return references;
    }

    private List<JourneyMessageReference> MapDisruptionReferences(RisJourneys.MessageDisruptionCommunication source)
    {
        var references = new List<JourneyMessageReference>();
        if (source.LangDe is null) return references;

        if (source.LangDe.Links.Any())
        {
            references.AddRange(source.LangDe.Links.Select(link => new JourneyMessageReference()
            {
                ReferenceType = MessageReferenceType.Link,
                Url = link.Url,
                Label = link.Label,
                Message = null!
            }));
        }

        if (source.LangDe.Images.Any())
        {
            references.AddRange(source.LangDe.Images.Select(link => new JourneyMessageReference()
            {
                ReferenceType = MessageReferenceType.Image,
                Url = link.Url,
                Label = link.Label,
                Message = null!
            }));
        }

        if (source.LangDe.Attachments.Any())
        {
            references.AddRange(source.LangDe.Attachments.Select(link => new JourneyMessageReference()
            {
                ReferenceType = MessageReferenceType.Attachment,
                Url = link.Url,
                Label = link.Label,
                Message = null!
            }));
        }

        return references;
    }

    private int ParseEvaOrThrow(string? evaNumberStr)
    {
        if (!int.TryParse(evaNumberStr, out var evaNumber))
            throw new AutoMapperMappingException($"Value '{evaNumberStr}' is not a valid EvaNumber");
        return evaNumber;
    }

    private Navigator.Data.Enums.JourneyType MapJourneyOrThrow(string? journeyType) => journeyType?.ToUpperInvariant() switch
    {
        "REGULAR" => Navigator.Data.Enums.JourneyType.Regular,
        "REPLACEMENT" => Navigator.Data.Enums.JourneyType.Replacement,
        "RELIEF" => Navigator.Data.Enums.JourneyType.Relief,
        "EXTRA" => Navigator.Data.Enums.JourneyType.Extra,
        _ => throw new AutoMapperMappingException($"Value '{journeyType}' is not a valid JourneyType"),
    };

    private Navigator.Data.Enums.TimeType MapTimeOrThrow(string? timeType) => timeType?.ToUpperInvariant() switch
    {
        "SCHEDULE" => Navigator.Data.Enums.TimeType.Schedule,
        "PREVIEW" => Navigator.Data.Enums.TimeType.Preview,
        "REAL" => Navigator.Data.Enums.TimeType.Real,
        _ => throw new AutoMapperMappingException($"Value '{timeType}' is not a valid TimeType"),
    };

    private Navigator.Data.Enums.ScheduleType MapScheduleOrThrow(RisJourneys.EventType eventType) => eventType switch
    {
        RisJourneys.EventType.ARRIVAL => Navigator.Data.Enums.ScheduleType.Arrival,
        RisJourneys.EventType.DEPARTURE => Navigator.Data.Enums.ScheduleType.Departure,
        _ => throw new AutoMapperMappingException($"Value '{eventType}' is not a valid ScheduleType"),
    };

    private JourneyTransport MapJourneyTransport(RisJourneys.JourneyEventBased source)
    {
        if (source.Info.TransportAtStart == null)
            throw new AutoMapperMappingException($"Cannot map Journey {source.JourneyID}: Missing 'Info.TransportAtStart'.");

        TransportType? transportType = TransportTypeConverter.StringTransportToNavigatorTransport(source.Info.TransportAtStart.Type);
        if (transportType == null)
            throw new AutoMapperMappingException($"Cannot map Journey {source.JourneyID}: Invalid or unknown Transport Type '{source.Info.TransportAtStart.Type}'");

        TransportType? replacementTransportType = null;
        var journeyEventReplacements = source.Events
            .Where(journeyEvent => journeyEvent.Transport != null && journeyEvent.Transport.ReplacementTransport != null && !string.IsNullOrEmpty(journeyEvent.Transport.ReplacementTransport.RealType))
            .Select(journeyEvent => TransportTypeConverter.StringTransportToNavigatorTransport(journeyEvent.Transport.ReplacementTransport.RealType))
            .OfType<TransportType>()
            .ToList();
        if (journeyEventReplacements.Any()) replacementTransportType = journeyEventReplacements
            .GroupBy(transport => transport)
            .OrderByDescending(grouping => grouping.Count())
            .Select(grouping => grouping.Key)
            .First();

        return new JourneyTransport()
        {
            Id = source.JourneyID,
            TransportType = transportType!.Value,
            ReplacementTransportType = replacementTransportType,
            Category = source.Info.TransportAtStart.Category,
            CategoryInternal = source.Info.TransportAtStart.CategoryInternal,
            JourneyDescription = source.Info.TransportAtStart.JourneyDescription,
            Label = source.Info.TransportAtStart.Label,
            Number = source.Info.TransportAtStart.JourneyNumber,
            Line = source.Info.TransportAtStart.Line
        };
    }
}
