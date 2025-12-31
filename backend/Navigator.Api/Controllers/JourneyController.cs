using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Navigator.Api.DTOs.Journey;
using Navigator.Data.Models.Journey;
using Navigator.Data.Repository.JourneyRepository;

namespace Navigator.Api.Controllers;

[ApiController]
[Route("api/v1/journey")]
[Tags("Journey")]
public class JourneyController(
    IJourneyRepository journeyRepository,
    IMapper mapper
) : Controller
{
    /// <summary>
    /// Load a journey by its JourneyID
    /// </summary>
    [HttpGet]
    [ProducesResponseType<Journey>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetJourney([FromQuery] [MaxLength(82)] string journeyId)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var journey = await journeyRepository.GetJourneyAsync(journeyId);
        return Ok(mapper.Map<Journey>(journey));
    }
    
    /// <summary>
    /// Load a batch of journeys
    /// </summary>
    [HttpPost("batch")]
    [ProducesResponseType<IEnumerable<Journey>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        var journeys = await journeyRepository.GetJourneysBatchAsync(batchRequest);
        return Ok(mapper.Map<IEnumerable<Journey>>(journeys?.Journeys));
    }
}