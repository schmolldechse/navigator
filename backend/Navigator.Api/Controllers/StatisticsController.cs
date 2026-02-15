using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Navigator.Api.DTOs.Statistics;
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
    public async Task<IActionResult> GetMetric([FromBody] MetricQueryRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var metricDataSets = await statisticsRepository.GetMetricAsync(request.BuildRequest());
        return Ok(mapper.Map<IEnumerable<MetricSeries>>(metricDataSets));
    }
}
