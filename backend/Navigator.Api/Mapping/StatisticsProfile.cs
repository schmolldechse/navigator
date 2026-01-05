using AutoMapper;
using Navigator.Api.DTOs.Statistics;
using Navigator.Api.Enums;
using Navigator.Data.Entities.Statistics;
using Navigator.Data.Models.Statistics;

namespace Navigator.Api.Mapping;

public class StatisticsProfile : Profile
{
    public StatisticsProfile()
    {
        // DatabaseSize
        CreateMap<DatabaseSize, MetricDataPoints>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.MeasuredAt))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.SizeInBytes));

        CreateMap<BaseSnapshot<DatabaseSize>, IEnumerable<MetricSeries>>()
            .ConvertUsing((src, dest, context) =>
            {
                var dataPoints = context.Mapper.Map<IEnumerable<MetricDataPoints>>(src.Values);

                var series = new MetricSeries()
                {
                    Unit = MetricUnit.Bytes,
                    Type = MetricType.DatabaseSize,
                    Timerange = null!,
                    Summary = CalculateSummary(dataPoints, (long)src.StartedWith, (long)src.Total),
                    DataPoints = dataPoints
                };

                return new[] { series };
            });

        // RecordedRisIds
        CreateMap<BaseSnapshot<RisIdSnapshot>, IEnumerable<MetricSeries>>()
            .ConvertUsing((src, dest, context) =>
            {
                var activeDataPoints = src.Values
                    .Select(dataPoint => new MetricDataPoints()
                    {
                        Timestamp = dataPoint.MeasuredAt,
                        Value = dataPoint.Active
                    })
                    .ToList();
                var inactiveDataPoints = src.Values
                    .Select(dataPoint => new MetricDataPoints()
                    {
                        Timestamp = dataPoint.MeasuredAt,
                        Value = dataPoint.Inactive
                    })
                    .ToList();

                var activeSeries = new MetricSeries()
                {
                    Unit = MetricUnit.Count,
                    Type = MetricType.RecordedActiveRisIds,
                    Timerange = null!,
                    Summary = CalculateSummary(activeDataPoints, null, null),
                    DataPoints = activeDataPoints
                };

                var inactiveSeries = new MetricSeries()
                {
                    Unit = MetricUnit.Count,
                    Type = MetricType.RecordedInactiveRisIds,
                    Timerange = null!,
                    Summary = CalculateSummary(inactiveDataPoints, null, null),
                    DataPoints = inactiveDataPoints
                };

                return new[] { activeSeries, inactiveSeries };
            });

        // RecordedJourneys
        CreateMap<JourneySnapshot, MetricDataPoints>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.MeasuredAt))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Total));

        CreateMap<BaseSnapshot<JourneySnapshot>, IEnumerable<MetricSeries>>()
            .ConvertUsing((src, dest, context) =>
            {
                var dataPoints = src.Values
                    .Select(dataPoint => new MetricDataPoints()
                    {
                        Timestamp = dataPoint.MeasuredAt,
                        Value = dataPoint.Total
                    })
                    .ToList();

                var series = new MetricSeries()
                {
                    Unit = MetricUnit.Count,
                    Type = MetricType.RecordedJourneys,
                    Timerange = null!,
                    Summary = CalculateSummary(dataPoints, null, null),
                    DataPoints = dataPoints
                };

                return new[] { series };
            });
    }

    private MetricSummary CalculateSummary(IEnumerable<MetricDataPoints> dataPoints, decimal? explicitStart, decimal? explicitEnd)
    {
        if (!dataPoints.Any())
        {
            var defaultValue = explicitStart ?? 0;
            return new MetricSummary()
            {
                StartValue = defaultValue,
                EndValue = explicitEnd ?? defaultValue,
                AbsoluteChange = (explicitEnd ?? defaultValue) - defaultValue,
                MinValue = defaultValue,
                MaxValue = defaultValue
            };
        }

        var startValue = explicitStart ?? dataPoints.First().Value;
        var endValue = explicitEnd ?? dataPoints.Last().Value;

        var allValues = dataPoints
            .Select(dataPoint => dataPoint.Value)
            .Append(startValue);

        return new MetricSummary()
        {
            StartValue = startValue,
            EndValue = endValue,
            AbsoluteChange = endValue - startValue,
            MinValue = allValues.Min(),
            MaxValue = allValues.Max()
        };
    }
}
