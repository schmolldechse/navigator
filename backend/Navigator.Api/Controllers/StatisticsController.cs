using Microsoft.AspNetCore.Mvc;
using Navigator.Data.Models.Statistics;
using Navigator.Data.Repository.StatisticsRepository;

namespace Navigator.Api.Controllers;

[ApiController]
[Route("api/v1/statistics")]
[Tags("Statistics")]
public class StatisticsController(
    IStatisticsRepository statisticsRepository
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

        var metric = await statisticsRepository.GetMetricAsync(request);
        return Ok(metric);
    }
}
