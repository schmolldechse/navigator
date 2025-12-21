using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Navigator.Api.DTOs.Timetable;
using Navigator.Data.Models.Timetable;
using Navigator.Data.Repository.TimetableRepository;

namespace Navigator.Api.Controllers;

[ApiController]
[Route("api/v1/timetable")]
[Tags("Timetable")]
public class TimetableController(
    ITimetableRepository timetableRepository,
    IMapper mapper
) : Controller
{
    /// <summary>
    /// Lookup departures
    /// </summary>
    [HttpPost("departures")]
    [ProducesResponseType<IEnumerable<TimetableDeparture>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDepartures([FromBody] TimetableRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var timetable = await timetableRepository.GetDeparturesAsync(new RisBoardRequest()
        {
            EvaNumber = request.EvaNumber,
            TimeStart = request.When!.Value,
            Duration = request.Duration!.Value
        });

        return Ok(mapper.Map<IEnumerable<TimetableDeparture>>(timetable.Departures));
    }

    /// <summary>
    /// Lookup arrivals
    /// </summary>
    [HttpPost("arrivals")]
    [ProducesResponseType<IEnumerable<TimetableArrival>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetArrivals([FromBody] TimetableRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var timetable = await timetableRepository.GetArrivalsAsync(new RisBoardRequest()
        {
            EvaNumber = request.EvaNumber,
            TimeStart = request.When!.Value,
            Duration = request.Duration!.Value
        });

        return Ok(mapper.Map<IEnumerable<TimetableArrival>>(timetable.Arrivals));
    }
}
