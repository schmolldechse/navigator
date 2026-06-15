using Microsoft.AspNetCore.Mvc;
using Navigator.Data.Models.Statistics.Api;
using Navigator.Data.Repository.StatisticsRepository;

namespace Navigator.Api.Controllers;

[ApiController]
[Route("api/v1/statistics")]
[Tags("Statistics")]
public class StatisticsController(
    IStatisticsRepository statisticsRepository
) : ControllerBase
{
    [HttpPost("network")]
    [ProducesResponseType<StatisticsMetricResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("Load a network statistics metric")]
    public async Task<IActionResult> GetNetworkMetric(
        [FromBody] NetworkStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await statisticsRepository.GetNetworkMetricAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("network/map-hotspots.geojson")]
    [Consumes("application/json")]
    [Produces("application/geo+json")]
    [ProducesResponseType<GeoJsonFeatureCollection>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("Load network map hotspots as GeoJSON")]
    public async Task<IActionResult> GetNetworkMapHotspots(
        [FromBody] NetworkMapHotspotsRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await statisticsRepository.GetNetworkMetricAsync(request, cancellationToken);
        if (response.Result is not NetworkMapHotspotsResult hotspots)
            throw new InvalidOperationException("Network map hotspots returned an unexpected result type.");

        return new JsonResult(hotspots.FeatureCollection)
        {
            ContentType = "application/geo+json"
        };
    }

    [HttpPost("stations")]
    [ProducesResponseType<StatisticsMetricResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("Load a station statistics metric")]
    public async Task<IActionResult> GetStationMetric(
        [FromBody] StationStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await statisticsRepository.GetStationMetricAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("lines")]
    [ProducesResponseType<StatisticsMetricResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("Load a line statistics metric")]
    public async Task<IActionResult> GetLineMetric(
        [FromBody] LineStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await statisticsRepository.GetLineMetricAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("journeys")]
    [ProducesResponseType<StatisticsMetricResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("Load a journey-number statistics metric")]
    public async Task<IActionResult> GetJourneyMetric(
        [FromBody] JourneyStatisticsMetricRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await statisticsRepository.GetJourneyMetricAsync(request, cancellationToken);
        return Ok(response);
    }
}
