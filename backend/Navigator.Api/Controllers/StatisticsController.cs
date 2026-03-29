using Microsoft.AspNetCore.Mvc;
using Navigator.Api.DTOs.Statistics;
using Navigator.Api.Mapping;
using Navigator.Data.Repository.StatisticsRepository;

namespace Navigator.Api.Controllers;

[ApiController]
[Route("api/v1/statistics")]
[Tags("Statistics")]
public class StatisticsController(
    IStatisticsRepository statisticsRepository,
    StatisticsMapper mapper
) : Controller
{
    [HttpPost("metrics")]
    [ProducesResponseType<MetricSeries>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("Load metric series")]
    [EndpointDescription("Retrieves measured metric statistics for the specified time range.")]
    public async Task<IActionResult> GetMetric([FromBody] BaseMetricRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var metric = await statisticsRepository.GetMetricAsync(mapper.MapBaseRequest(request));
        return Ok(mapper.MapMetricSeries(metric));
    }
}
