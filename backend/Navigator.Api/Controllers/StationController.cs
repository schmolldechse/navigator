using Microsoft.AspNetCore.Mvc;
using Navigator.Api.DTOs.Station;
using Navigator.Api.Mapping;
using Navigator.Data.Repository.StationRepository;
using Navigator.Data.Repository.StationRil100Repository;
using Navigator.Data.Repository.StationTransportRepository;
using System.ComponentModel.DataAnnotations;

namespace Navigator.Api.Controllers;

[ApiController]
[Route("api/v1/stations")]
[Tags("Stations")]
public class StationController(
    IStationRepository stationsRepository,
    IStationRil100Repository stationRil100Repository,
    IStationTransportRepository stationTransportRepository,
    StationMapper mapper
) : ControllerBase
{
    [HttpPost("search")]
    [ProducesResponseType<IEnumerable<Station>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [EndpointSummary("Search stations")]
    [EndpointDescription("Searches for stations by a search term.")]
    public async Task<IActionResult> SearchStations([Required][FromBody] StationBySerchtermRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var mappedRequest = mapper.MapStationBySearchtermRequest(request);

        var stations = (await stationsRepository.GetVendoStationsAsync(mappedRequest))
            .Select(station => mapper.MapVendoStation(station));
        var evaNumbers = stations.Select(station => station.EvaNumber).ToList();

        var (ril100, transports) = (
            await stationRil100Repository.GetRilByEvaNumbersAsync(evaNumbers.ToArray()),
            await stationTransportRepository.GetTransportByEvaNumbersAsync(evaNumbers.ToArray())
        );

        foreach (var station in stations)
        {
            if (ril100.Contains(station.EvaNumber)) station.Ril100 = ril100[station.EvaNumber].ToArray();
            if (transports.Contains(station.EvaNumber)) station.Transports = station.Transports.Union(transports[station.EvaNumber]).ToArray();
        }

        await stationsRepository.SaveStationsAsync(stations.Select(station => mapper.MapStationToDatabaseStation(station)));
        return Ok(stations);
    }

    [HttpPost("nearby")]
    [ProducesResponseType<IEnumerable<BaseStation>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [EndpointSummary("Stations by geographic coordinates")]
    [EndpointDescription("Finds nearby stations based on latitude and longitude. Supports optional filters for maximum distance (in meters) and result limit.")]
    public async Task<IActionResult> SearchStationByCoordinates([Required][FromBody] StationByGeographicCoordinatesRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var stations = await stationsRepository.GetStationsByCoordinatesAsync(mapper.MapStationCoordinateRequest(request));
        if (!stations.Any()) return NoContent();

        return Ok(stations.Select(station => mapper.MapDatabaseStationToBaseStation(station)));
    }

    [HttpGet("{evaNumber:int}")]
    [ProducesResponseType<Station>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Stations by EVA number")]
    [EndpointDescription("Retrieves a single station by its EVA number.")]
    public async Task<IActionResult> SearchStationByEvaNumber([Required][FromRoute] int evaNumber)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var station = await stationsRepository.GetByEvaNumberAsync(evaNumber);
        if (station is null) return NotFound("Station not found");

        return Ok(mapper.MapDatabaseStationToStation(station));
    }

    [HttpGet("{evaNumber:int}/gathering")]
    [ProducesResponseType<StationGatheringInfo>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Gathering Information")]
    [EndpointDescription("Retrieves gathering information for a station by its EVA number, including whether querying is enabled, the last queried timestamp, and the active and disabled transport types. Returns 404 Not Found if no station matches.")]
    public async Task<IActionResult> GetGatheringInfoByEvaNumber([Required][FromRoute] int evaNumber)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var station = await stationsRepository.GetByEvaNumberAsync(evaNumber);
        if (station is null) return NotFound("Station not found");

        return Ok(mapper.MapDatabaseStationToStationGatheringInfo(station));
    }

    [HttpPost("batch")]
    [ProducesResponseType<BaseStation>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("Load a station batch")]
    [EndpointDescription("Retrieves multiple stations based on the provided evaNumber batch")]
    public async Task<IActionResult> GetStationBatch([Required][FromBody] IEnumerable<int> evaNumbers)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var stations = await stationsRepository.GetStationBatch(evaNumbers);
        return Ok(stations.Select(station => mapper.MapDatabaseStationToBaseStation(station)));
    }
}
