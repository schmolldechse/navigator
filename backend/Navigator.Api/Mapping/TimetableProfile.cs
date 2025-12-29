using AutoMapper;
using Navigator.Api.DTOs.Timetable;
using Navigator.Api.Enums;
using Navigator.Data.Enums;
using Navigator.Data.Models.Ris;

namespace Navigator.Api.Mapping;

public class TimetableProfile : Profile
{
    public TimetableProfile()
    {
        /// General
        CreateMap<RisBoards.Administration, TimetableEntryAdministration>()
            .ForMember(dest => dest.AdministrationId, opt => opt.MapFrom(src => src.AdministrationID))
            .ForMember(dest => dest.OperatorCode, opt => opt.MapFrom(src => src.OperatorCode))
            .ForMember(dest => dest.OperatorName, opt => opt.MapFrom(src => src.OperatorName));

        CreateMap<RisBoards.StopPlaceEmbedded, TimetableEntryStopPlace>()
            .ForMember(dest => dest.EvaNumber, opt => opt.MapFrom(src => ParseEvaOrThrow(src.EvaNumber)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<RisBoards.StopAtStopPlace, TimetableEntryRichStopPlace>()
            .ForMember(dest => dest.EvaNumber, opt => opt.MapFrom(src => ParseEvaOrThrow(src.EvaNumber)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Cancelled, opt => opt.MapFrom(src => src.Canceled));

        CreateMap<RisBoards.StopAtStopPlacePrio, TimetableEntryRichStopPlace>()
            .ForMember(dest => dest.EvaNumber, opt => opt.MapFrom(src => ParseEvaOrThrow(src.EvaNumber)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Cancelled, opt => opt.MapFrom(src => src.Canceled))
            .ForMember(dest => dest.Additional, opt => opt.MapFrom(src => src.Additional));

        /// Departure
        CreateMap<RisBoards.TransportPublicDestinationPortionWorking, TimetableEntryCoupledTransport>()
            .ForMember(dest => dest.JourneyId, opt => opt.MapFrom(src => src.JourneyID))
            .ForMember(dest => dest.SeparationAt, opt => opt.MapFrom(src => src.SeparationAt));

        CreateMap<RisBoards.TransportPublicDestinationVia, TimetableEntryTransport>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => TransportTypeConverter.BoardsTransportToNavigatorTransport(src.Type)))
            .ForMember(dest => dest.ReplacementType, opt => opt.MapFrom(src => src.ReplacementTransport != null ? TransportTypeConverter.StringTransportToNavigatorTransport(src.ReplacementTransport.RealType) : (TransportType?)null))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.CategoryInternal, opt => opt.MapFrom(src => src.CategoryInternal))
            .ForMember(dest => dest.JourneyDescription, opt => opt.MapFrom(src => src.JourneyDescription))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.Line, opt => opt.MapFrom(src => src.Line));

        CreateMap<RisBoards.StopDeparture, TimetableDeparture>()
            .ForMember(dest => dest.JourneyId, opt => opt.MapFrom(src => src.JourneyID))
            .ForMember(dest => dest.Administration, opt => opt.MapFrom(src => src.Administration))
            .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => src.Transport.Destination))
            .ForMember(dest => dest.DifferingDestination, opt => opt.MapFrom(src => src.Transport.DifferingDestination))
            .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => src.Transport.Direction.StopPlaces))
            .ForMember(dest => dest.ViaStops, opt => opt.MapFrom(src => src.Transport.Via))
            .ForMember(dest => dest.Schedule, opt => opt.MapFrom(src => new TimetableEntrySchedule()
            {
                PlannedTime = src.TimeSchedule,
                ActualTime = src.Time,
                PlannedPlatform = src.PlatformSchedule,
                ActualPlatform = src.Platform
            }))
            .ForMember(dest => dest.Informations, opt => opt.Ignore())
            .ForMember(dest => dest.Cancelled, opt => opt.MapFrom(src => src.Canceled))
            .ForMember(dest => dest.Additional, opt => opt.MapFrom(src => src.Additional))
            .ForMember(dest => dest.Demand, opt => opt.MapFrom(src => src.OnDemand))
            .ForMember(dest => dest.TravelsWith, opt => opt.MapFrom(src => src.TravelsWith))
            .AfterMap((src, dest, context) =>
            {
                dest.Transport.JourneyType = JourneyTypeConverter.MapBoardsJourneyTypeOrThrow(src.JourneyType);

                var informations = new List<TimetableEntryInformation>();

                if (src.Attributes.Any()) informations.AddRange(src.Attributes.Select(attribute => new TimetableEntryInformation()
                {
                    Type = InformationType.JourneyAttribute,
                    Key = MessageKeyConverter.MapToMessageKey(attribute.Code),
                    Text = attribute.Text,
                    TextShort = attribute.TextShort
                }));

                if (src.Disruptions.Any(disruption => disruption.Descriptions.ContainsKey("DE"))) informations.AddRange(src.Disruptions.Select(disruption => new TimetableEntryInformation()
                {
                    Type = InformationType.Disruption,
                    Key = MessageKey.UNPLANNED_INFO,
                    Text = disruption.Descriptions["DE"].Text,
                    TextShort = disruption.Descriptions["DE"].TextShort
                }));

                if (src.Messages.Any()) informations.AddRange(src.Messages.Select(message => new TimetableEntryInformation()
                {
                    Type = InformationType.Message,
                    Key = MessageKeyConverter.MapToMessageKey(message.Code),
                    Text = message.Text,
                    TextShort = message.TextShort
                }));
                dest.Informations = informations;
            });

        /// Arrival
        CreateMap<RisBoards.TransportPublicOriginVia, TimetableEntryTransport>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => TransportTypeConverter.BoardsTransportToNavigatorTransport(src.Type)))
            .ForMember(dest => dest.ReplacementType, opt => opt.MapFrom(src => src.ReplacementTransport != null ? TransportTypeConverter.StringTransportToNavigatorTransport(src.ReplacementTransport.RealType) : (TransportType?)null))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.CategoryInternal, opt => opt.MapFrom(src => src.CategoryInternal))
            .ForMember(dest => dest.JourneyDescription, opt => opt.MapFrom(src => src.JourneyDescription))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.Line, opt => opt.MapFrom(src => src.Line));

        CreateMap<RisBoards.StopArrival, TimetableArrival>()
            .ForMember(dest => dest.JourneyId, opt => opt.MapFrom(src => src.JourneyID))
            .ForMember(dest => dest.Administration, opt => opt.MapFrom(src => src.Administration))
            .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src.Transport.Origin))
            .ForMember(dest => dest.DifferingOrigin, opt => opt.MapFrom(src => src.Transport.DifferingOrigin))
            .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => src.Transport.Direction.StopPlaces))
            .ForMember(dest => dest.ViaStops, opt => opt.MapFrom(src => src.Transport.Via))
            .ForMember(dest => dest.Schedule, opt => opt.MapFrom(src => new TimetableEntrySchedule()
            {
                PlannedTime = src.TimeSchedule,
                ActualTime = src.Time,
                PlannedPlatform = src.PlatformSchedule,
                ActualPlatform = src.Platform
            }))
            .ForMember(dest => dest.Informations, opt => opt.Ignore())
            .ForMember(dest => dest.Cancelled, opt => opt.MapFrom(src => src.Canceled))
            .ForMember(dest => dest.Additional, opt => opt.MapFrom(src => src.Additional))
            .ForMember(dest => dest.Demand, opt => opt.MapFrom(src => src.OnDemand))
            .ForMember(dest => dest.TravelsWith, opt => opt.MapFrom(src => src.TravelsWith.Select(travelsWith => travelsWith.JourneyID)))
            .AfterMap((src, dest, context) =>
            {
                dest.Transport.JourneyType = JourneyTypeConverter.MapBoardsJourneyTypeOrThrow(src.JourneyType);

                var informations = new List<TimetableEntryInformation>();

                if (src.Attributes.Any()) informations.AddRange(src.Attributes.Select(attribute => new TimetableEntryInformation()
                {
                    Type = InformationType.JourneyAttribute,
                    Key = MessageKeyConverter.MapToMessageKey(attribute.Code),
                    Text = attribute.Text,
                    TextShort = attribute.TextShort
                }));

                if (src.Disruptions.Any(disruption => disruption.Descriptions.ContainsKey("DE"))) informations.AddRange(src.Disruptions.Select(disruption => new TimetableEntryInformation()
                {
                    Type = InformationType.Disruption,
                    Key = MessageKey.UNPLANNED_INFO,
                    Text = disruption.Descriptions["DE"].Text,
                    TextShort = disruption.Descriptions["DE"].TextShort
                }));

                if (src.Messages.Any()) informations.AddRange(src.Messages.Select(message => new TimetableEntryInformation()
                {
                    Type = InformationType.Message,
                    Key = MessageKeyConverter.MapToMessageKey(message.Code),
                    Text = message.Text,
                    TextShort = message.TextShort
                }));
                dest.Informations = informations;
            });
    }

    private int ParseEvaOrThrow(string? evaNumberStr)
    {
        if (!int.TryParse(evaNumberStr, out var evaNumber))
            throw new AutoMapperMappingException($"Value '{evaNumberStr}' is not a valid EvaNumber");
        return evaNumber;
    }
}
