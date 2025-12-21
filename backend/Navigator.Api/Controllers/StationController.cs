using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Navigator.Api.DTOs.Station;
using Navigator.Data.Entities.Station;
using Navigator.Data.Models.Station;
using Navigator.Data.Repository.StationRepository;
using Navigator.Data.Repository.StationRilRepository;
using Navigator.Data.Repository.StationTransportRepository;

namespace Navigator.Api.Controllers;

[ApiController]
[Route("api/v1/stations")]
[Tags("Stations")]
public class StationController(
    IStationRepository stationsRepository,
    IStationRilRepository stationRilRepository,
    IStationTransportRepository stationTransportRepository,
    IMapper mapper
) : ControllerBase
{
    /// <summary>
    /// Searches for stations that match the specified criteria
    /// </summary>
    [HttpPost]
    [ProducesResponseType<IEnumerable<StationDTO>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchStations([FromBody] StationSearchRequestDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var stations = mapper.Map<IEnumerable<StationDTO>>(await stationsRepository.GetVendoStationsAsync(mapper.Map<VendoStationsBySearchRequest>(dto)));
        var evaNumbers = stations.Select(station => station.EvaNumber).ToList();

        var (ril100, transports) = (
            await stationRilRepository.GetRilByEvaNumbersAsync(evaNumbers.ToArray()),
            await stationTransportRepository.GetTransportByEvaNumbersAsync(evaNumbers.ToArray())
        );

        foreach (var station in stations)
        {
            if (ril100.Contains(station.EvaNumber)) station.Ril100 = ril100[station.EvaNumber].ToArray();
            if (transports.Contains(station.EvaNumber)) station.Transports = station.Transports.Union(transports[station.EvaNumber]).ToArray();
        }

        await stationsRepository.SaveStationsAsync(mapper.Map<Station[]>(stations));
        return Ok(stations);
    }

    /// <summary>
    /// Search for stations based on geographic coordinates
    /// </summary>
    [HttpPost("nearby")]
    [ProducesResponseType<IEnumerable<StationSummaryDTO>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchStationByCoordinates([FromBody] CoordinatesRequestDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var stations = await stationsRepository.GetStationsByCoordinatesAsync(mapper.Map<StationsByCoordinateRequest>(dto));
        if (!stations.Any()) return NotFound("Station not found");

        return Ok(mapper.Map<IEnumerable<StationSummaryDTO>>(stations));
    }

    /// <summary>
    /// Searches for stations using the specified EVA number
    /// </summary>
    [HttpGet]
    [ProducesResponseType<StationDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchStationByEvaNumber([FromQuery] int evaNumber)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var station = await stationsRepository.GetByEvaNumberAsync(evaNumber);
        if (station is null) return NotFound("Station not found");

        return Ok(mapper.Map<StationDTO>(station));
    }

    /// <summary>
    /// Retrieves gathering information for a station identified by its EVA number
    /// </summary>
    [HttpGet("gathering")]
    [ProducesResponseType<StationGatheringInfoDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGatheringInfoByEvaNumber([FromQuery] int evaNumber)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var station = await stationsRepository.GetByEvaNumberAsync(evaNumber);
        if (station is null) return NotFound("Station not found");

        return Ok(mapper.Map<StationGatheringInfoDTO>(station));
    }
}
