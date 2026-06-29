using Microsoft.AspNetCore.Mvc;
using Navigator.Api.DTOs.Timetable;
using Navigator.Api.Mapping;
using Navigator.Data.Models.Timetable;
using Navigator.Data.Repository.TimetableRepository;

namespace Navigator.Api.Controllers;

[ApiController]
[Route("api/v1/timetable")]
[Tags("Timetable")]
public class TimetableController(
    ITimetableRepository timetableRepository,
    TimetableMapper mapper
) : Controller
{
    [HttpPost("departures")]
    [ProducesResponseType<IEnumerable<TimetableDeparture>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [EndpointSummary("Lookup departures")]
    [EndpointDescription("Retrieves upcoming departures for a station by its EVA number. The time window is defined by a start time (defaults to now) and a duration in minutes (defaults to 60).")]
    public async Task<IActionResult> GetDepartures([FromBody] TimetableRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var timetable = await timetableRepository.GetDeparturesAsync(new RisBoardRequest()
        {
            EvaNumber = request.EvaNumber,
            TimeStart = request.When!.Value,
            Duration = request.Duration!.Value
        });

        return Ok(timetable.Departures.DistinctBy(departure => departure.JourneyID).Select(departure => mapper.MapDepartureTimetable(departure)));
    }

    [HttpPost("arrivals")]
    [ProducesResponseType<IEnumerable<TimetableArrival>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [EndpointSummary("Lookup arrivals")]
    [EndpointDescription("Retrieves upcoming arrivals for a station by its EVA number. The time window is defined by a start time (defaults to now) and a duration in minutes (defaults to 60).")]
    public async Task<IActionResult> GetArrivals([FromBody] TimetableRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var timetable = await timetableRepository.GetArrivalsAsync(new RisBoardRequest()
        {
            EvaNumber = request.EvaNumber,
            TimeStart = request.When!.Value,
            Duration = request.Duration!.Value
        });

        return Ok(timetable.Arrivals.DistinctBy(departure => departure.JourneyID).Select(departure => mapper.MapArrivalTimetable(departure)));
    }
}
