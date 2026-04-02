using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Navigator.Api.DTOs.Journey;
using Navigator.Api.Mapping;
using Navigator.Data.Models.Journey;
using Navigator.Data.Repository.JourneyRepository;

namespace Navigator.Api.Controllers;

[ApiController]
[Route("api/v1/journey")]
[Tags("Journey")]
public class JourneyController(
    IJourneyRepository journeyRepository,
    JourneyMapper mapper
) : Controller
{
    [HttpGet]
    [ProducesResponseType<Journey>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [EndpointSummary("Journey by ID")]
    [EndpointDescription("Retrieves a single journey by its unique journey ID. The journey ID (max 82 characters) is expected to encode a date (yyyyMMdd) in the first 8 characters followed by the journey identifier.")]
    public async Task<IActionResult> GetJourney([FromQuery][MaxLength(82)] string journeyId)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var journey = await journeyRepository.GetJourneyAsync(journeyId);
        if (journey is null) return BadRequest("Journey not found");

        return Ok(mapper.MapJourney(journey));
    }

    [HttpPost("batch")]
    [ProducesResponseType<IEnumerable<Journey>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [EndpointSummary("Journey batch")]
    [EndpointDescription("Retrieves multiple journeys in a single request. Each journey ID (max 82 characters) is expected to encode a date (yyyyMMdd) in the first 8 characters followed by the journey identifier.")]
    public async Task<IActionResult> GetJourneyBatch([FromBody] IEnumerable<string> journeyIds)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (journeyIds.Any(journeyId => journeyId.Length > 82))
            return BadRequest("One or more JourneyIds exceed the maximum length of 82 characters.");

        var batchRequest = journeyIds.Select(journeyId => new JourneyOnDateRequest
        {
            Id = journeyId.Substring(9),
            FetchingDate = DateTime.ParseExact(journeyId.Substring(0, 8), "yyyyMMdd", null)
        });

        var journeyBatch = await journeyRepository.GetJourneyBatchAsync(batchRequest);
        if (!journeyBatch.Journeys.Any()) return NoContent();

        return Ok(journeyBatch.Journeys.Select(journey => mapper.MapJourney(journey)));
    }
}