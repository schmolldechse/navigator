using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Navigator.Api.DTOs.Statistics;
using Navigator.Api.Enums;
using Navigator.Data.Repository.StatisticsRepository;

namespace Navigator.Api.Controllers;

[ApiController]
[Route("api/v1/statistics")]
[Tags("Statistics")]
public class StatisticsController(
    IStatisticsRepository statisticsRepository,
    IMapper mapper
) : Controller
{
    /// <summary>
    /// Retrieves measured metric statistics for the specified time range.
    /// </summary>
    [HttpPost("metrics")]
    [ProducesResponseType<IEnumerable<MetricSeries>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMetric([FromBody] MetricQueryResult request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        object repositoryResult = request.MetricType switch
        {
            MetricType.DatabaseSize => await statisticsRepository.GetDatabaseSizeSnapshotAsync(request.Start, request.End),
            MetricType.RecordedRisIds => await statisticsRepository.GetRisIdSnapshotAsync(request.Start, request.End),
            MetricType.RecordedJourneys => await statisticsRepository.GetJourneySnapshotAsync(request.Start, request.End),
            _ => throw new InvalidOperationException("Unreachable code reached.")
        };

        var seriesResponse = mapper.Map<IEnumerable<MetricSeries>>(repositoryResult);
        foreach (var series in seriesResponse)
        {
            series.Timerange = new Timerange()
            {
                Start = request.Start,
                End = request.End
            };
        }

        return Ok(seriesResponse);
    }
}
