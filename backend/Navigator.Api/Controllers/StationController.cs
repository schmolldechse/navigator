using Microsoft.AspNetCore.Mvc;
using Navigator.Data.DTOs.Station;
using Navigator.Data.Repository.Stations;

namespace Navigator.Api.Controllers;

[ApiController]
[Route("api/v1/stations")]
[Tags("Stations")]
public class StationController(IStationsRepository stationsRepository) : ControllerBase
{
    /// <summary>
    /// Search for stations based on the provided criteria.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<StationSearchRequestDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchStations([FromBody] StationSearchRequestDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var stations = await stationsRepository.GetStationsAsync(dto);
        return Ok(stations);
    }
}
