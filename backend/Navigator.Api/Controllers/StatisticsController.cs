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
    /// Estimates database size
    /// </summary>
    [HttpPost("estimate-size")]
    [ProducesResponseType<MeasuredTimerangeStatistic>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EstimateSize([FromBody] EstimateSizeRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var now = DateTimeOffset.UtcNow;
        var isInbetween = request.Start <= now && now <= request.End;

        var values = mapper.Map<IEnumerable<MeasuredStatisticValue>>(await statisticsRepository.GetSizesByTimerangeAsync(request.Start, request.End));
        if (isInbetween)
        {
            var estimatedSize = await statisticsRepository.EstimateDatabaseSizeAsync();
            if (estimatedSize is not null) values = values.Append(new MeasuredStatisticValue
            {
                Date = now,
                Value = estimatedSize.SizeInBytes
            });
        }

        return Ok(new MeasuredTimerangeStatistic()
        {
            Timerange = new Timerange()
            {
                Start = request.Start,
                End = request.End
            },
            Values = values,
            Unit = StatisticUnit.Bytes,
            Total = values.LastOrDefault()?.Value ?? 0,
            StartedWith = values.FirstOrDefault()?.Value ?? 0,
            ChangedBy = (values.LastOrDefault()?.Value ?? 0) - (values.FirstOrDefault()?.Value ?? 0)
        });
    }
}
